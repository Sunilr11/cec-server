using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using GDT.CEC.Repository.Models.AzureLabs;
using System.Text.Json.Serialization;

namespace GDT.CEC.Repository.Models.Users
{
    
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }
        [JsonIgnore]
        [BsonElement("password")]
        public string Password { get; set; }
        [BsonRequired]
        [BsonElement("firstname")]
        public string FirstName { get; set; }

        [BsonElement("lastname")]
        public string LastName { get; set; }
        [BsonRequired]
        [BsonElement("email")]
        public string Email { get; set; }
        [JsonIgnore]
        [BsonElement("displayname")]
        public string DisplayName { get; set; }
        [BsonElement("phonenumber")]
        public string PhoneNumber { get; set; }
        
        [BsonElement("azureobjectid")]
        public string AzureObjectID { get; set; }

        [BsonElement("status")]
        public string Status { get; set; } = "Pending";


        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;

        [BsonElement("enabled")]
        public bool Enabled { get; set; } = true;

        [BsonElement("userType")]
        public string UserType { get; set; } = "External";
       
        [BsonElement("roleId")]
        public int RoleId { get; set; }

        [BsonElement("addedTime")]
        [JsonIgnore]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime added_time { get; set; }
        [JsonIgnore]
        [BsonElement("lastlogin")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime last_login { get; set; }

        [BsonElement("areaOfInt")]
        public List<AreaOfInterest> AreaOfInt { get; set; }

        [BsonElement("labs")]
        public List<AzureLab> Labs { get; set; }
        [BsonElement("rejectReason")]
        public string RejectReason { get; set; }
    }



    public class UserRegisterConfig
    {
        public List<AreaOfInterest> areaOfInterests { get; set; }
        public List<AzureLab> azureLabs { get; set; }
    }

    public class UserPagingModel
    {
        public List<User> Users { get; set; }
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
   
    public class AreaOfInterest
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore]
        public string ID { get; set; }

        [BsonElement("value")]
        public string Value { get; set; }
        [BsonElement("label")]
        public string Label { get; set; }
    }


    public class TempUserRegister
    {
        public User User { get; set; }
        [JsonIgnore]
        public string   OTPKey { get; set; }
        [JsonIgnore]
        public string  OTPHash { get; set; }
        public string RegistrationKey { get; set; }
    }

    public class OTPVerificaton
    {
        public string key { get; set; }
        public string OTP { get; set; }
    }

    public class UserEmailVerification
    {
        public string Email { get; set; }
        public string Key { get; set; }
        [JsonIgnore]
        public string  OtpHash { get; set; }
        public bool IsVerified { get; set; }
    }

    public class UserImportStatus
    {
        public string ImportStatusKey { get; set; }
        public int TotalCount { get; set; }
        public int ProcessedCount { get; set; }
        public int SucessfulCount { get; set; }
        public List<string> Errors { get; set; }
    }

    public class PagingModel
    {
        public int pageno { get; set; }
        public int pagesize { get; set; }
        public string sortexpression { get; set; }
        public string sortdir { get; set; }
        public Dictionary<string,string> filters { get; set; }
    }
    public class StatusUpdateModel
    {
        [BsonElement("userid")]
        public string Userid { get; set; }
        [BsonElement("status")]
        public string Status { get; set; }
        public string RejectReason { get; set; } = "";
    }
}
