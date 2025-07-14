using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDT.CEC.Repository.Models.AzureLabs
{
    public class LabAccessLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; }

        [BsonElement("username")]
        public string UserName { get; set; }
        [BsonElement("labname")]
        public string LabName { get; set; }
        [BsonElement("launchedAt")]
        public DateTime LaunchedAt { get; set; }

        [BsonElement("closedAt")]
        public DateTime ClosedAt { get; set; }
        [BsonElement("duration")]
        public string Duration { get; set; }

    }

    public class LabAccessLogRepoDTO
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; }
        [BsonElement("firstname")]
        public string FirstName { get; set; }
        [BsonElement("lastname")]
        public string LastName { get; set; }

        [BsonElement("labname")]
        public string LabName { get; set; }        

        [BsonElement("launchedAt")]
        public DateTime LaunchedAt { get; set; }
        [BsonElement("closedAt")]
        public DateTime ClosedAt { get; set; }
        [BsonElement("username")]
        public string UserName { get; set; }

        [BsonElement("userType")]
        public string UserType { get; set; }

        [BsonElement("duration")]
        public string Duration { get; set; }

    }



    public class LabAccessLogRepoPagingDTO
    {
        public List<LabAccessLogRepoDTO> Logs { get; set; }
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }


    public class LabAccessDetails
    {
        public int TotalSessions { get; set; }
        public TimeSpan TotalDuration { get; set; }
    }

    public class labAccessLogPagingModel
    {
        public List<LabAccessLogRepoDTO> AcessLogs { get; set; }
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }

    public class LabAccessLogCountDTO
    {
        public List<Data> Days { get; set; }
        public List<Data> Weeks { get; set; }
        public List<Data> Months { get; set; }

    }

    public class Data
    {
        public string TimeFrame { get; set; }
        public List<LabData> LabData { get; set; }

    }

    public class LabData
    {
        public string LabID { get; set; }
        public string LabName { get; set; }
        public int Count { get; set; }
    }


}
