using Microsoft.IdentityModel.Tokens;
using Rova_2023.DTO.LoginResponseDTO;
using Rova_2023.DTO.User_DTO;
using Rova_2023.Models;
using Rova_2023.Repository;
using Rova_2023.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Rova_2023.Services
{
    public class UserServices : IUserServices
    {
        public readonly IUserRepository userRepository;
        public readonly IConfiguration Configuration;
        public readonly IHttpClientFactory httpClientFactory;
        public readonly IHttpContextAccessor httpContextAccessor;

        public UserServices(IUserRepository userRepository, IConfiguration Configuration, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            this.userRepository = userRepository;
            this.Configuration = Configuration;
            this.httpClientFactory = httpClientFactory;
            this.httpContextAccessor = httpContextAccessor;
        }

        //adding the user entered details to the session
        public async Task<ServiceResponse<string>> addUserDetailsToSessionAsync(UserRequestDTO userRequestDto)
        {
            //checking the user entered details in the database
            var existingUser = await userRepository.checkUserExistOrNotAsync(userRequestDto.Name, userRequestDto.Phone);
            if (existingUser)
            {
                var errorResponse = new ServiceResponse<string>
                {
                    Success = false,
                    ResultMessage = "User already exists in the database, please login !"
                };
                return errorResponse;
            }
            if (string.IsNullOrEmpty(userRequestDto.Name) || !userRequestDto.Name.All(char.IsLetter))
            {
                var errorResponse = new ServiceResponse<string>
                {
                    Success = false,
                    ErrorMessage = "Validation failed",
                    ResultMessage = "User Name should not be empty and should contain only letters"
                };
                return errorResponse;
            }
            if (string.IsNullOrEmpty(userRequestDto.Phone) || !userRequestDto.Phone.All(char.IsDigit) || userRequestDto.Phone.Length != 10)
            {
                var errorResponse = new ServiceResponse<string>
                {
                    Success = false,
                    ErrorMessage = "Validation failed",
                    ResultMessage = "Phone number should not be empty and contain exactly 10 digits"
                };
                return errorResponse;
            }

            //calling sendOtp method by passing a parameter
            var sendOtpResult = await sendOtpToThePhoneNumberAsync(userRequestDto.Phone);
            if (sendOtpResult.Success)
            {
                //storing the user Name and phonenumber in the session
                httpContextAccessor.HttpContext.Session.SetString("UserName", userRequestDto.Name);
                httpContextAccessor.HttpContext.Session.SetString("UserPhone", userRequestDto.Phone);

                return new ServiceResponse<string>
                {
                    Success = true,
                    ResultMessage = "OTP sent successfully to the phone, please verify in the next step",

                };

            }
            return new ServiceResponse<string>
            {
                Success = false,
                ErrorMessage = "Failed to send OTP",
            };

        }

        public string generateOtp()
        {
            Random Random = new Random();
            int otpNumber = Random.Next(1000, 9999);
            return otpNumber.ToString("D4");
        }

        //sending otp to the phone number
        public async Task<ServiceResponse<string>> sendOtpToThePhoneNumberAsync(string PhoneNumber)
        {
            try
            {
                var httpContext = httpContextAccessor.HttpContext;
                string Otp = generateOtp();
                httpContext.Session.SetString("UserOTP", Otp);// storing the otp in session

                var httpClient = httpClientFactory.CreateClient();
                httpClient.BaseAddress = new Uri("https://www.fast2sms.com/dev/bulkV2");
                httpClient.DefaultRequestHeaders.Add("Authorization", Configuration["Fast2Sms:ApiKey"]);

                var Payload = new
                {
                    language = "english",
                    route = "otp",
                    variables_values = Otp,
                    numbers = PhoneNumber,
                    flash = "0",
                };

                var Response = await httpClient.PostAsJsonAsync("https://www.fast2sms.com/dev/bulkV2", Payload);
                if (Response.IsSuccessStatusCode)
                {
                    return new ServiceResponse<string>
                    {
                        Success = true,
                        ResultMessage = "otp sent successfully",
                    };
                }
                else
                {
                    return new ServiceResponse<string>
                    {
                        Success = false,
                        ResultMessage = "Failed to send OTP",
                    };
                }

            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    ResultMessage = "Failed to send OTP",
                    ErrorMessage = ex.Message
                };
            }
        }

        //verify the otp send to the phone number
        public async Task<ServiceResponse<Users>> verifyOtpAsync(string enteredOtp)
        {
            //getting the otp stored in the session
            string storedOtp = httpContextAccessor.HttpContext.Session.GetString("UserOTP");

            if (storedOtp == enteredOtp)
            {
                var User = new Users
                {
                    Name = httpContextAccessor.HttpContext.Session.GetString("UserName"),
                    Phone = httpContextAccessor.HttpContext.Session.GetString("UserPhone"),
                };
                var userResponse = await userRepository.addUserDetailsToDatabaseAsync(User);//adding the user details from session to database
                if (userResponse.Success)
                {
                    return new ServiceResponse<Users>
                    {
                        Success = true,
                        ResultMessage = "OTP is verified.",
                        Data = User
                    };
                }
                else
                {
                    return new ServiceResponse<Users>
                    {
                        Success = false,
                        ErrorMessage = "Failed to add user to the database",
                        ResultMessage = "Please try again later."
                    };
                }

            }
            else
            {
                return new ServiceResponse<Users>
                {
                    Success = false,
                    ErrorMessage = "Incorrect OTP.",
                    ResultMessage = "Please enter the correct OTP."
                };
            }
        }

        //login for the user already existed in database by giving phone number
        public async Task<ServiceResponse<string>> LoginAsync(string phoneNumber)
        {
            try
            {
                //get the user in database through phone number
                var User = await userRepository.getUserDetailsByPhoneNumberAsync(phoneNumber);
                if (User == null)
                {
                    var errorResponse = new ServiceResponse<string>
                    {
                        Success = false,
                        ResultMessage = "User not found, please Signup!",
                        ErrorMessage = "User not found!"
                    };
                    return errorResponse;
                }
                var httpContext = httpContextAccessor.HttpContext;
                httpContext.Session.SetString("UserId", User.Id.ToString());
                httpContext.Session.SetString("UserName", User.Name);
                httpContext.Session.SetString("UserPhone", User.Phone);

                string Otp = generateOtp();
                var httpClient = httpClientFactory.CreateClient();
                httpClient.BaseAddress = new Uri("https://www.fast2sms.com/dev/bulkV2");
                httpClient.DefaultRequestHeaders.Add("Authorization", Configuration["Fast2Sms:ApiKey"]);

                var Payload = new
                {
                    language = "english",
                    route = "otp",
                    variables_values = Otp,
                    numbers = phoneNumber,
                    flash = "0",
                };

                var Response = await httpClient.PostAsJsonAsync("https://www.fast2sms.com/dev/bulkV2", Payload);

                if (Response.IsSuccessStatusCode)
                {
                    httpContext.Session.SetString("UserLoginOTP", Otp);
                    return new ServiceResponse<string>
                    {
                        Success = true,
                        ResultMessage = "OTP sent successfully to the phone, please verify in the next step",
                    };
                }
                else
                {
                    return new ServiceResponse<string>
                    {
                        Success = false,
                        ErrorMessage = "Failed to send OTP",
                        ResultMessage = "Error occurred while sending OTP. Please try again",
                    };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    ResultMessage = "Failed to send OTP",
                    ErrorMessage = ex.Message,

                };
            }
        }
        //verify the otp send to the phone number which is existed in the database
        public async Task<ServiceResponse<LoginResponseDTO>> verifyLoginOtpAsync(string enteredOtp)
        {
            try
            {
                string storedOtp = httpContextAccessor.HttpContext.Session.GetString("UserLoginOTP");
                string Id = httpContextAccessor.HttpContext.Session.GetString("UserId");
                string Name = httpContextAccessor.HttpContext.Session.GetString("UserName");
                string Phone = httpContextAccessor.HttpContext.Session.GetString("UserPhone");

                if (storedOtp == enteredOtp)
                {
                    var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]));
                    var Creds = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);
                    var Claims = new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier,Id),
                    };

                    var Token = new JwtSecurityToken
                       (
                       issuer: Configuration["Jwt:Issuer"],
                       audience: Configuration["Jwt:Audience"],
                       claims: Claims,
                       expires: DateTime.UtcNow.AddHours(72),
                       signingCredentials: Creds
                       );
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var tokenString = tokenHandler.WriteToken(Token);

                    var loginResponseDto = new LoginResponseDTO()
                    {
                        Id = Convert.ToInt32(Id),
                        Name = Name,
                        Phone = Phone,
                        Token = tokenString

                    };
                    return new ServiceResponse<LoginResponseDTO>
                    {
                        Success = true,
                        ResultMessage = "Token is valid up to 24 hours",
                        Data = loginResponseDto
                    };
                }
                return new ServiceResponse<LoginResponseDTO>
                {
                    Data = null,
                    Success = false,
                    ErrorMessage = "Incorrect OTP, Please enter correct otp"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<LoginResponseDTO>
                {
                    Success = false,
                    ResultMessage = "Failed to send OTP, Please try again",
                    ErrorMessage = ex.Message,
                };

            }
        }

    }
}
























































































