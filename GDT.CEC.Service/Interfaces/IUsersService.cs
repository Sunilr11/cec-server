

using GDT.CEC.Repository.Models.HttpClient;
using GDT.CEC.Repository.Models.Response;
using GDT.CEC.Service.DTOs;

namespace GDT.CEC.Service.Interfaces
{
    public interface IUsersService
    {
        public Task<bool> CreateAsync(User docs, List<string> errorsMessages);

        public Task<bool> DeleteAsync(string id, List<string> errorsMessages);
       
        public bool UserNameExist(string username);
        public bool UserEmailExist(string email);
        public Task<ReturnResponse<Response>> VerifyAndSaveUserAsync(string key, string submittedOtp);
        public Task<List<AreaOfInterest>> GetAllAreaOfInterest();
        public Task<UserRegisterConfig> GetUserRegisterConfiguration();
        public string GenerateOTPSendEmail(string email);
        public ReturnResponse<Response> EmailOTPVerification(string key, string submittedOtp);
        bool IsUserEmailVerified(string email, string key);
        Task<ReturnResponse<Response>> SaveUserAsync(TempUserRegister tempUser);
        void RemoveMemoryCache(string key);
        Task<User> GetUserByOIDAsync(string id);
        Task<bool> UpdateAsync(User user, string id);
        Task<ReturnResponse<Response>> ApproveRejectUser(string userid, string status, string rejectReason);
        Task<UserPagingModel> GetUsersAsync(int pagesize, int pageno, string sortDirection = "asc", string orderby = "firstname", Dictionary<string, string> filters = null);
        Task<ReturnResponse<List<AzureADUserDTO>>> GetAllAdUsers();
        Task<ReturnResponse<string>> SaveADUserAsync(List<AzureADUserDTO> tempUserLst);
        UserImportStatus GetUserImportStatus(string userImportKey);
        Task<int> UpdateUserRoleAsync(List<string> userId, int roleId);
        Task<bool> UpdateUserActiveAsync(string userId, bool isActive, bool isSuperAdmin);
        Task<ReturnResponse<UserStatics>> GetUserStatisticsAsync();
    }
}
