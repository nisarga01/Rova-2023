using Rova_2023.Models;
using Rova_2023.Utilities;

namespace Rova_2023.IRepository
{
    public interface ICropInfoRepository
    {
        Task<ServiceResponse<CropInfo>> addCropDetailsAsync(CropInfo cropInfo);
        Task<ServiceResponse<CropInfo>> getCropDetailsByModelNameAsync(string modelName);

    }
}
