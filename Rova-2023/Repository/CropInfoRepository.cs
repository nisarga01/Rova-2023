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

            public async Task<ServiceResponse<CropInfo>> AddCropInfoAsync(CropInfo cropInfo)
            {

                if (string.IsNullOrWhiteSpace(cropInfo.CropName) || string.IsNullOrWhiteSpace(cropInfo.CropDiseaseName))

                {
                    return new ServiceResponse<CropInfo>()
                    {
                        data = null,
                        success = false,
                        Errormessage = "Enter all the fields correctly",
                    };
                }
                try
                {
                    await rovaDBContext.CropInfo.AddAsync(cropInfo);
                    await rovaDBContext.SaveChangesAsync();

                    return new ServiceResponse<CropInfo>()
                    {
                        data = cropInfo,
                        success = true,
                        ResultMessage = "Crop details added successfully"

                    };


                }
                catch (Exception ex)
                {
                    return new ServiceResponse<CropInfo>()
                    {
                        data = null,
                        success = false,
                        Errormessage = ex.Message,
                        ResultMessage = "Error occured while adding, please try again later"
                    };

                }
            }

        public async Task<ServiceResponse<List<CropInfo>>> GetAllCropsAsync(string modelname)
        {
            try
            {
                if (!rovaDBContext.CropInfo.Any(u => u.ModelName == modelname))
                {
                    return new ServiceResponse<List<CropInfo>>()
                    {
                        data = null,
                        success = false,
                        Errormessage = "Crop details not found!",
                        
                    };
                }


                List<CropInfo> cropInfos = await rovaDBContext.CropInfo.Where(i => i.ModelName == modelname).ToListAsync();

                return new ServiceResponse<List<CropInfo>>()
                {
                    data = cropInfos,
                    success = true,
                    ResultMessage = "Crop details found"
                };

            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<CropInfo>>()
                {
                    success = false,
                    Errormessage = ex.Message,
                    ResultMessage = "Error occured, try again later!"
                };
            }
        }


    }
}


