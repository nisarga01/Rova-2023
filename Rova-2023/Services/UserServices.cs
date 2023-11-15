using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Rova_2023.Data;
using Rova_2023.DTO.LoginResponseDTO;
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
using Twilio.Types;

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
        public async Task<ServiceResponse<string>> AddUserDetailstoSessionAsync(UserRequestDTO UserRequestDTO)
        {
            var ExistingUser = await userRepository.CheckUserDetailsinDatabaseAsync(UserRequestDTO.Name, UserRequestDTO.Phone);
            if (ExistingUser)
            {
                var ErrorResponse = new ServiceResponse<string>
                {
                    success = false,
                    ResultMessage = "User already exists in the database, please login !"
                };
                return ErrorResponse;
            }
            if (string.IsNullOrEmpty(UserRequestDTO.Name) || !UserRequestDTO.Name.All(char.IsLetter))
            {
                var ErrorResponse = new ServiceResponse<string>
                {
                    success = false,
                    Errormessage = "Validation failed",
                    ResultMessage = "User Name should not be empty and should contain only letters"
                };
                return ErrorResponse;
            }
            if (string.IsNullOrEmpty(UserRequestDTO.Phone) || !UserRequestDTO.Phone.All(char.IsDigit) || UserRequestDTO.Phone.Length != 10)
            {
                var ErrorResponse = new ServiceResponse<string>
                {
                    success = false,
                    Errormessage = "Validation failed",
                    ResultMessage = "Phone number should not be empty and contain exactly 10 digits"
                };
                return ErrorResponse;
            }

            var SendOtpResult = await SendOtpAsync(UserRequestDTO.Phone);
            if (SendOtpResult.success)
            {
                httpContextAccessor.HttpContext.Session.SetString("UserName", UserRequestDTO.Name);
                httpContextAccessor.HttpContext.Session.SetString("UserPhone", UserRequestDTO.Phone);

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
            Random Random = new Random();
            int OtpNumber = Random.Next(1000, 9999);
            return OtpNumber.ToString("D4");
        }
        public async Task<ServiceResponse<string>> SendOtpAsync(string PhoneNumber)
        {
            try
            {

                var HttpContext = httpContextAccessor.HttpContext;
                string Otp = GenerateOtp();
                HttpContext.Session.SetString("UserOTP", Otp);

                var HttpClient = httpClientFactory.CreateClient();
                HttpClient.BaseAddress = new Uri("https://www.fast2sms.com/dev/bulkV2");
                HttpClient.DefaultRequestHeaders.Add("Authorization", configuration["Fast2Sms:ApiKey"]);

                var Payload = new
                {
                    language = "english",
                    route = "otp",
                    variables_values = Otp,
                    numbers = PhoneNumber,
                    flash = "0",
                };

                var Response = await HttpClient.PostAsJsonAsync("https://www.fast2sms.com/dev/bulkV2", Payload);
                if (Response.IsSuccessStatusCode)
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

        public async Task<ServiceResponse<Users>> VerifyOtpAsync(string EnteredOtp)
        {
            string StoredOtp = httpContextAccessor.HttpContext.Session.GetString("UserOTP");

            if (StoredOtp == EnteredOtp)
            {
                var User = new Users
                {
                    Name = httpContextAccessor.HttpContext.Session.GetString("UserName"),
                    Phone = httpContextAccessor.HttpContext.Session.GetString("UserPhone"),
                };
                var UserResponse = await userRepository.AddUsertoDatabaseAsync(User);
                if (UserResponse.success)
                {
                    return new ServiceResponse<Users>
                    {
                        success = true,
                        ResultMessage = "OTP is verified.",
                        data = User

                    };
                }
                else
                {
                    return new ServiceResponse<Users>
                    {
                        success = false,
                        Errormessage = "Failed to add user to the database",
                        ResultMessage = "Please try again later."
                    };
                }

            }
            else
            {
                return new ServiceResponse<Users>
                {
                    success = false,
                    Errormessage = "Incorrect OTP.",
                    ResultMessage = "Please enter the correct OTP."
                };
            }
        }

        public async Task<ServiceResponse<string>> LoginAsync(string PhoneNumber)
        {
            try
            {
                var User = await userRepository.CheckUserByPhoneNumberAsync(PhoneNumber);
                if (User == null)
                {
                    var ErrorResponse = new ServiceResponse<string>
                    {
                        success = false,
                        ResultMessage = "User not found, please Signup!",
                        Errormessage = "User not found!"
                    };
                    return ErrorResponse;
                }
                var HttpContext = httpContextAccessor.HttpContext;
                HttpContext.Session.SetString("UserId", User.Id.ToString());
                HttpContext.Session.SetString("UserName", User.Name);
                HttpContext.Session.SetString("UserPhone", User.Phone);

                string Otp = GenerateOtp();
                var HttpClient = httpClientFactory.CreateClient();
                HttpClient.BaseAddress = new Uri("https://www.fast2sms.com/dev/bulkV2");
                HttpClient.DefaultRequestHeaders.Add("Authorization", configuration["Fast2Sms:ApiKey"]);

                var Payload = new
                {
                    language = "english",
                    route = "otp",
                    variables_values = Otp,
                    numbers = PhoneNumber,
                    flash = "0",
                };

                var Response = await HttpClient.PostAsJsonAsync("https://www.fast2sms.com/dev/bulkV2", Payload);

                if (Response.IsSuccessStatusCode)
                {
                    HttpContext.Session.SetString("UserLoginOTP", Otp);

                    return new ServiceResponse<string>
                    {
                        success = true,
                        ResultMessage = "OTP sent successfully to the phone, please verify in the next step",

                    };
                }
                else
                {
                    return new ServiceResponse<string>
                    {
                        success = false,
                        Errormessage = "Failed to send OTP",
                        ResultMessage = "Error occurred while sending OTP. Please try again",

                    };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>
                {
                    success = false,
                    ResultMessage = "Failed to send OTP",
                    Errormessage = ex.Message,

                };
            }
        }

        public async Task<ServiceResponse<LoginResponseDTO>> VerifyLoginOtpAsync(string EnteredOtp)
        {
            try
            {
                string StoredOTP = httpContextAccessor.HttpContext.Session.GetString("UserLoginOTP");
                string Id = httpContextAccessor.HttpContext.Session.GetString("UserId");
                string Name = httpContextAccessor.HttpContext.Session.GetString("UserName");
                string Phone = httpContextAccessor.HttpContext.Session.GetString("UserPhone");

                if (StoredOTP == EnteredOtp)
                {
                    var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
                    var Creds = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);
                    var Claims = new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier,Id),
                    };
                
                    var Token = new JwtSecurityToken
                       (
                       issuer: configuration["Jwt:Issuer"],
                       audience: configuration["Jwt:Audience"],
                       claims: Claims,
                       expires: DateTime.UtcNow.AddHours(72),
                       signingCredentials: Creds
                       );
                    var TokenHandler = new JwtSecurityTokenHandler();
                    var TokenString = TokenHandler.WriteToken(Token);

                    var loginResponseDTO = new LoginResponseDTO()
                    {
                        Id = Convert.ToInt32(Id),
                        Name = Name,
                        Phone = Phone,
                        Token = TokenString

                    };
                    return new ServiceResponse<LoginResponseDTO>
                    {
                        success = true,
                        ResultMessage = "Token is valid up to 24 hours",
                        data = loginResponseDTO
                    };
                }
                return new ServiceResponse<LoginResponseDTO>
                {
                    data = null,
                    success = false,
                    Errormessage = "Incorrect OTP, Please enter correct otp"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<LoginResponseDTO>
                {
                    success = false,
                    ResultMessage = "Failed to send OTP, Please try again",
                    Errormessage = ex.Message,

                };

            }
        }

    }
}


           
  







            




            
    



                
            


        
    





























































