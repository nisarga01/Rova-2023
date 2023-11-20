using Rova_2023.DTO.CropInfo_DTO;
using Rova_2023.Utilities;

namespace Rova_2023.IServices
{
    public interface ICropInfoServices
    {
        Task<ServiceResponse<CropInfoResponseDTO>> addCropDetailsAsync(CropInfoRequestDTO cropInfoRequestDTO);
        Task<ServiceResponse<CropInfoResponseDTO>> getCropDetailsByModelNameAsync(string modelName);
    }
}
