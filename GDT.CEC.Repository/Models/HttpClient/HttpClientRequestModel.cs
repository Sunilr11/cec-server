using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDT.CEC.Repository.Models.HttpClient
{
    public class HttpClientRequestModel
    {
        public string BaseUrl { get; set; }
        public string MethodNameOrUrl { get; set; }
        public string ApiKey { get; set; }
        public string AuthToken { get; set; }
        public string AuthTokenType { get; set; } = "Bearer";
        public string ResponseDataType { get; set; } = "application/json";
        public string PostDataType { get; set; } = "application/json";
    }
}
