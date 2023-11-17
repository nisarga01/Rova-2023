using Rova_2023.Models;
using Rova_2023.Utilities;

namespace Rova_2023.Repository
{
    public interface ICropInfoRepository
    {
        Task<ServiceResponse<CropInfo>> addCropDetailsAsync(CropInfo cropInfo);
        Task<ServiceResponse<List<CropInfo>>> getCropDetailsByModelNameAsync(string modelName);

    }
}
