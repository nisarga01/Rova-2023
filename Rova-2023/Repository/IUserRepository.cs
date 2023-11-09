using Rova_2023.DTO.LoginResponseDTO;
using Rova_2023.DTO.RegisterationDTO;
using Rova_2023.DTO.User_DTO;
using Rova_2023.Models;
using Rova_2023.Utilities;

namespace Rova_2023.Repository
{
    public interface IUserRepository
    {
        Task<bool> CheckUserDetailsinDatabaseAsync(string name, string phone);
        Task<ServiceResponse<Users>> AddUsertoDatabaseAsync(Users user);

        //Task<ServiceResponse<Users>> GetPhoneFromDatabaseAsync(UserLoginDTO userLoginDTO);
        Task<bool> GetByPhoneNumberAsync(string PhoneNumber);
        Task<ServiceResponse <bool>> CheckUserDetailsAsync(LoginResponseDTO loginResponseDTO);

    }
}
