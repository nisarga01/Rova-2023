using Microsoft.AspNetCore.Mvc;
using Rova_2023.DTO.LoginResponseDTO;
using Rova_2023.DTO.RegisterationDTO;
using Rova_2023.DTO.User_DTO;
using Rova_2023.Models;
using Rova_2023.Utilities;
using Twilio.Types;


namespace Rova_2023.Services
{
    public interface IUserServices
    {
        Task<ServiceResponse<string>> AddUserDetailstoSessionAsync(UserRequestDTO userRequestDTO);

        Task<ServiceResponse<string>> SendOtpAsync(string phoneNumber);
        Task<ServiceResponse<Users>> VerifyOtpAsync(string enteredotp);
        Task<ServiceResponse<bool>> LoginAsync(string PhoneNumber);
        Task<ServiceResponse<string>> VerifyLoginOtpAsync(string enteredotp);
        //Task<ServiceResponse<LoginResponseDTO>> SendResponse(LoginResponseDTO loginresponsedto);



    }
}
