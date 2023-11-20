using Rova_2023.DTO.CropInfo_DTO;
using Rova_2023.Models;
using Rova_2023.IRepository;
using Rova_2023.IServices;
using Rova_2023.Utilities;

namespace Rova_2023.Services
{
    public class CropInfoServices : ICropInfoServices
    {
        public readonly ICropInfoRepository cropInfoRepository;
        public CropInfoServices(ICropInfoRepository cropInfoRepository)
        {
            this.cropInfoRepository = cropInfoRepository;
        }
        //add the crop details
        public async Task<ServiceResponse<CropInfoResponseDTO>> addCropDetailsAsync(CropInfoRequestDTO cropInfoRequestDTO)
        {
            var cropInfo = new CropInfo()
            {
                CropName = cropInfoRequestDTO.CropName,
                CropDiseaseName = cropInfoRequestDTO.CropDiseaseName,
                Symptoms = cropInfoRequestDTO.Symptoms,
                Solutions = cropInfoRequestDTO.Solutions,
                ModelName = cropInfoRequestDTO.ModelName,
            };
            var Result = await cropInfoRepository.addCropDetailsAsync(cropInfo);

            var Response = new ServiceResponse<CropInfoResponseDTO>()
            {
                Data = Result.Success ? new CropInfoResponseDTO()
                {
                    Id = Result.Data.Id,
                    CropName = Result.Data.CropName,
                    CropDiseaseName = Result.Data.CropDiseaseName,
                    Symptoms = Result.Data.Symptoms,
                    Solutions = Result.Data.Solutions,
                    ModelName = Result.Data.ModelName,
                } : null,
                Success = Result.Success,
                ErrorMessage = Result.ErrorMessage,
                ResultMessage = Result.ResultMessage
            };
            return Response;
        }

        public async Task<ServiceResponse<CropInfoResponseDTO>> getCropDetailsByModelNameAsync(string modelName)
        {
            // Fetch the crop details through modelName
            var Result = await cropInfoRepository.getCropDetailsByModelNameAsync(modelName);

            CropInfoResponseDTO cropInfoResponseDTO = null;

            if (Result.Success && Result.Data != null)
            {
                var data = Result.Data; // Assuming Result.Data is a single CropInfoResponseDTO
                cropInfoResponseDTO = new CropInfoResponseDTO()
                {
                    Id = data.Id,
                    CropName = data.CropName,
                    CropDiseaseName = data.CropDiseaseName,
                    Symptoms = data.Symptoms,
                    Solutions = data.Solutions,
                    ModelName = data.ModelName,
                };
            }
            var response = new ServiceResponse<CropInfoResponseDTO>()
            {
                Data = cropInfoResponseDTO,
                Success = Result.Success,
                ResultMessage = Result.ResultMessage,
                ErrorMessage = Result.ErrorMessage
            };

            return response;
        }

    }
}




















