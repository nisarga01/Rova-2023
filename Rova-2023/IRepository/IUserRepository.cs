using Rova_2023.Models;
using Rova_2023.Utilities;

namespace Rova_2023.IRepository
{
    public interface IUserRepository
    {
        Task<bool> checkUserExistsOrNotAsync(string Name, string Phone);
        Task<ServiceResponse<Users>> addUserDetailsToDatabaseAsync(Users User);
        Task<Users> getUserDetailsByPhoneNumberAsync(string phoneNumber);
    }
}
