using GDT.CEC.Repository;
using GDT.CEC.Service.DTOs;
using GDT.CEC.Repository.Helpers;
using GDT.CEC.Repository.Models.HttpClient;
using GDT.CEC.Repository.Models.Response;
using GDT.CEC.Repository.Models.Users;
using Microsoft.Extensions.Caching.Memory;
using GDT.CEC.Repository.Models.AzureLabs;

namespace GDT.CEC.Service.Implementation
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepo _usersRepo;
        private readonly IOptions<CacheConfig> cacheConfig;
        private readonly CacheConfig _cacheConfig;
        private readonly IDistributedCache distributedCache;
        private readonly IMemoryCache _memoryCache;
        private readonly IOptions<MailConfig> _mailConfig;
        private readonly IOptions<AzureADConfig> _azureADConfig;
        private readonly IOptions<AzureAdGetUsersConfig> _azureADGetUsersConfig;
        private readonly IOptions<AzureAdConfigCodinCity> _azureAdConfigCodinCity;
        private readonly IAzureLabService _azureLabService;
        private readonly IOptions<AppConfig> _appConfig;

        public UsersService(IUsersRepo usersRepo, IOptions<CacheConfig> cacheConfig, IDistributedCache distributedCache, IMemoryCache memoryCache,
            IOptions<MailConfig> mailConfig, IOptions<AzureADConfig> azureADConfig, IAzureLabService azureLabService, IOptions<AppConfig> appConfig, IOptions<AzureAdGetUsersConfig> azureADGetUsersConfig,
            IOptions<AzureAdConfigCodinCity> azureAdConfigCodinCity
            )
        {
            if (usersRepo == null)
            {
                new ArgumentNullException("userRepo cannot be null");
            }
            _usersRepo = usersRepo;
            this.cacheConfig = cacheConfig;
            _cacheConfig = cacheConfig.Value;
            this.distributedCache = distributedCache;
            _memoryCache = memoryCache;
            _mailConfig = mailConfig;
            _azureADConfig = azureADConfig;
            _azureLabService = azureLabService;
            _appConfig = appConfig;
            _azureADGetUsersConfig = azureADGetUsersConfig;
            _azureAdConfigCodinCity = azureAdConfigCodinCity;
        }

        public async Task<int> UpdateUserRoleAsync(List<string> userId, int roleId)
        {
            return await _usersRepo.UpdateUserRoleAsync(userId, roleId);
        }

        public async Task<bool> UpdateUserActiveAsync(string userId, bool isActive, bool isSuperAdmin)
        {
            return await _usersRepo.UpdateUserActiveAsync(userId, isActive,isSuperAdmin);
        }

        public async Task<bool> CreateAsync(User register, List<string> errorsMessages)
        {
            try
            {
                bool result = false;
                register.added_time = DateTime.UtcNow;
                await _usersRepo.CreateAsync(register);

                return result;
            }
            catch
            {
                throw;
            }
        }
        public bool UserNameExist(string username)
        {
            try
            {
                return _usersRepo.UserNameExists(username);
            }
            catch
            {
                throw;
            }
        }
        public bool UserEmailExist(string email)
        {
            try
            {
                return _usersRepo.UserEmailExists(email);
            }
            catch
            {
                throw;
            }
        }
        public bool IsUserEmailVerified(string email, string key)
        {
            try
            {
                bool dataExist = _memoryCache.TryGetValue(key, out UserEmailVerification tempUser);
                if (dataExist)
                {
                    return tempUser.IsVerified;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }

        public void RemoveMemoryCache(string key)
        {
            _memoryCache.Remove(key);
        }
        public string GenerateOTPSendEmail(string email)
        {
            string templatePath = "";
            try
            {
                string otp = OTPGenerator.GenerateOTP();
                string otpHash = OTPGenerator.HashOTP(otp);
                string otpKey = Guid.NewGuid().ToString();


                templatePath = Path.Combine(Directory.GetCurrentDirectory(), AppConstants.EMAILTEMPLATE_OTP);
                string template = System.IO.File.ReadAllText(templatePath);


                string body = template.Replace("{otp}", otp).Replace("{imagepath}", _mailConfig.Value.EmailTemplateImagePath).Replace("{company_name}", _appConfig.Value.CompanyName);


                MailManager mailManager = new MailManager(_mailConfig);
                mailManager.SendMail(email, "OTP verification", body);

                UserEmailVerification userEmailVerification = new UserEmailVerification { Email = email, Key = otpKey, OtpHash = otpHash, IsVerified = false };
                TimeSpan cacheDuration = TimeSpan.FromSeconds(_cacheConfig.OTP_ExpiryTime);
                _memoryCache.Set(otpKey, userEmailVerification, new MemoryCacheEntryOptions().SetAbsoluteExpiration(DateTimeOffset.UtcNow.Add(cacheDuration)));
                return otpKey;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ReturnResponse<Response> EmailOTPVerification(string key, string submittedOtp)
        {
            bool dataExist = _memoryCache.TryGetValue(key, out UserEmailVerification emailVerification);
            string submittedOtpHash = OTPGenerator.HashOTP(submittedOtp);
            if (dataExist)
            {
                if (emailVerification.OtpHash == submittedOtpHash)
                {
                    emailVerification.IsVerified = true;
                    _memoryCache.Remove(key);
                    TimeSpan cacheDuration = TimeSpan.FromSeconds(_cacheConfig.OTP_ExpiryTime);
                    _memoryCache.Set(key, emailVerification, new MemoryCacheEntryOptions().SetAbsoluteExpiration(DateTimeOffset.UtcNow.Add(cacheDuration)));
                    return new ReturnResponse<Response> { Message = AppConstants.USER_OTP_SUCCESS, StatusCode = 1 };
                }
            }

            return new ReturnResponse<Response> { Message = AppConstants.USER_OTP_FAILED, StatusCode = 0 };
        }

        public async Task<ReturnResponse<Response>> SaveUserAsync(TempUserRegister tempUser)
        {
            try
            {
                tempUser.User.added_time = DateTime.UtcNow;
                tempUser.User.Status = "Pending";
                tempUser.User.RoleId = AppConstants.ROLE_ID_USER;
                tempUser.User.IsActive = false;
                await _usersRepo.CreateAsync(tempUser.User);
                _memoryCache.Remove(tempUser.RegistrationKey);
                return new ReturnResponse<Response> { Message = AppConstants.USERS_ADDED_SUCCESSFULLY, StatusCode = 1, LogMessage = "" };
            }
            catch
            {
                throw;
            }
        }
        public async Task<ReturnResponse<string>> SaveADUserAsync(List<AzureADUserDTO> tempUserLst)
        {
            try
            {
                string userImportKey = Guid.NewGuid().ToString();
                TimeSpan cacheDuration = TimeSpan.FromMinutes(30);
                UserImportStatus userImportStatus = new UserImportStatus();
                userImportStatus.TotalCount = tempUserLst.Count;
                userImportStatus.SucessfulCount = 0;
                userImportStatus.ImportStatusKey=userImportKey;
                userImportStatus.ProcessedCount = 0;

                _memoryCache.Set(userImportKey, userImportStatus, new MemoryCacheEntryOptions().SetAbsoluteExpiration(DateTimeOffset.UtcNow.Add(cacheDuration)));

                Thread thread = new Thread(()=>UserImportThread(userImportKey,tempUserLst,userImportStatus));
                thread.Start();


                return new ReturnResponse<string> { Message = AppConstants.IMPORT_USER_STARTED, StatusCode = 1, LogMessage = "", Data = userImportKey };
            }
            catch
            {
                throw;
            }
        }

        private void UserImportThread(string userImportKey ,List<AzureADUserDTO> userDTOs, UserImportStatus userImportStatus)
        {
            TimeSpan cacheDuration = TimeSpan.FromMinutes(30);
            MailManager mailManager = new MailManager(_mailConfig);
            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), AppConstants.EMAILTEMPLATE_CRED);
            string template = System.IO.File.ReadAllText(templatePath);
            foreach (var tempUser in userDTOs)
            {
                try
                {
                    User user = new User
                    {
                        added_time = DateTime.UtcNow,
                        Status = "Pending",
                        Email = tempUser.Mail ?? tempUser.UserPrincipalName,
                        AzureObjectID = tempUser.ID,
                        DisplayName = tempUser.DisplayName,
                        FirstName = tempUser.GivenName,
                        LastName = tempUser.Surname,
                        PhoneNumber = tempUser.MobilePhone,
                        UserType = "Internal",
                        RoleId = AppConstants.ROLE_ID_USER,
                        Username = tempUser.UserPrincipalName,
                        Password = OTPGenerator.GeneratePassword(),
                        IsActive=false,
                        Labs = tempUser.Labs != null ? tempUser.Labs.Select(n => new AzureLab { AzureLabID = n.ID, Name = n.Name }).ToList() : null
                    };
                    _usersRepo.CreateAsync(user);

                    userImportStatus.SucessfulCount++;
                    userImportStatus.ProcessedCount++;
                }
                catch (Exception ex)
                {
                    userImportStatus.ProcessedCount++;
                    userImportStatus.Errors.Add("Adding user failed for " + tempUser.Mail + "[" + ex.Message + "]");
                }

                _memoryCache.Set(userImportKey, userImportStatus, new MemoryCacheEntryOptions().SetAbsoluteExpiration(DateTimeOffset.UtcNow.Add(cacheDuration)));

            }
        }

        public UserImportStatus GetUserImportStatus(string userImportKey)
        {
            bool dataExist = _memoryCache.TryGetValue(userImportKey, out UserImportStatus userImportStatus);
            return userImportStatus;
        }

        public async Task<ReturnResponse<Response>> VerifyAndSaveUserAsync(string key, string submittedOtp)
        {
            try
            {
                bool dataExist = _memoryCache.TryGetValue(key, out TempUserRegister tempUser);
                string submittedOtpHash = OTPGenerator.HashOTP(submittedOtp);
                if (dataExist)
                {
                    if (tempUser.OTPHash == submittedOtpHash)
                    {
                        tempUser.User.added_time = DateTime.UtcNow;
                        tempUser.User.DisplayName = tempUser.User.FirstName + " " + tempUser.User.LastName;
                        tempUser.User.Username = tempUser.User.FirstName + "." + tempUser.User.LastName + "@" + _azureADConfig.Value.CompanyDomain;
                        tempUser.User.Password = OTPGenerator.GeneratePassword();
                        tempUser.User.Status = "Pending";
                        tempUser.User.RoleId = AppConstants.ROLE_ID_USER;
                        AzureAPIManager azureAPIManager = new AzureAPIManager(_azureADConfig.Value,_azureADGetUsersConfig.Value,_azureAdConfigCodinCity.Value);
                        APIResponse<AzureCreateUserRespModel> aPIResponse = await azureAPIManager.CreateAzureADUser(tempUser.User);
                        if (aPIResponse.StatusCode == 1)
                        {
                            await _usersRepo.CreateAsync(tempUser.User);
                            _memoryCache.Remove(key);
                            return new ReturnResponse<Response> { Message = AppConstants.USERS_ADDED_SUCCESSFULLY, StatusCode = 1 };
                        }
                        else
                        {
                            return new ReturnResponse<Response> { Message = aPIResponse.Message, StatusCode = 0 };
                        }

                    }
                    else
                    {
                        return new ReturnResponse<Response> { Message = AppConstants.USER_OTP_FAILED, StatusCode = 0 };
                    }
                }
                return new ReturnResponse<Response> { Message = AppConstants.USER_OTP_FAILED, StatusCode = 0 };
            }
            catch
            {
                throw;
            }
        }


        public async Task<List<AreaOfInterest>> GetAllAreaOfInterest()
        {
            return await _usersRepo.GetAllAreaOfInterestAsync();
        }

        public async Task<UserRegisterConfig> GetUserRegisterConfiguration()
        {
            var areas = await _usersRepo.GetAllAreaOfInterestAsync();
            var labs = await _azureLabService.GetLabs(true, false);
            return new UserRegisterConfig { areaOfInterests = areas, azureLabs = labs };
        }

        public async Task<bool> DeleteAsync(string id, List<string> errorsMessages)
        {
            try
            {
                bool result = false;
                var IsDocsExists = await _usersRepo.GetDocumentByIdAsync(id);
                if (IsDocsExists != null)
                {
                    await _usersRepo.DeleteAsync(id);


                    var cachedUserList = await distributedCache.GetAsync(AppConstants.ALL_USER_DOC_KEYNAME);
                    if (cachedUserList != null)
                    {
                        await distributedCache.RemoveAsync(AppConstants.ALL_USER_DOC_KEYNAME);
                    }
                    result = true;
                }
                return result;
            }
            catch
            {
                throw;
            }
        }


        public async Task<bool> UpdateAsync(User user, string id)
        {
            try
            {
                await _usersRepo.UpdateAsync(id, user);
                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<User> GetUserByOIDAsync(string id)
        {
            try
            {
                User result = new();
                result = await _usersRepo.GetByOIdAsync(id);
                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<UserPagingModel> GetUsersAsync(int pagesize, int pageno, string sortDirection = "asc", string orderby = "firstname", Dictionary<string, string> filters = null)
        {
            return await _usersRepo.GetUsersAsync(pagesize, pageno, sortDirection, orderby,filters);
        }

        public async Task<ReturnResponse<UserStatics>> GetUserStatisticsAsync()
        {
            try
            {
                var users = await _usersRepo.GetAllUsersAsync();
                UserStatics userStatics = new UserStatics
                {
                    Total = users.Count,
                    Active = users.Where(m => m.IsActive == true && m.Status=="Approved").Count(),
                    Disabled = users.Where(m => m.IsActive == false && m.Status == "Approved").Count(),
                    Rejected= users.Where(m =>  m.Status == "Rejected").Count(),
                    Pending= users.Where(m => m.Status == "Pending").Count()
                };
                return new ReturnResponse<UserStatics> { Message = "", StatusCode = 1, LogMessage = "", Data = userStatics };
            }
            catch
            {
                throw;
            }
        }

        public async Task<ReturnResponse<Response>> ApproveRejectUser(string userid, string status,string rejectReason)
        {
            User tempUser = await _usersRepo.GetUserByIdAsync(userid);
            if (tempUser != null)
            {
                if (status.ToLower() == "approved")
                {
                    try
                    {
                        if (string.IsNullOrEmpty(tempUser.FirstName))
                        {
                            tempUser.FirstName = tempUser.DisplayName.Split(" ")[0];
                        }
                        if (string.IsNullOrEmpty(tempUser.LastName))
                        {
                            tempUser.LastName = tempUser.DisplayName.Split(" ")[1];
                        }
                    }
                    catch { }
                    tempUser.DisplayName = tempUser.FirstName + " " + tempUser.LastName;
                    string uname = tempUser.FirstName.Substring(0, 1) + tempUser.LastName.Substring(0, 1);
                    tempUser.Username = _usersRepo.GetUniqueUserName(uname, _azureADConfig.Value.CompanyDomain);
                    tempUser.Password = OTPGenerator.GeneratePassword();
                    tempUser.Status = "Approved";
                    AzureAPIManager azureAPIManager = new AzureAPIManager(_azureADConfig.Value, _azureADGetUsersConfig.Value,_azureAdConfigCodinCity.Value);
                    APIResponse<AzureCreateUserRespModel> aPIResponse = await azureAPIManager.CreateAzureADUser(tempUser);
                    if (aPIResponse.StatusCode == 1)
                    {
                        tempUser.AzureObjectID = aPIResponse.Data.id;
                        tempUser.IsActive = true;
                            await _usersRepo.UpdateApprovedUserAsync(userid, tempUser);

                        MailManager mailManager = new MailManager(_mailConfig);
                        string templatePath = Path.Combine(Directory.GetCurrentDirectory(), AppConstants.EMAILTEMPLATE_CRED);
                        string template = System.IO.File.ReadAllText(templatePath);
                        string body = template.Replace("{firstname}", tempUser.FirstName).Replace("{username}", tempUser.Username).Replace("{password}", tempUser.Password).Replace("{imagepath}", _mailConfig.Value.EmailTemplateImagePath).Replace("{company_name}", _appConfig.Value.CompanyName);
                        mailManager.SendMail(tempUser.Email, AppConstants.EMAIL_CRED_SUBJECT, body);

                        return new ReturnResponse<Response> { Message = AppConstants.USER_REGISTRATION_APPROVED, StatusCode = 1, LogMessage = aPIResponse.Message  };
                        

                    }
                    else
                    {
                        return new ReturnResponse<Response> { Message = aPIResponse.Message, StatusCode = 0 };
                    }
                }
                else
                {
                    await _usersRepo.ApproveRejectUser(userid, status, rejectReason);

                    MailManager mailManager = new MailManager(_mailConfig);
                    string templatePath = Path.Combine(Directory.GetCurrentDirectory(), AppConstants.EMAILTEMPLATE_REJECT);
                    string template = System.IO.File.ReadAllText(templatePath);
                    string body = template.Replace("{firstname}", tempUser.FirstName).Replace("{rejectReason}", rejectReason).Replace("{imagepath}", _mailConfig.Value.EmailTemplateImagePath).Replace("{company_name}", _appConfig.Value.CompanyName);
                    mailManager.SendMail(tempUser.Email, AppConstants.EMAIL_REJECT_SUBJECT, body);

                    return new ReturnResponse<Response> { Message = AppConstants.USER_REGISTRATION_REJECTED, StatusCode = 1 };
                }
            }


            return new ReturnResponse<Response> { Message = "", StatusCode = 0 };
        }


        public async Task<ReturnResponse<List<AzureADUserDTO>>> GetAllAdUsers()
        {
            AzureAPIManager azureAPIManager = new AzureAPIManager(_azureADConfig.Value, _azureADGetUsersConfig.Value,_azureAdConfigCodinCity.Value);
            var response =await  azureAPIManager.GetAllAzureADUsers();
            if (response.StatusCode == 1)
            {
                var adUsers = response.Data.value.Select(s => new AzureADUserDTO
                {
                    DisplayName = s.displayName,
                    GivenName = s.givenName,
                    ID = s.id,
                    Mail = s.mail,
                    UserPrincipalName = s.userPrincipalName,
                    JobTitle = s.jobTitle,
                    MobilePhone = s.mobilePhone,
                    Surname = s.surname
                }).ToList();

                var cecUsers = await _usersRepo.GetAllUsersAsync();
                var existingUsers=cecUsers.Select(m=>m.Username).ToList();

                adUsers.RemoveAll(m => existingUsers.Contains(m.UserPrincipalName));


                return new ReturnResponse<List<AzureADUserDTO>> { Message = response.Message, StatusCode = response.StatusCode, Data = adUsers };
            }
            else
            {
                return new ReturnResponse<List<AzureADUserDTO>> { Message = response.Message, StatusCode = response.StatusCode };
            }

        }
    }
}