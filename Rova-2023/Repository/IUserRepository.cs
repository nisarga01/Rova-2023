using Rova_2023.DTO.LoginResponseDTO;
using Rova_2023.DTO.RegisterationDTO;
using Rova_2023.DTO.User_DTO;
using Rova_2023.Models;
using Rova_2023.Utilities;

namespace Rova_2023.Repository
{
    public interface IUserRepository
    {
        Task<bool> CheckUserDetailsinDatabaseAsync(string Name, string Phone);
        Task<ServiceResponse<Users>> AddUsertoDatabaseAsync(Users User);
        Task<Users> CheckUserByPhoneNumberAsync(string phoneNumber);



    }
}