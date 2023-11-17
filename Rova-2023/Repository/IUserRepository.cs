using Rova_2023.Models;
using Rova_2023.Utilities;

namespace Rova_2023.Repository
{
    public interface IUserRepository
    {
        Task<bool> checkUserExistOrNotAsync(string Name, string Phone);
        Task<ServiceResponse<Users>> addUserDetailsToDatabaseAsync(Users User);
        Task<Users> getUserDetailsThroughPhoneNumberAsync(string phoneNumber);
    }
}


