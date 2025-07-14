namespace GDT.CEC.Repository.Implementation
{
    public class UsersRepo : IUsersRepo
    {
        private readonly IMongoDBManager _mongoDBManager;
        private readonly IMongoCollection<User> _collection;
        private readonly IMongoCollection<AreaOfInterest> _collectionAreaOfInterest;
        public UsersRepo(IMongoDBManager dbContext, IMongoDBManager mongoDBManager)
        {
            _collection = dbContext.GetCollection<User>(AppConstants.COLLECTION_USER);
            _collectionAreaOfInterest = dbContext.GetCollection<AreaOfInterest>(AppConstants.COLLECTION_AREAOFINTEREST);
            _mongoDBManager = mongoDBManager;
        }
        public async Task CreateAsync(User model)
        {
            await _collection.InsertOneAsync(model);
        }
        public bool UserNameExists(string username)
        {
            return _collection.Find(m => m.Username == username).Any();
        }
        public bool UserEmailExists(string email)
        {
            return _collection.Find(m => m.Email == email).Any();
        }
        public string GetUniqueUserName(string username, string companyDomain)
        {
            string userEmail = username + "@" + companyDomain;
            int i = 1;
            while (_collection.Find(m => m.Username == userEmail).Any())
            {
                userEmail = username + i.ToString("00") + "@" + companyDomain;
                i++;
            }
            return userEmail;
        }

        public async Task<bool> UpdateUserActiveAsync(string userId, bool isActive, bool isSuperAdmin)
        {
            var user = GetUserByIdAsync(userId);
            if (user != null && user.Result.RoleId == AppConstants.ROLE_ID_ADMIN && isSuperAdmin==false)
            {
                return false;
            }
            var filter = Builders<User>.Filter.Eq(u => u.ID, userId);
            var update = Builders<User>.Update.Set(u => u.IsActive, isActive);

            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<int> UpdateUserRoleAsync(List<string> userIds, int roleId)
        {
            var filter = Builders<User>.Filter.In(u => u.ID, userIds);
            var update = Builders<User>.Update.Set(u => u.RoleId, roleId);

            var result = await _collection.UpdateManyAsync(filter, update);
            return (int)result.ModifiedCount;
        }

        public async Task<User> GetByOIdAsync(string id)
        {
            return await _collection.Find(p => p.AzureObjectID == id).FirstOrDefaultAsync();
        }
        public async Task<User> GetUserByIdAsync(string id)
        {
            return await _collection.Find(p => p.ID == id).FirstOrDefaultAsync();
        }
        public async Task UpdateAsync(string id, User docs)
        {
            docs.ID = id;
            var filter = Builders<User>.Filter.Eq(p => p.AzureObjectID, id);
            if (filter != null)
            {
                var update = Builders<User>.Update
                     .Set(x => x.FirstName, docs.FirstName)
                     .Set(x => x.LastName, docs.LastName)
                     .Set(x => x.PhoneNumber, docs.PhoneNumber)
                     .Set(x => x.Email, docs.Email)
                     .Set(x => x.AreaOfInt, docs.AreaOfInt)
                     .Set(x => x.IsActive, docs.IsActive)
                     .Set(x => x.Labs, docs.Labs);
                await _collection.UpdateOneAsync(filter, update);
            }
        }
        public async Task UpdateApprovedUserAsync(string id, User docs)
        {
            docs.ID = id;
            var filter = Builders<User>.Filter.Eq(p => p.ID, id);
            if (filter != null)
            {
                var update = Builders<User>.Update
                     .Set(x => x.AzureObjectID, docs.AzureObjectID)
                     .Set(x => x.Status, docs.Status)
                     .Set(x => x.Password, docs.Password)
                     .Set(x => x.Username, docs.Username)
                      .Set(x => x.IsActive, docs.IsActive)
                     .Set(x => x.DisplayName, docs.DisplayName);               
                await _collection.UpdateOneAsync(filter, update);
            }
        }
        public async Task ApproveRejectUser(string userid, string status, string rejectReason)
        {
            var filter = Builders<User>.Filter.Eq(p => p.ID, userid);
            if (filter != null)
            {
                var update = Builders<User>.Update
                     .Set(x => x.Status, status)
                      .Set(x => x.RejectReason, rejectReason);
                await _collection.UpdateOneAsync(filter, update);
            }
        }
        public async Task<List<User>> GetAllUsersAsync()
        {
            var userCollection = await _collection.Find(_ => true).ToListAsync();
            return userCollection;
        }
        public async Task<UserPagingModel> GetUsersAsync(int pagesize, int pageno, string sortDirection = "asc", string orderby = "firstname",Dictionary<string,string>filters=null)
        {
            var userCollection = await _collection.Find(_ => true).ToListAsync();
            if (filters != null)
            {
                foreach (KeyValuePair<string, string> filter in filters)
                {
                    if (filter.Key == "usertype")
                    {
                        userCollection = userCollection.Where(m => m.UserType == filter.Value).ToList();
                    }
                    else if (filter.Key == "roleid")
                    {
                        userCollection = userCollection.Where(m => m.RoleId == Convert.ToInt32(filter.Value)).ToList();
                    }
                    else if (filter.Key == "active")
                    {
                        userCollection = userCollection.Where(m => m.IsActive == (filter.Value == "true")).ToList();
                    }
                    else if (filter.Key == "username")
                    {
                        userCollection = userCollection.Where(m => m.Username != null && m.Username.ToLower().StartsWith(filter.Value.ToLower())).ToList();
                    }
                    else if (filter.Key == "email")
                    {
                        userCollection = userCollection.Where(m => m.Email != null && m.Email.ToLower().StartsWith(filter.Value.ToLower())).ToList();
                    }
                    else if(filter.Key=="status")
                    {
                        userCollection = userCollection.Where(m => m.Status != null && m.Status.ToLower() == filter.Value.ToLower()).ToList();
                    }
                    else if (filter.Key == "name")
                    {
                        userCollection = userCollection.Where(m => (m.FirstName != null && m.FirstName.ToLower().StartsWith(filter.Value.ToLower()))
                        || (m.LastName != null && m.LastName.ToLower().StartsWith(filter.Value.ToLower()))
                        || (m.Email != null && m.Email.ToLower().StartsWith(filter.Value.ToLower()))).ToList();
                    }
                }
            }
             int count = userCollection.Count;
            Func<User, object> sortExp = x => x.FirstName;
            switch (orderby)
            {
                case "username":
                    sortExp = x => x.Username;
                    break;
                case "lastname":
                    sortExp = x => x.LastName;
                    break;
                case "email":
                    sortExp = x => x.Email;
                    break;
                case "addedtime":
                    sortExp = x => x.added_time;
                    break;
            }

            var users = SortAndPage<User>(userCollection, sortExp, sortDirection , pageno, pagesize);
            UserPagingModel userPagingModel = new UserPagingModel { PageSize = pagesize, CurrentPage = pageno, TotalCount = count, Users = users.ToList() };

            return userPagingModel;
        }
        public async Task<List<AreaOfInterest>> GetAllAreaOfInterestAsync()
        {
            return await _collectionAreaOfInterest.Find(_ => true).ToListAsync();
        }

        public async Task<User> GetByCustomKeyAsync(string customKey)
        {
            var filter = Builders<User>.Filter.Eq("key", customKey); 
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }
        public async Task<User> GetDocumentByIdAsync(string id)
        {
            var filter = Builders<User>.Filter.Eq("_id", id); 
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var filter = Builders<User>.Filter.Eq(p => p.ID, id);
            await _collection.DeleteOneAsync(filter);
        }

        public IEnumerable<T> SortAndPage<T>(IEnumerable<T> source,
                                     Func<T, object> sortExpression,
                                     string sortDirection,
                                     int pageIndex,
                                     int pageSize)
        {

            var sortedData = sortDirection == "asc"
                ? source.OrderBy(sortExpression)
                : source.OrderByDescending(sortExpression);



            var pagedData = sortedData.Skip((pageIndex - 1) * pageSize)
                                       .Take(pageSize);


            return pagedData;
        }
    }
}
