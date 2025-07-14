using GDT.CEC.Repository.Models.Configurations;
using GDT.CEC.Repository.Models.HttpClient;
using GDT.CEC.Repository.Models.Response;
using Newtonsoft.Json;

namespace GDT.CEC.Repository.Helpers
{
    public class AzureAPIManager
    {
        private readonly AzureADConfig _azureADConfig;
        private readonly AzureAdGetUsersConfig _azureAdGetUsersConfig;
        private readonly AzureAdConfigCodinCity _azureAdConfigCodinCity;

        public AzureAPIManager(AzureADConfig azureADConfig, AzureAdGetUsersConfig azureAdGetUsersConfig,AzureAdConfigCodinCity azureAdConfigCodinCity)
        {
            _azureADConfig = azureADConfig;
            _azureAdGetUsersConfig = azureAdGetUsersConfig;
            _azureAdConfigCodinCity = azureAdConfigCodinCity;
        }
        public async Task<APIResponse<AzureADUserRespModel>> GetAllAzureADUsers1()
        {
            string authtoken = await GetAuthToken(_azureAdGetUsersConfig);
            if (!string.IsNullOrEmpty(authtoken))
            {
                var response = await GetAllAzureAdUsers2(authtoken);
                if (response.IsSuccessStatusCode)
                {
                    var resp = await response.Content.ReadAsStringAsync();
                    string strResp = resp.ToString();
                    AzureADUserRespModel responseObject = JsonConvert.DeserializeObject<AzureADUserRespModel>(strResp);

                    APIResponse<AzureADUserRespModel> apiresponse = new APIResponse<AzureADUserRespModel>
                    {
                        Message = "",
                        StatusCode = 1,
                        Data = responseObject
                    };
                    return apiresponse;
                }
                else
                {
                    var resp = await response.Content.ReadAsStringAsync();
                    string strResp = resp.ToString();
                    var responseObject = JsonConvert.DeserializeObject<AzureApiErrorMessage>(strResp);
                    return new APIResponse<AzureADUserRespModel> { Message = responseObject.error.message, StatusCode = 0 };
                }
            }
            else
            {
                return new APIResponse<AzureADUserRespModel> { Message = "Authorisation Failed", StatusCode = 0 };
            }
        }


        public async Task<APIResponse<AzureADUserRespModel>> GetAllAzureADUsers()
        {

            string authtoken = await GetAuthToken(_azureAdGetUsersConfig);
            if (!string.IsNullOrEmpty(authtoken))
            {
                string nextUrl = "first";
                AzureADUserRespModel respModel = new AzureADUserRespModel();
                respModel.value = new List<AzureADUser>();
                while (!string.IsNullOrEmpty(nextUrl))
                {
                    var response = await GetAllAzureAdUsers2(authtoken, nextUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        var resp = await response.Content.ReadAsStringAsync();
                        string strResp = resp.ToString();
                        AzureADUserRespModel responseObject = JsonConvert.DeserializeObject<AzureADUserRespModel>(strResp);
                        nextUrl = responseObject.NextLink;
                        respModel.value.AddRange(responseObject.value.ToList());
                    }
                    else
                    {
                        var resp = await response.Content.ReadAsStringAsync();
                        string strResp = resp.ToString();
                        var responseObject = JsonConvert.DeserializeObject<AzureApiErrorMessage>(strResp);
                        return new APIResponse<AzureADUserRespModel> { Message = responseObject.error.message, StatusCode = 0 };
                    }
                }



                APIResponse<AzureADUserRespModel> apiresponse = new APIResponse<AzureADUserRespModel>
                {
                    Message = "",
                    StatusCode = 1,
                    Data = respModel
                };
                return apiresponse;
            }
            else
            {
                return new APIResponse<AzureADUserRespModel> { Message = "Authorisation Failed", StatusCode = 0 };
            }
        }

        public async Task<APIResponse<AzureCreateUserRespModel>> CreateAzureADUser(User user)
        {
            string authtoken = await GetAuthToken(_azureAdConfigCodinCity);
            var response = await CreateUser(user, authtoken);


            if (response.IsSuccessStatusCode)
            {
                var resp = await response.Content.ReadAsStringAsync();
                string strResp = resp.ToString();
                var responseObject = JsonConvert.DeserializeObject<AzureCreateUserRespModel>(strResp);
                APIResponse<AzureCreateUserRespModel> apiresponse = new APIResponse<AzureCreateUserRespModel>
                {
                    Message = "Azure AD user created for " + user.Username,
                    StatusCode = 1,
                    Data = responseObject
                };
                return apiresponse;
            }
            else
            {
                var resp = await response.Content.ReadAsStringAsync();
                string strResp = resp.ToString();
                var responseObject = JsonConvert.DeserializeObject<AzureApiErrorMessage>(strResp);
                return new APIResponse<AzureCreateUserRespModel> { Message = responseObject.error.message, StatusCode = 0 };
            }

        }

        public async Task<APIResponse<AzureUpdateRoleAssignmentRespModel>> UpdateAzureADUser(User user)
        {
            string authtoken = await GetManagementAuthToken();
            var response = await UpdateRole(user, authtoken);
            string outMessage = "";
            if (response.IsSuccessStatusCode)
            {
                outMessage = "Role Assigned for user " + user.Username;
                var resp = await response.Content.ReadAsStringAsync();
                string strResp = resp.ToString();
                var responseObject = JsonConvert.DeserializeObject<AzureUpdateRoleAssignmentRespModel>(strResp);
                var vmResp = await UpdateVMAssignment(user, authtoken, responseObject);
                if (vmResp.IsSuccessStatusCode)
                {
                    APIResponse<AzureUpdateRoleAssignmentRespModel> apiresponse = new APIResponse<AzureUpdateRoleAssignmentRespModel>
                    {
                        Message = outMessage + ", VM assigned for " + user.Username,
                        StatusCode = 1,
                        Data = responseObject
                    };
                    return apiresponse;
                }
                else
                {
                    var respVm = await vmResp.Content.ReadAsStringAsync();
                    string strRespVm = respVm.ToString();
                    var responseObjectVm = JsonConvert.DeserializeObject<AzureApiErrorMessage>(strRespVm);
                    return new APIResponse<AzureUpdateRoleAssignmentRespModel> { Message = outMessage + responseObjectVm.error.message, StatusCode = 0 };
                }
            }
            else
            {
                var resp = await response.Content.ReadAsStringAsync();
                string strResp = resp.ToString();
                var responseObject = JsonConvert.DeserializeObject<AzureApiErrorMessage>(strResp);
                return new APIResponse<AzureUpdateRoleAssignmentRespModel> { Message = responseObject.error.message, StatusCode = 0 };
            }

        }


        public async Task<HttpResponseMessage> CreateUser(User user, string token)
        {
            AzureCreateUserReqModel postData = new AzureCreateUserReqModel();
            postData.userPrincipalName = user.Username;
            postData.displayName = user.DisplayName;
            postData.mailNickname = user.FirstName;
            postData.accountEnabled = true;
            postData.passwordProfile = new PasswordProfile { forceChangePasswordNextSignIn = true, password = user.Password };

            HttpClientRequestModel httpClientRequestModel = new HttpClientRequestModel();
            httpClientRequestModel.BaseUrl = _azureADConfig.AzureADGraphBaseUrl;
            httpClientRequestModel.MethodNameOrUrl = _azureADConfig.AzureADUserEndpoint;
            httpClientRequestModel.AuthToken = token;

            var response = await HttpClientMethods.PostDataAsync(httpClientRequestModel, postData);
            return response;

        }

        public async Task<HttpResponseMessage> UpdateRole(User user, string token)
        {
            AzureUpdateRoleAssignmentReqModel postData = new AzureUpdateRoleAssignmentReqModel();
            postData.properties = new UpdateRoleProperties();
            postData.properties.principalId = user.AzureObjectID;
            postData.properties.roleDefinitionId = _azureADConfig.AzureADRoleDefinitionID;


            HttpClientRequestModel httpClientRequestModel = new HttpClientRequestModel();
            httpClientRequestModel.BaseUrl = _azureADConfig.AzureADManagementBaseUrl;
            httpClientRequestModel.MethodNameOrUrl = _azureADConfig.AzureADManagementEndpoint.Replace("{role_assignment_id}", Guid.NewGuid().ToString());
            httpClientRequestModel.AuthToken = token;

            var response = await HttpClientMethods.PutDataAsync(httpClientRequestModel, postData);
            return response;
        }


        private async Task<HttpResponseMessage> GetAllAzureAdUsers(string token)
        {

            HttpClientRequestModel httpClientRequestModel = new HttpClientRequestModel();
            httpClientRequestModel.BaseUrl = _azureADConfig.AzureADGraphBaseUrl;
            httpClientRequestModel.MethodNameOrUrl = _azureADConfig.AzureADUserEndpoint + "?$top=999&$expand=memberOf($select=id,displayName)";
            httpClientRequestModel.AuthToken = token;

            var response = await HttpClientMethods.GetDataAsync(httpClientRequestModel);
            return response;

        }
        private async Task<HttpResponseMessage> GetAllAzureAdUsers2(string token, string nextUrl = "")
        {

            HttpClientRequestModel httpClientRequestModel = new HttpClientRequestModel();
            httpClientRequestModel.BaseUrl = _azureADConfig.AzureADGraphBaseUrl;
            if (nextUrl == "" || nextUrl == "first")
            {
                httpClientRequestModel.MethodNameOrUrl = _azureADConfig.AzureADUserEndpoint + "?$top=999&$all=true&$count=true";
            }
            else
            {
                httpClientRequestModel.MethodNameOrUrl = nextUrl.Replace(_azureADConfig.AzureADGraphBaseUrl, "");
            }
            httpClientRequestModel.AuthToken = token;

            var response = await HttpClientMethods.GetDataAsync(httpClientRequestModel);
            return response;

        }

        public async Task<HttpResponseMessage> UpdateVMAssignment(User user, string token, AzureUpdateRoleAssignmentRespModel respModel)
        {
            AzureUpdateVMAssignmentReqModel postData = new AzureUpdateVMAssignmentReqModel();
            postData.properties = new UpdateVMProperties();
            postData.properties.principalId = user.AzureObjectID;
            postData.properties.roleDefinitionId = _azureADConfig.AzureADVMRoleDefinitionId;
            postData.properties.PrincipalType = respModel.properties.principalType;
            postData.properties.Scope = _azureADConfig.AzureAdVMScope;



            HttpClientRequestModel httpClientRequestModel = new HttpClientRequestModel();
            httpClientRequestModel.BaseUrl = _azureADConfig.AzureADManagementBaseUrl;
            httpClientRequestModel.MethodNameOrUrl = _azureADConfig.AzureADManagementVMroleEndpoint.Replace("{roleAssignmentId}", Guid.NewGuid().ToString());
            httpClientRequestModel.AuthToken = token;

            var response = await HttpClientMethods.PutDataAsync(httpClientRequestModel, postData);
            return response;
        }
        public async Task<string> GetAuthToken()
        {
            HttpClientRequestModel httpClientRequestModel = new HttpClientRequestModel();
            httpClientRequestModel.BaseUrl = _azureADConfig.AzureADOuthTokenBaseUrl;
            httpClientRequestModel.MethodNameOrUrl = _azureADConfig.AzureADOuthTokenEndpoint;
            httpClientRequestModel.AuthToken = "";
            Dictionary<string, string> postData = new Dictionary<string, string>();
            postData.Add("client_id", _azureADConfig.AzureADClientID);
            postData.Add("client_secret", _azureADConfig.AzureADClientSecret);
            postData.Add("grant_type", _azureADConfig.AzureADGrantType);
            postData.Add("resource", _azureADConfig.AzureADResource);

            var response = await HttpClientMethods.PostDataAsync(httpClientRequestModel, postData);
            if (response.IsSuccessStatusCode)
            {
                string resp = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(resp);
                return responseObject["access_token"];
            }
            return "";
        }
        public async Task<string> GetAuthToken(AzureAdGetUsersConfig config)
        {
            HttpClientRequestModel httpClientRequestModel = new HttpClientRequestModel();
            httpClientRequestModel.BaseUrl = config.AzureADOuthTokenBaseUrl;
            httpClientRequestModel.MethodNameOrUrl = _azureADConfig.AzureADOuthTokenEndpoint;
            httpClientRequestModel.AuthToken = "";
            Dictionary<string, string> postData = new Dictionary<string, string>();
            postData.Add("client_id", config.AzureADClientID);
            postData.Add("client_secret", config.AzureADClientSecret);
            postData.Add("grant_type", _azureADConfig.AzureADGrantType);
            postData.Add("resource", _azureADConfig.AzureADResource);

            var response = await HttpClientMethods.PostDataAsync(httpClientRequestModel, postData);
            if (response.IsSuccessStatusCode)
            {
                string resp = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(resp);
                return responseObject["access_token"];
            }
            return "";
        }
        public async Task<string> GetAuthToken(AzureAdConfigCodinCity config)
        {
            HttpClientRequestModel httpClientRequestModel = new HttpClientRequestModel();
            httpClientRequestModel.BaseUrl = config.AzureADOuthTokenBaseUrl;
            httpClientRequestModel.MethodNameOrUrl = _azureADConfig.AzureADOuthTokenEndpoint;
            httpClientRequestModel.AuthToken = "";
            Dictionary<string, string> postData = new Dictionary<string, string>();
            postData.Add("client_id", config.AzureADClientID);
            postData.Add("client_secret", config.AzureADClientSecret);
            postData.Add("grant_type", _azureADConfig.AzureADGrantType);
            postData.Add("resource", _azureADConfig.AzureADResource);

            var response = await HttpClientMethods.PostDataAsync(httpClientRequestModel, postData);
            if (response.IsSuccessStatusCode)
            {
                string resp = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(resp);
                return responseObject["access_token"];
            }
            return "";
        }
        public async Task<string> GetManagementAuthToken()
        {
            HttpClientRequestModel httpClientRequestModel = new HttpClientRequestModel();
            httpClientRequestModel.BaseUrl = _azureADConfig.AzureADOuthTokenBaseUrl;
            httpClientRequestModel.MethodNameOrUrl = _azureADConfig.AzureADManagementTokenEndpoint;
            httpClientRequestModel.AuthToken = "";
            Dictionary<string, string> postData = new Dictionary<string, string>();
            postData.Add("client_id", _azureADConfig.AzureADClientID);
            postData.Add("client_secret", _azureADConfig.AzureADClientSecret);
            postData.Add("grant_type", _azureADConfig.AzureADGrantType);
            postData.Add("scope", _azureADConfig.AzureADScope);

            var response = await HttpClientMethods.PostDataAsync(httpClientRequestModel, postData);
            if (response.IsSuccessStatusCode)
            {
                string resp = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(resp);
                return responseObject["access_token"];
            }
            return "";
        }

        private async Task<string> GetAnalyticsAuthToken()
        {
            HttpClientRequestModel httpClientRequestModel = new HttpClientRequestModel();
            httpClientRequestModel.BaseUrl = _azureADConfig.AzureADOuthTokenBaseUrl;
            httpClientRequestModel.MethodNameOrUrl = _azureADConfig.AzureADManagementTokenEndpoint;
            httpClientRequestModel.AuthToken = "";
            Dictionary<string, string> postData = new Dictionary<string, string>();
            postData.Add("client_id", _azureADConfig.AzureADClientID);
            postData.Add("client_secret", _azureADConfig.AzureADClientSecret);
            postData.Add("grant_type", _azureADConfig.AzureADGrantType);
            postData.Add("scope", _azureADConfig.AzureADAnalyticsScope);

            var response = await HttpClientMethods.PostDataAsync(httpClientRequestModel, postData);
            if (response.IsSuccessStatusCode)
            {
                string resp = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(resp);
                return responseObject["access_token"];
            }
            return "";
        }


        public async Task<APIResponse<List<AzureAnalyticsApiRow>>> GetAnalyticsLogs(DateTime latestDatetime)
        {
            string token = await GetAnalyticsAuthToken();
            if (!string.IsNullOrEmpty(token))
            {
                var response = await GetLabAccessLogs(latestDatetime, token);
                if (response.IsSuccessStatusCode)
                {
                    var resp = await response.Content.ReadAsStringAsync();
                    string strResp = resp.ToString();
                    AzureAnalyticsApiResponse analyticsResp = JsonConvert.DeserializeObject<AzureAnalyticsApiResponse>(strResp);

                    List<AzureAnalyticsApiRow> rows = new List<AzureAnalyticsApiRow>();

                    foreach (var table in analyticsResp.Tables)
                    {
                        foreach (var row in table.Rows)
                        {
                            AzureAnalyticsApiRow azureApiRow = new AzureAnalyticsApiRow
                            {
                                UserName = row[0],
                                ConnectionType = row[1],
                                StartTime = DateTime.Parse(row[2]),
                                EndTime = DateTime.Parse(row[3]),
                                SessionHostName = row[4],
                                HostPoolName = row[5],
                                Duration = TimeSpan.Parse(row[6])
                            };

                            rows.Add(azureApiRow);
                        }
                    }
                    APIResponse<List<AzureAnalyticsApiRow>> apiresponse = new APIResponse<List<AzureAnalyticsApiRow>>
                    {
                        Message = "",
                        StatusCode = 1,
                        Data = rows
                    };
                    return apiresponse;
                }
                else
                {
                    var resp = await response.Content.ReadAsStringAsync();
                    string strResp = resp.ToString();
                    var responseObject = JsonConvert.DeserializeObject<AzureApiErrorMessage>(strResp);
                    return new APIResponse<List<AzureAnalyticsApiRow>> { Message = responseObject.error.message, StatusCode = 0 };
                }
            }
            else
            {
                return new APIResponse<List<AzureAnalyticsApiRow>> { Message = "Authorisation Failed", StatusCode = 0 };
            }
        }

        public async Task<HttpResponseMessage> GetLabAccessLogs(DateTime latestDatetime, string token)
        {
            string query = "WVDConnections | where State == \"Connected\" | project CorrelationId, UserName, ConnectionType, StartTime = TimeGenerated, SessionHostName, HostPoolName = split(_ResourceId, \"/\")[-1] | join (WVDConnections  | where TimeGenerated > datetime(" + latestDatetime.ToString("yyyy-MM-dd HH:mm:ss.fffffff") + ")  | where State == \"Completed\"    | project EndTime = TimeGenerated, CorrelationId)     on CorrelationId | extend SessionDuration = EndTime - StartTime | summarize Duration = sum(SessionDuration) by UserName, ConnectionType, StartTime, EndTime, SessionHostName, tostring(HostPoolName) | sort by Duration desc";
            AzureAnalyticsReqModel postData = new AzureAnalyticsReqModel
            {
                query = query
            };
            HttpClientRequestModel httpClientRequestModel = new HttpClientRequestModel();
            httpClientRequestModel.BaseUrl = _azureADConfig.AzureAnalyticsBaseUrl;
            httpClientRequestModel.MethodNameOrUrl = _azureADConfig.AzureAnalyticsEndpoint;
            httpClientRequestModel.AuthToken = token;

            var response = await HttpClientMethods.PostDataAsync(httpClientRequestModel, postData);
            return response;
        }
    }
}
