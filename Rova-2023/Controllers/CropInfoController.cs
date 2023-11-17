using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Rova_2023.DTO.CropInfo_DTO;
using Rova_2023.Services;

namespace Rova_2023.Controllers
{
    [Route("api/CropInfo")]
    [ApiController]
    public class CropInfoController : ControllerBase
    {
        public readonly ICropInfoServices cropInfoServices;
        public CropInfoController(ICropInfoServices cropInfoServices)
        {
            this.cropInfoServices = cropInfoServices;
        }

        [AllowAnonymous]
        [EnableCors("CORSPolicy")]
        [HttpPost("addCropDetails")]
        public async Task<IActionResult> addCropDetails([FromBody] CropInfoRequestDTO cropInfoRequestDTO)
        {
            //adding crop details
            var Result = await cropInfoServices.addCropDetailsAsync(cropInfoRequestDTO);
            if (Result.Success)
                return Ok(Result);
            return BadRequest(Result);
        }

        [AllowAnonymous]
        [EnableCors("CORSPolicy")]
        [HttpGet("getAllCrops")]
        public async Task<IActionResult> getCropDetailsByModelName([FromQuery] string modelName)
        {
            //get details of the crop
            var Result = await cropInfoServices.getCropDetailsByModelNameAsync(modelName);
            if (Result.Success)
                return Ok(Result);
            return BadRequest(Result);
        }


    }
}
