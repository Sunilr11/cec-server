using System.ComponentModel.DataAnnotations;

namespace GDT.CEC.Repository.Models.Response
{
    public class ReturnResponse<T>
    {
        [Required]
        public string Message { get; set; }
        public T Data { get; set; }

        public int StatusCode { get; set; } = 1;
        public string LogMessage { get; set; }
    }

    public class APIResponse<T>
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
