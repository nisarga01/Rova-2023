using Rova_2023.DTO.CropInfo_DTO;
using Rova_2023.DTO.User_DTO;
using Rova_2023.Models;
using Rova_2023.Utilities;

namespace Rova_2023.Services
{
    public interface ICropInfoServices
    {
        Task<ServiceResponse<CropInfoResponseDTO>> AddCropInfoAsync(CropInfoRequestDTO cropInfoRequestDTO);
        Task<ServiceResponse<List<CropInfoResponseDTO>>> GetAllCropsAsync(string modelname);

    }
}
