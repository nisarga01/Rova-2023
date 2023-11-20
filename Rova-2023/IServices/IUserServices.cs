using Rova_2023.DTO.LoginResponseDTO;
using Rova_2023.DTO.User_DTO;
using Rova_2023.Models;
using Rova_2023.Utilities;

namespace Rova_2023.IServices
{
    public interface IUserServices
    {
        Task<ServiceResponse<string>> addUserDetailsToSessionAsync(UserRequestDTO userRequestDto);
        Task<ServiceResponse<string>> sendOtpToThePhoneNumberAsync(string phoneNumber);
        Task<ServiceResponse<Users>> verifyOtpAsync(string EnteredOtp);
        Task<ServiceResponse<string>> LoginAsync(string phoneNumber);
        Task<ServiceResponse<LoginResponseDTO>> verifyLoginOtpAsync(string EnteredOtp);
    }
}
