using Rova_2023.Data;
using Rova_2023.Models;
using Rova_2023.Utilities;
using Rova_2023.DTO.LoginDTO;
using Rova_2023.DTO.RegisterationDTO;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using Rova_2023.DTO.User_DTO;

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
        /*public async Task<ServiceResponse<string>> CheckUserDetailsinDatabasAsync(string name, string phone)
        {
            
            if (rovaDBContext.Users.Any(u => u.Name == name && u.Phone == phone))
            {
                return new ServiceResponse<string>()
                {
                    success = true,

                };

            }
            else
            {
                return new ServiceResponse<string>()
                {
                    success = false,

                };
            }
            
        }*/
        public async Task<bool> CheckUserDetailsinDatabasAsync(string name, string phone)
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







        /*public async Task<ServiceResponse<Users>> GetPhoneFromDatabaseAsync(UserLoginDTO userLoginDTO )
        {
            try
            {
                Users user = null;
                var isExist = rovaDBContext.Users.Any(f => f.Phone == userLoginDTO .Phone);
                if (!isExist)
                {
                    return new ServiceResponse<Users>
                    {
                        success = false,
                        ResultMessage = "Please signup before login",
                        Errormessage = "phone number does not exist "
                    };

                }
                user = rovaDBContext.Users.FirstOrDefault(f => f.Phone == userLoginDTO .Phone);
                if (user != null)
                {
                    return new ServiceResponse<Users>
                    {
                        success = true,
                        data = user,
                        ResultMessage = "Phone number is valid"
                    };

                }
                return new ServiceResponse<Users>
                {
                    success = true,
                    ResultMessage = "Please enter correct Phone number",
                    Errormessage = "Enter correct Phone number "
                };


            }
            catch (Exception ex)
            {
                return new ServiceResponse<Users>
                {
                    success = false,
                    ResultMessage = ex.Message,
                    Errormessage = "Error occured please try again "
                };
            }
        }*/


    }











}











