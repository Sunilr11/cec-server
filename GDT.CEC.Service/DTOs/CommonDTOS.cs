using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDT.CEC.Service.DTOs
{
    public  class CommonDTOS
    {
    }

    public class ImageUploadDTO
    {
        public string Base64 { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
}
