using Microsoft.AspNetCore.Mvc;
using Rova_2023.DTO.LoginDTO;
using Rova_2023.DTO.RegisterationDTO;
using Rova_2023.DTO.User_DTO;
using Rova_2023.Utilities;
using Twilio.Types;


namespace Rova_2023.Services
{
    public interface IUserServices
    {
        Task<ServiceResponse<string >> AddUserDetailstoSessionAsync(UserRequestDTO userRequestDTO);
        //Task<UserResponseDTO> CheckUserDetailsinDatabasAsync(string name, string phone);
        Task<ServiceResponse<string>> SendOtpAsync(string phoneNumber);
        Task<ServiceResponse<string>> VerifyOtpAsync(string enteredotp,ISession session);
        //Task<ServiceResponse<string>> LoginOtpAsync(UserLoginDTO userLoginDTO);
        Task<ServiceResponse<string>> VerifyLoginOtpAsync(string enteredotp, ISession session);



    }
}
