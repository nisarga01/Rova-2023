using Rova_2023.Models;
namespace Rova_2023.Utilities
{
    public class ServiceResponse<T>
    {
        public T? data { get; set; }
        public bool success { get; set; }
        public string Errormessage { get; set; }
        public string ResultMessage { get; set; }

    }
}

