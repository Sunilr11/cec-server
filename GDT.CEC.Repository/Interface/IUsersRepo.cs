

using MongoDB.Bson;

namespace GDT.CEC.Repository.Interface
{
    public interface IUsersRepo
    {
        public Task CreateAsync(User docs);
        public Task<User> GetByOIdAsync(string id);
        public Task UpdateAsync(string id, User docs);
        public Task DeleteAsync(string id);
        public Task<User> GetByCustomKeyAsync(string customKey);
        public Task<User> GetDocumentByIdAsync(string id);
        public bool UserNameExists(string username);
        public bool UserEmailExists(string email);
        public Task<List<AreaOfInterest>> GetAllAreaOfInterestAsync();
        string GetUniqueUserName(string username, string companyDomain);
        Task ApproveRejectUser(string userid, string status, string rejectReason);
        Task<User> GetUserByIdAsync(string id);
        Task UpdateApprovedUserAsync(string id, User docs);
        Task<UserPagingModel> GetUsersAsync(int pagesize, int pageno, string sortDirection = "asc", string orderby = "firstname", Dictionary<string, string> filters = null);
        Task<List<User>> GetAllUsersAsync();
        Task<int> UpdateUserRoleAsync(List<string> userId, int roleId);
        Task<bool> UpdateUserActiveAsync(string userId, bool isActive, bool isSuperAdmin);
    }
}
