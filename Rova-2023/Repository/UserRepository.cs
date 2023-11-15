using Rova_2023.Data;
using Rova_2023.Models;
using Rova_2023.Utilities;
using Rova_2023.DTO.RegisterationDTO;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using Rova_2023.DTO.User_DTO;
using System.Numerics;
using System.Xml.Linq;
using Rova_2023.DTO.LoginResponseDTO;
using Twilio.Jwt.AccessToken;

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

        public async Task<bool> CheckUserDetailsinDatabaseAsync(string Name, string Phone)
        {
            var IsExist = await rovaDBContext.Users.AnyAsync(u => u.Name == Name && u.Phone == Phone);
            return IsExist;
        }


        public async Task<ServiceResponse<Users>> AddUsertoDatabaseAsync(Users User)
        {
            try
            {

                await rovaDBContext.Users.AddAsync(User);
                await rovaDBContext.SaveChangesAsync();

                return new ServiceResponse<Users>
                {
                    success = true,
                    ResultMessage = "User added to the database",
                    data = User
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<Users>
                {
                    success = false,
                    Errormessage = "Failed to add user to the database",
                    ResultMessage = ex.Message
                };
            }
        }
       
        public async Task<Users> CheckUserByPhoneNumberAsync(string PhoneNumber)
        {
            var User = await rovaDBContext.Users
                .Where(u => u.Phone == PhoneNumber)
                .FirstOrDefaultAsync();

            if (User != null)
            {
                
                var users = new Users 
                {
                    Id = User.Id,
                    Name = User.Name,
                    Phone = User.Phone
                    
                };

                return users;
                
            }
            return null;

            
        }



    }
}






















