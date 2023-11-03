using Microsoft.AspNetCore.Mvc;
using Rova_2023.DTO.CropInfo_DTO;
using Rova_2023.Models;
using Rova_2023.Services;

namespace Rova_2023.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CropInfoController : ControllerBase
    {
        public readonly ICropInfoServices cropinfoServices;
        public CropInfoController(ICropInfoServices cropInfoServices)
        {
            cropinfoServices = cropInfoServices;

        }

        [HttpPost("AddCropInfo")]
        public async Task<IActionResult> AddCropInfo([FromBody] CropInfoRequestDTO cropInfoRequestDTO)
        {
            var result = await cropinfoServices.AddCropInfoAsync(cropInfoRequestDTO);
            if (result.success)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpGet("GetAllCrops")]
        public async Task<IActionResult> GetAllCrops([FromQuery] string modelname)
        {
            var result = await cropinfoServices.GetAllCropsAsync(modelname);

            if (result.success)
                return Ok(result);
            return BadRequest(result);
        }


    }
}
