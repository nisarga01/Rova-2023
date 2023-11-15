﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rova_2023.DTO.LoginResponseDTO;
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


        public UserController(IUserServices userServices, IHttpContextAccessor httpContextAccessor)
        {
            this.userServices = userServices;
            this.httpContextAccessor = httpContextAccessor;

        }
        [AllowAnonymous]
        [EnableCors("CORSPolicy")]
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] UserRequestDTO UserRequestDTO)
        {
            
            var result = await userServices.AddUserDetailstoSessionAsync(UserRequestDTO);

            if (result.success)
                return Ok(result);
            return BadRequest(result);
        }


        [AllowAnonymous]
        [EnableCors("CORSPolicy")]
        [HttpPost("VerifyOTP")]
        public async Task<IActionResult> VerifyOTP(string enteredOTP )
        {
            
            var result = await userServices.VerifyOtpAsync(enteredOTP);

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

        [AllowAnonymous]
        [EnableCors("CORSPolicy")]
        [HttpPost("login")]
        public async Task<IActionResult> Login(string PhoneNumber)
        {
            var result = await userServices.LoginAsync(PhoneNumber);

            if (result.success)
                return Ok(result);
            return BadRequest(result);
        }

        [AllowAnonymous]
        [EnableCors("CORSPolicy")]
        [HttpPost("VerifyloginOTP")]
        public async Task<IActionResult> VerifyloginOTP(string enteredotp)
        {
            var result = await userServices.VerifyLoginOtpAsync(enteredotp);

            if (result.success)
                return Ok(result);
            return BadRequest(result);
        }





    }


}



