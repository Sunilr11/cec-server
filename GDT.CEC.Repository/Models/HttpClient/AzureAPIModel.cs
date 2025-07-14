using GDT.CEC.Repository.Models.Response;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace GDT.CEC.Repository.Models.HttpClient
{
    public class AzureAPIModel
    {

    }

    public class AzureCreateUserReqModel
    {
        public bool accountEnabled { get; set; }
        public string displayName { get; set; }
        public string mailNickname { get; set; }
        public string userPrincipalName { get; set; }
        public PasswordProfile passwordProfile { get; set; }
    }
    public class AzureCreateUserRespModel
    {

        public string id { get; set; }
        public string displayName { get; set; }
        public string mail { get; set; }
        public string userPrincipalName { get; set; }
        public string jobTitle { get; set; }
        public string department { get; set; }
        public string city { get; set; }
        public string country { get; set; }

    }

    public class AzureApiErrorMessage
    {
        public AzureApiError error { get; set; }
    }

    public class AzureApiError
    {
        public string code { get; set; }
        public string message { get; set; }
    }

    public class PasswordProfile
    {
        public bool forceChangePasswordNextSignIn { get; set; }
        public string password { get; set; }
    }

    public class AzureUpdateRoleAssignmentReqModel
    {
        public UpdateRoleProperties properties { get; set; }
    }

    public class UpdateRoleProperties
    {
        public string principalId { get; set; }
        public string roleDefinitionId { get; set; }
    }    
    public class AzureUpdateRoleAssignmentRespModel
    {
        public UpdateRoleResponseProperty properties { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public string name { get; set; }
    }

    public class UpdateRoleResponseProperty
    {
        public string roleDefinitionId { get; set; }
        public string principalId { get; set; }
        public string principalType { get; set; }
        public string scope { get; set; }
        public string createdOn { get; set; }
        public string updatedOn { get; set; }
        public string createdBy { get; set; }
        public string updatedBy { get; set; }
    }


    public class AzureUpdateVMAssignmentReqModel
    {
        public UpdateVMProperties properties { get; set; }
    }

    public class UpdateVMProperties
    {
        public string principalId { get; set; }
        public string PrincipalType { get; set; }
        public string roleDefinitionId { get; set; }
        public string Scope { get; set; }
        public string Condition { get; set; }
        public string ConditionVersion { get; set; }
        public string Description { get; set; }
    }

    public class AzureADUserRespModel
    {
        [JsonProperty("@odata.nextLink")]
        public string NextLink { get; set; }
        public List<AzureADUser> value { get; set; }

    }
    public class AzureADUser
    {
        public string displayName { get; set; }
        public string givenName { get; set; }
        public string jobTitle { get; set; }
        public string mail { get; set; }
        public string mobilePhone { get; set; }
        public string surname { get; set; }
        public string userPrincipalName { get; set; }
        public string id { get; set; }

        public List<MemberOf> memberOf { get; set; }
    }

    public class MemberOf
    {
        public string id { get; set; }
        public string displayName { get; set; }

    }


    public class AzureAnalyticsReqModel
    {
        public string query { get; set; }
    }

    public class AzureAnalyticsApiResponse
    {
        public List<Table> Tables { get; set; }

       
    }

    public class Table
    {
        public string Name { get; set; }
        public List<Column> Columns { get; set; }
        public List<List<string>> Rows { get; set; }
    }

    public class Column
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class AzureAnalyticsApiRow
    {
        public string UserName { get; set; }
        public string ConnectionType { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string SessionHostName { get; set; }
        public string HostPoolName { get; set; }
        public TimeSpan Duration { get; set; }
    }
}