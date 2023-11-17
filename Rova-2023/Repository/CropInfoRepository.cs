using Microsoft.EntityFrameworkCore;
using Rova_2023.Data;
using Rova_2023.Models;
using Rova_2023.Utilities;


namespace Rova_2023.Repository
{
    public class CropInfoRepository : ICropInfoRepository
    {
        public readonly RovaDBContext rovaDBContext;
        public CropInfoRepository(RovaDBContext rovaDBContext)
        {
            this.rovaDBContext = rovaDBContext;
        }

        public async Task<ServiceResponse<CropInfo>> addCropDetailsAsync(CropInfo cropInfo)
        {
            if (string.IsNullOrWhiteSpace(cropInfo.CropName) || string.IsNullOrWhiteSpace(cropInfo.CropDiseaseName))
            {
                return new ServiceResponse<CropInfo>()
                {
                    Data = null,
                    Success = false,
                    ErrorMessage = "Enter all the fields correctly",
                };
            }
            try
            {
                await rovaDBContext.CropInfo.AddAsync(cropInfo);
                await rovaDBContext.SaveChangesAsync();

                return new ServiceResponse<CropInfo>()
                {
                    Data = cropInfo,
                    Success = true,
                    ResultMessage = "Crop details added successfully"

                };


            }
            catch (Exception ex)
            {
                return new ServiceResponse<CropInfo>()
                {
                    Data = null,
                    Success = false,
                    ErrorMessage = ex.Message,
                    ResultMessage = "Error occured while adding, please try again later"
                };

            }
        }

        public async Task<ServiceResponse<List<CropInfo>>> getCropDetailsByModelNameAsync(string modelName)
        {
            try
            {
                if (!rovaDBContext.CropInfo.Any(u => u.ModelName == modelName))
                {
                    return new ServiceResponse<List<CropInfo>>()
                    {
                        Data = null,
                        Success = false,
                        ErrorMessage = "Crop details not found!",

                    };
                }

                List<CropInfo> cropInfos = await rovaDBContext.CropInfo.Where(i => i.ModelName == modelName).ToListAsync();

                return new ServiceResponse<List<CropInfo>>()
                {
                    Data = cropInfos,
                    Success = true,
                    ResultMessage = "Crop details found"
                };

            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<CropInfo>>()
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    ResultMessage = "Error occured, try again later!"
                };
            }
        }
    }
}


