using Rova_2023.Data;
using Rova_2023.Models;
using Rova_2023.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using Rova_2023.IRepository;

namespace Rova_2023.Repository
{
    public class UserRepository : IUserRepository
    {
        public readonly RovaDBContext rovaDBContext;
        public readonly IMemoryCache memoryCache;
        public UserRepository(RovaDBContext rovaDBContext, IMemoryCache memoryCache)
        {
            this.rovaDBContext = rovaDBContext;
            this.memoryCache = memoryCache;
        }
        //check the user existance in the database
        public async Task<bool> checkUserExistsOrNotAsync(string Name, string Phone)
        {
            var isExist = await rovaDBContext.Users.AnyAsync(u => u.Name == Name || u.Phone == Phone);
            return isExist;
        }

        //add the user details to the database
        public async Task<ServiceResponse<Users>> addUserDetailsToDatabaseAsync(Users User)
        {
            try
            {
                await rovaDBContext.Users.AddAsync(User);
                await rovaDBContext.SaveChangesAsync();

                return new ServiceResponse<Users>
                {
                    Success = true,
                    Data = User
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<Users>
                {
                    Success = false,
                    ResultMessage = ex.Message
                };
            }
        }

        //fetch the user details through phone number
        public async Task<Users> getUserDetailsByPhoneNumberAsync(string PhoneNumber)
        {
            return await rovaDBContext.Users
                .Where(u => u.Phone == PhoneNumber)
                .FirstOrDefaultAsync();
        }

    }
}






















