using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Rova_2023.DTO.LoginDTO;
using Rova_2023.DTO.RegisterationDTO;
using Rova_2023.DTO.User_DTO;
using Rova_2023.Models;
using Rova_2023.Repository;
using Rova_2023.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;

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

            var existingUser = await CheckUserAsync(userRequestDTO.Name, userRequestDTO.Phone);

            if (existingUser != null)
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
            else if (string.IsNullOrEmpty(userRequestDTO.Phone) || !userRequestDTO.Phone.All(char.IsDigit) || userRequestDTO.Phone.Length != 10)
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
                return new ServiceResponse<string>
                {
                    success = true,
                    Errormessage = "OTP sent successfully to the phone",
                    ResultMessage = sendOtpResult.Errormessage
                };

            }
            else if (!sendOtpResult.success)
            {

                return new ServiceResponse<string>
                {
                    success = false,
                    Errormessage = "Failed to send OTP",
                    ResultMessage = sendOtpResult.Errormessage
                };
            }

            httpContextAccessor.HttpContext.Session.SetString("UserName", userRequestDTO.Name);
            httpContextAccessor.HttpContext.Session.SetString("UserPhone", userRequestDTO.Phone);

            return new ServiceResponse<string>
            {
                success = false,
                Errormessage = "Cant able to store in session",

            };
        }

        public async Task<UserResponseDTO> CheckUserAsync(string name, string phone)
        {

            var existingUser = await userRepository.CheckUserDetailsinDatabasAsync(name, phone);

            return existingUser;
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
                if (httpContext == null)
                {
                    return new ServiceResponse<string>
                    {
                        success = false,
                        Errormessage = "HttpContext is not available",
                        ResultMessage = "Failed to send OTP"
                    };
                }

                if (string.IsNullOrEmpty(phoneNumber))
                {
                    return new ServiceResponse<string>
                    {
                        success = false,
                        Errormessage = "Phone number  is empty or null",
                        ResultMessage = "Failed to send OTP"
                    };
                }


                string storedPhoneNumber = httpContext.Session.GetString("UserPhone");
                if (string.IsNullOrEmpty(storedPhoneNumber))
                {
                    return new ServiceResponse<string>
                    {
                        success = false,
                        Errormessage = "Phone number not found in session",
                        ResultMessage = "Failed to send OTP"
                    };
                }

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
                    numbers = storedPhoneNumber,
                    flash = "0",
                };

                var response = await httpClient.PostAsJsonAsync("https://www.fast2sms.com/dev/bulkV2", payload);

                if (response.IsSuccessStatusCode)
                {
                    return new ServiceResponse<string>
                    {
                        success = true,
                        ResultMessage = "Your OTP is:" + otp
                    };
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return new ServiceResponse<string>
                    {
                        success = false,
                        Errormessage = "Failed to send OTP. Response: " + responseContent
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

        public async Task<ServiceResponse<string>> VerifyOtpAsync(string enteredotp, ISession session)
        {
            string storedOTP = session.GetString("UserOTP");

            if (string.IsNullOrEmpty(storedOTP))
            {
                return new ServiceResponse<string>
                {
                    success = false,
                    Errormessage = "OTP not found ",
                    ResultMessage = "Please send OTP first."
                };
            }

            if (storedOTP == enteredotp)
            {
                var user = new Users
                {
                    Name = session.GetString("UserName"),
                    Phone = session.GetString("UserPhone"),
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
        /* public async Task<ServiceResponse<string>> LoginOtpAsync(UserLoginDTO userLoginDTO)
         {
             try
             {

                 var   phoneNumber = await userRepository.GetPhoneFromDatabaseAsync(userLoginDTO);
                 if (phoneNumber == null)
                 {
                     var errorResponse = new ServiceResponse<string>
                     {
                         success = false,
                         ResultMessage = "User already exists in the database, please login !"
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
                         ResultMessage = "Your OTP is:" + otp
                     };
                 }
                 else
                 {
                     var responseContent = await response.Content.ReadAsStringAsync();
                     return new ServiceResponse<string>
                     {
                         success = false,
                         Errormessage = "Failed to send OTP. Response: " + responseContent
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
         }*/

        public async Task<ServiceResponse<string>> VerifyLoginOtpAsync(string enteredotp, ISession session)
        {
            string storedOTP = session.GetString("UserOTP");

            if (string.IsNullOrEmpty(storedOTP))
            {
                return new ServiceResponse<string>
                {
                    success = false,
                    Errormessage = "OTP not found ",
                    ResultMessage = "Please send OTP first."
                };
            }
            if (storedOTP == enteredotp)
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
    }


        
         
}


    











    







































