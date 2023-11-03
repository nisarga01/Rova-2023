using Newtonsoft.Json;
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
        public async Task<ServiceResponse<CropInfoResponseDTO>> AddCropInfoAsync(CropInfoRequestDTO cropInfoRequestDTO)
        {

            var cropinfo = new CropInfo()
            {
                CropName = cropInfoRequestDTO.CropName,
                CropDiseaseName = cropInfoRequestDTO.CropDiseaseName,
                Symptoms = cropInfoRequestDTO.Symptoms,
                Solutions = cropInfoRequestDTO.Solutions,
                ModelName = cropInfoRequestDTO.ModelName,

            };
            var result = await cropInfoRepository.AddCropInfoAsync(cropinfo);

            var response = new ServiceResponse<CropInfoResponseDTO>()
            {
                data = result.success ? new CropInfoResponseDTO()
                {
                    Id = result.data.Id,
                    CropName = result.data.CropName,
                    CropDiseaseName = result.data.CropDiseaseName,
                    Symptoms = result.data.Symptoms,
                    Solutions = result.data.Solutions,
                    ModelName = result.data.ModelName,
                } : null,
                success = result.success,
                Errormessage = result.Errormessage,
                ResultMessage = result.ResultMessage
            };
            return response;
        }
        public async Task<ServiceResponse<List<CropInfoResponseDTO>>> GetAllCropsAsync(string modelname)

        {
            var result = await cropInfoRepository.GetAllCropsAsync(modelname);

            var getcrops = new List<CropInfoResponseDTO>();

            if (result.success)
            {
                result.data.ForEach(d =>
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

                    getcrops.Add(cropInfoResponseDTO);
                });
            }

            var response = new ServiceResponse<List<CropInfoResponseDTO>>()
            {
                data = result.success ? getcrops : null,
                success = result.success,
                ResultMessage = result.ResultMessage,
                Errormessage = result.Errormessage
            };

            return response;
        }
    }



}












