using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Rova_2023.DTO.User_DTO;
using Rova_2023.IServices;



namespace Rova_2023.Controllers
{
    [Route("api/Users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public readonly IUserServices userServices;
        public UserController(IUserServices userServices)
        {
            this.userServices = userServices;
        }

        [AllowAnonymous]
        [EnableCors("CORSPolicy")]
        [HttpPost("signUp")]
        public async Task<IActionResult> signUp([FromBody] UserRequestDTO userRequestDto)
        {
            //storing the details entered by the user to the session
            var Result = await userServices.addUserDetailsToSessionAsync(userRequestDto);
            if (Result.Success)
                return Ok(Result);
            if (!Result.Success && Result.ErrorCode == "UserAlreadyExists")
                return Conflict(Result);
            if (!Result.Success && Result.ErrorCode == "ValidationFailed")
                return UnprocessableEntity(Result);
            return BadRequest(Result);
        }

        [AllowAnonymous]
        [EnableCors("CORSPolicy")]
        [HttpPost("verifyOtp")]
        public async Task<IActionResult> verifyOtp(string enteredOtp)
        {
            // verify the otp send to the phone number
            var Result = await userServices.verifyOtpAsync(enteredOtp);
            if (Result.Success)
                return Ok(Result);
            if (!Result.Success && Result.ErrorCode == "Unauthorized")
                return Unauthorized(Result);
            return BadRequest(Result);
        }

        [AllowAnonymous]
        [EnableCors("CORSPolicy")]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(string phoneNumber)
        {
            //login by giving phone number
            var Result = await userServices.LoginAsync(phoneNumber);
            if (Result.Success)
                return Ok(Result);
            if (!Result.Success && Result.ErrorCode == "Unauthorized")
                return Unauthorized(Result);
            if (!Result.Success && Result.ErrorCode == "ValidationFailed")
                return UnprocessableEntity(Result);
            if (!Result.Success && Result.ErrorCode == "TooManyRequests")
                return StatusCode(429, Result);
            return BadRequest(Result);
        }

        [AllowAnonymous]
        [EnableCors("CORSPolicy")]
        [HttpPost("verifyLoginOtp")]
        public async Task<IActionResult> verifyLoginOtp(string enteredOtp)
        {
            //verify the otp send to the phone number
            var Result = await userServices.verifyLoginOtpAsync(enteredOtp);
            if (Result.Success)
                return Ok(Result);
            if (!Result.Success && Result.ErrorCode == "Unauthorized")
                return Unauthorized(Result);
            return BadRequest(Result);
        }
    }
}



