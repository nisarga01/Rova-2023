using Rova_2023.Models;
using Rova_2023.Utilities;
using Rova_2023.DTO.CropInfo_DTO;

namespace Rova_2023.Repository
{
    public interface ICropInfoRepository
    {
        Task<ServiceResponse<CropInfo>> AddCropInfoAsync(CropInfo cropInfo);
        Task<ServiceResponse<List<CropInfo>>> GetAllCropsAsync(string modelname);

    }
}
