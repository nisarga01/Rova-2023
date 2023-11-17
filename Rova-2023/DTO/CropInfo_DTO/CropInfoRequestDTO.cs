using System.ComponentModel.DataAnnotations;

namespace Rova_2023.DTO.CropInfo_DTO
{
    public class CropInfoRequestDTO
    {
        //public int Id { get; set; }

        [Required]
        public string CropName { get; set; }

        public string CropDiseaseName { get; set; }

        [Required]
        public List<string> Symptoms { get; set; } // Store JSON data

        [Required]
        public List<string> Solutions { get; set; } // Store JSON data

        public string? ModelName { get; set; }
    }
}
