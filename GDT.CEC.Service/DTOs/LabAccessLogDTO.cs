using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDT.CEC.Service.DTOs
{
    public class LabAccessLogDTO
    {
        [BsonElement("_id")]
        public string ID { get; set; }
        public string UserName { get; set; }
        public string UserFullName { get; set; }
        public string LabName { get; set; }
        public DateTime LaunchedAt { get; set; }

        public DateTime ClosedAt { get; set; }

        public string UserType { get; set; }
        public string Duration { get; set; }
    }
    public class LabAcccessLogPagingModel
    {
        public List<LabAccessLogDTO> Logs { get; set; }
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
}
