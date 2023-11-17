using Rova_2023.DTO.CropInfo_DTO;
using Rova_2023.Utilities;

namespace Rova_2023.Services
{
    public interface ICropInfoServices
    {
        Task<ServiceResponse<CropInfoResponseDTO>> addCropDetailsAsync(CropInfoRequestDTO cropInfoRequestDTO);
        Task<ServiceResponse<List<CropInfoResponseDTO>>> getCropDetailsByModelNameAsync(string modelName);
    }
}
