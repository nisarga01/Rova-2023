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

        public async Task<bool> CheckUserDetailsinDatabaseAsync(string name, string phone)
        {
            var isExist = await rovaDBContext.Users.AnyAsync(u => u.Name == name && u.Phone == phone);
            return isExist;
        }


        public async Task<ServiceResponse<Users>> AddUsertoDatabaseAsync(Users user)
        {
            try
            {

                await rovaDBContext.Users.AddAsync(user);
                await rovaDBContext.SaveChangesAsync();

                return new ServiceResponse<Users>
                {
                    success = true,
                    ResultMessage = "User added to the database",
                    data = user
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
        public async Task<bool> GetByPhoneNumberAsync(string PhoneNumber)
        {
            var isExisting = await rovaDBContext.Users.AnyAsync(u => u.Phone == PhoneNumber);
            return isExisting;
        }

        //public async Task<ServiceResponse<LoginResponseDTO>> CheckUserDetailsAsync(string storedPhone,string tokenString)
        //{
        //    var isExisting = await rovaDBContext.Users
        //      .FirstOrDefaultAsync(u => u.Phone == storedPhone);
        //    if (isExisting != null)
        //    {
        //        var userDto = new LoginResponseDTO
        //        {
        //            Id = isExisting.Id,
        //            Phone = isExisting.Phone,
        //            Token = tokenString,
        //        };
        //        await rovaDBContext.SaveChangesAsync();
        //        return new ServiceResponse<LoginResponseDTO>
        //        {
        //            data = userDto,
        //            success = true,
        //            ResultMessage = "User Details found"

        //        };


        //    }

        //    return new ServiceResponse<LoginResponseDTO>
        //    {
        //        success = false,
        //        data = null,
        //        Errormessage = "User Details not found"
        //    };
        //}
        public async Task<ServiceResponse<LoginResponseDTO>> CheckUserDetailsAsync(string storedPhone, string tokenString)
        {
            var isExisting = await rovaDBContext.Users
        .FirstOrDefaultAsync(u => u.Phone == storedPhone);

            if (isExisting != null)
            {
                var userDto = new LoginResponseDTO
                {
                    Id = isExisting.Id,
                    Phone = isExisting.Phone,
                    Name = isExisting.Name,
                    Token = tokenString
                };

                await rovaDBContext.SaveChangesAsync();

                return new ServiceResponse<LoginResponseDTO>
                {
                    data = userDto,
                    success = true,
                    ResultMessage = "User Details found"
                };
            }

            return new ServiceResponse<LoginResponseDTO>
            {
                success = false,
                data = null,
                Errormessage = "User Details not found"
            };
        }
    }
}






















