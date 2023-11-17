using Rova_2023.DTO.CropInfo_DTO;
using Rova_2023.Models;
using Rova_2023.Repository;
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
        public async Task<ServiceResponse<List<CropInfoResponseDTO>>> getCropDetailsByModelNameAsync(string modelName)
        {
            //fetch the crop details through modelName
            var Result = await cropInfoRepository.getCropDetailsByModelNameAsync(modelName);

            var getCrops = new List<CropInfoResponseDTO>();

            if (Result.Success)
            {
                Result.Data.ForEach(d =>
                {
                    CropInfoResponseDTO cropInfoResponseDTO = new CropInfoResponseDTO()
                    {
                        Id = d.Id,
                        CropName = d.CropName,
                        CropDiseaseName = d.CropDiseaseName,
                        Symptoms = d.Symptoms,
                        Solutions = d.Solutions,
                        ModelName = d.ModelName,
                    };

                    getCrops.Add(cropInfoResponseDTO);
                });
            }
            var Response = new ServiceResponse<List<CropInfoResponseDTO>>()
            {
                Data = Result.Success ? getCrops : null,
                Success = Result.Success,
                ResultMessage = Result.ResultMessage,
                ErrorMessage = Result.ErrorMessage
            };
            return Response;
        }
    }
}
















