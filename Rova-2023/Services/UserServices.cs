using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Rova_2023.Data;
using Rova_2023.DTO.LoginDTO;
using Rova_2023.DTO.RegisterationDTO;
using Rova_2023.DTO.User_DTO;
using Rova_2023.Models;
using Rova_2023.Repository;
using Rova_2023.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Numerics;
using System.Security.Claims;
using System.Text;
using System.Xml.Linq;

namespace Rova_2023.Services
{
    public class UserServices : IUserServices
    {
        public readonly IUserRepository userRepository;
        public readonly IConfiguration configuration;
        public readonly IHttpClientFactory httpClientFactory;
        public readonly IHttpContextAccessor httpContextAccessor;

        public UserServices(IUserRepository userRepository, IConfiguration configuration, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            this.userRepository = userRepository;
            this.configuration = configuration;
            this.httpClientFactory = httpClientFactory;
            this.httpContextAccessor = httpContextAccessor;


        }
        public async Task<ServiceResponse<string>> AddUserDetailstoSessionAsync(UserRequestDTO userRequestDTO)
        {
            var existingUser = await userRepository.CheckUserDetailsinDatabaseAsync(userRequestDTO.Name, userRequestDTO.Phone);
            if (existingUser)
            {
                var errorResponse = new ServiceResponse<string>
                {
                    success = false,
                    ResultMessage = "User already exists in the database, please login !"
                };
                return errorResponse;
            }
            if (string.IsNullOrEmpty(userRequestDTO.Name) || !userRequestDTO.Name.All(char.IsLetter))
            {
                var errorResponse = new ServiceResponse<string>
                {
                    success = false,
                    Errormessage = "Validation failed",
                    ResultMessage = "User Name should not be empty and should contain only letters"
                };
                return errorResponse;
            }
            if (string.IsNullOrEmpty(userRequestDTO.Phone) || !userRequestDTO.Phone.All(char.IsDigit) || userRequestDTO.Phone.Length != 10)
            {
                var errorResponse = new ServiceResponse<string>
                {
                    success = false,
                    Errormessage = "Validation failed",
                    ResultMessage = "Phone number should not be empty and contain exactly 10 digits"
                };
                return errorResponse;
            }

            var sendOtpResult = await SendOtpAsync(userRequestDTO.Phone);
            if (sendOtpResult.success)
            {
                httpContextAccessor.HttpContext.Session.SetString("UserName", userRequestDTO.Name);
                httpContextAccessor.HttpContext.Session.SetString("UserPhone", userRequestDTO.Phone);

                return new ServiceResponse<string>
                {
                    success = true,
                    ResultMessage = "OTP sent successfully to the phone, please verify in the next step",

                };

            }
            return new ServiceResponse<string>
            {
                success = false,
                Errormessage = "Failed to send OTP",

            };

        }

        public string GenerateOtp()
        {
            Random random = new Random();
            int otpNumber = random.Next(1000, 9999);
            return otpNumber.ToString("D4");
        }
        public async Task<ServiceResponse<string>> SendOtpAsync(string phoneNumber)
        {
            try
            {

                var httpContext = httpContextAccessor.HttpContext;
                string otp = GenerateOtp();
                httpContext.Session.SetString("UserOTP", otp);

                var httpClient = httpClientFactory.CreateClient();
                httpClient.BaseAddress = new Uri("https://www.fast2sms.com/dev/bulkV2");
                httpClient.DefaultRequestHeaders.Add("Authorization", configuration["Fast2Sms:ApiKey"]);

                var payload = new
                {
                    language = "english",
                    route = "otp",
                    variables_values = otp,
                    numbers = phoneNumber,
                    flash = "0",
                };

                var response = await httpClient.PostAsJsonAsync("https://www.fast2sms.com/dev/bulkV2", payload);
                if (response.IsSuccessStatusCode)
                {
                    return new ServiceResponse<string>
                    {

                        success = true,
                        ResultMessage = "otp sent successfully",
                    };
                }
                else
                {
                    return new ServiceResponse<string>
                    {
                        success = false,
                        ResultMessage = "Failed to send OTP",
                    };
                }

            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>
                {
                    success = false,
                    ResultMessage = "Failed to send OTP",
                    Errormessage = ex.Message

                };
            }
        }

        public async Task<ServiceResponse<string>> VerifyOtpAsync(string enteredotp)
        {
            string storedOTP = httpContextAccessor.HttpContext.Session.GetString("UserOTP");

            if (storedOTP == enteredotp)
            {
                var user = new Users
                {
                    Name = httpContextAccessor.HttpContext.Session.GetString("UserName"),
                    Phone = httpContextAccessor.HttpContext.Session.GetString("UserPhone"),
                };
                var userResponse = await userRepository.AddUsertoDatabaseAsync(user);
                if (userResponse.success)
                {
                    return new ServiceResponse<string>
                    {

                        success = true,
                        ResultMessage = "OTP is verified."

                    };
                }
                else
                {
                    return new ServiceResponse<string>
                    {
                        success = false,
                        Errormessage = "Failed to add user to the database",
                        ResultMessage = "Please try again later."
                    };
                }

            }
            else
            {
                return new ServiceResponse<string>
                {
                    success = false,
                    Errormessage = "Incorrect OTP.",
                    ResultMessage = "Please enter the correct OTP."
                };
            }
        }

        public async Task<ServiceResponse<string>> LoginAsync(UserLoginDTO userLoginDTO)
        {
            try
            {

                var phoneNumber = await userRepository.GetPhoneFromDatabaseAsync(userLoginDTO);
                if (!phoneNumber)
                {
                    var errorResponse = new ServiceResponse<string>
                    {
                        success = false,
                        ResultMessage = "User not exists in the database, please Signup before login !"
                    };
                    return errorResponse;


                }
                var httpContext = httpContextAccessor.HttpContext;

                string otp = GenerateOtp();
                httpContext.Session.SetString("UserLoginOTP", otp);

                var httpClient = httpClientFactory.CreateClient();
                httpClient.BaseAddress = new Uri("https://www.fast2sms.com/dev/bulkV2");
                httpClient.DefaultRequestHeaders.Add("Authorization", configuration["Fast2Sms:ApiKey"]);

                var payload = new
                {
                    language = "english",
                    route = "otp",
                    variables_values = otp,
                    numbers = phoneNumber,
                    flash = "0",
                };

                var response = await httpClient.PostAsJsonAsync("https://www.fast2sms.com/dev/bulkV2", payload);

                if (response.IsSuccessStatusCode)
                {
                    return new ServiceResponse<string>
                    {
                        success = true,
                        ResultMessage = "OTP sent successfully to the phone, please verify in the next step",
                    };
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return new ServiceResponse<string>
                    {
                        success = false,
                        Errormessage = "Failed to send OTP"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>
                {
                    success = false,
                    ResultMessage = "Failed to send OTP",
                    Errormessage = ex.Message
                };
            }
        }

        //public async Task<ServiceResponse<string>> VerifyLoginOtpAsync(string enteredotp, ISession session)
        //{
        //    string storedOTP = session.GetString("UserOTP");

        //    if (string.IsNullOrEmpty(storedOTP))
        //    {
        //        return new ServiceResponse<string>
        //        {
        //            success = false,
        //            Errormessage = "OTP not found ",
        //            ResultMessage = "Please send OTP first."
        //        };
        //    }
        //    if (storedOTP == enteredotp)
        //    {
        //        return new ServiceResponse<string>
        //        {
        //            success = true,
        //            ResultMessage = "OTP is verified."
        //        };
        //    }
        //    else
        //    {
        //        return new ServiceResponse<string>
        //        {
        //            success = false,
        //            Errormessage = "Failed to add user to the database",
        //            ResultMessage = "Please try again later."
        //        };
        //    }
        //}
    }




}






















































