using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rova_2023.Models
{
    public class CropInfo
    {
        public int Id { get; set; }

        [Required]
        public string CropName { get; set; }

        public string CropDiseaseName { get; set; }

        [Required]
        public List<string> Symptoms { get; set; }

        [Required]
        public List<string> Solutions { get; set; }

        public string? ModelName { get; set; }
    }
}
