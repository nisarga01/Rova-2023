using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rova_2023.DTO.LoginDTO;
using Rova_2023.DTO.RegisterationDTO;
using Rova_2023.DTO.User_DTO;
using Rova_2023.Services;


namespace Rova_2023.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public readonly IUserServices userServices;
        public readonly IHttpContextAccessor httpContextAccessor;
        
        
        public UserController(IUserServices userServices,IHttpContextAccessor httpContextAccessor )
        {
            this.userServices = userServices; 
            this.httpContextAccessor = httpContextAccessor;
            
        }
       
        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn([FromBody] UserRequestDTO userRequestDTO )
         {
 
            var result = await userServices.AddUserDetailstoSessionAsync(userRequestDTO);

            if (result.success)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("VerifyOTP")]
        public async Task<IActionResult> VerifyOTP(string enteredOTP)
        {
            var session = HttpContext.Session;
            var result = await userServices.VerifyOtpAsync(enteredOTP,session);

            if (result.success)
                return Ok(result);

            return BadRequest(result);

        }




        /*[HttpPost("SendOTP")]
        public async Task<IActionResult> SendOTP(string phonenumber)
        {
            
            var result = await userServices.SendOtpAsync(phonenumber);

            if (result.success)

                return Ok(result);
            return BadRequest(result);
        }*/
        /*[HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDTO userlogindto)
        {
            var result = await userServices.LoginAsync(userlogindto, HttpContext.Session);

            if (result.success)

                return Ok(result);
            return BadRequest(result);
        }*/
        /*[HttpPost("VerifyloginOTP")]
        public async Task<IActionResult> VerifyloginOTP(UserLoginDTO userloginDTO, string enteredotp)
        {
            var result = await userServices.VerifyloginOTPAsync(userloginDTO, enteredotp, HttpContext.Session);

            if (result.success)
                return Ok(result);
            return BadRequest(result);
        }*/





    }


}



