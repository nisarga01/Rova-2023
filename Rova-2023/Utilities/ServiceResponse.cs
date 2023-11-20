using Rova_2023.Models;
namespace Rova_2023.Utilities
{
    public class ServiceResponse<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string ResultMessage { get; set; }
        public string ErrorCode { get;  set; }
    }
}

