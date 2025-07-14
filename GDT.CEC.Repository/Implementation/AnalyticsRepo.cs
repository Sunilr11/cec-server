using GDT.CEC.Repository.Models.AzureLabs;
using MongoDB.Bson;

namespace GDT.CEC.Repository.Implementation
{
    public class AnalyticsRepo : IAnalyticsRepo
    {
        private readonly IMongoDBManager _mongoDBManager;
        private readonly IMongoCollection<LabAccessLog> _collectionLabAccessLog;
        private readonly IMongoCollection<AzureLab> _collectionLabs;
        public AnalyticsRepo(IMongoDBManager dbContext, IMongoDBManager mongoDBManager)
        {
            _collectionLabAccessLog = dbContext.GetCollection<LabAccessLog>(AppConstants.COLLECTION_LABACCESSLOG);
            _collectionLabs = dbContext.GetCollection<AzureLab>(AppConstants.COLLECTION_LABS);
            _mongoDBManager = mongoDBManager;
        }
        public async Task CreateAsync(LabAccessLog model)
        {
            await _collectionLabAccessLog.InsertOneAsync(model);
        }
        public async Task CreateAsync(List<LabAccessLog> model)
        {
            await _collectionLabAccessLog.InsertManyAsync(model);
        }

        public async Task<LabAccessLogRepoPagingDTO> GetLabAccessLogDTOsAsync(int pageNo, int pageSize)
        {
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$lookup", new BsonDocument
                {
                    {"from", "users"},
                    {"localField", "username"},
                    {"foreignField", "username"},
                    {"as", "User"}
                }),
                new BsonDocument("$unwind", new BsonDocument { {"path", "$User"}, {"preserveNullAndEmptyArrays", true} }),
                 new BsonDocument { { "$sort", new BsonDocument { { "launchedAt", -1 } } } },
                 new BsonDocument { { "$skip", (pageNo - 1) * pageSize } },
                 new BsonDocument { { "$limit", pageSize } },
                new BsonDocument("$project", new BsonDocument
                {

                    {"ID", 1},
                    {"firstname", "$User.firstname"},
                    {"lastname", "$User.lastname"},
                    {"labname", 1},
                    { "username",1},
                    {"launchedAt", 1},
                    {"closedAt", 1},
                    {"duration", 1},
                    {"userType","$User.userType" }
                })
            };




            var result = await _collectionLabAccessLog.Aggregate<LabAccessLogRepoDTO>(pipeline).ToListAsync();
            var labs = await _collectionLabs.Find(_ => true).ToListAsync();

            foreach(LabAccessLogRepoDTO log in result)
            {
                var lab = labs.Where(m => m.HostPoolName == log.LabName).FirstOrDefault();
                if (lab != null)
                {
                    log.LabName = lab.Name;
                }
            }

            LabAccessLogRepoPagingDTO labAccessLog = new LabAccessLogRepoPagingDTO
            {
                Logs = result,
                TotalCount = (int)_collectionLabAccessLog.CountDocuments(m => true),
                PageSize = pageSize,
                CurrentPage = pageNo
            };

            return labAccessLog;
        }

        public async Task<DateTime?> GetLatestLaunchTime()
        {
            var filter = Builders<LabAccessLog>.Filter.Empty;
            var sort = Builders<LabAccessLog>.Sort.Descending(x => x.LaunchedAt);
            var pipeline = new[]
            {
                new BsonDocument("$sort", new BsonDocument("closedAt", -1)),
                new BsonDocument("$limit", 1)
            };

            var result = await _collectionLabAccessLog.Aggregate<LabAccessLogRepoDTO>(pipeline).FirstOrDefaultAsync();
            return (result == null ? null : result.ClosedAt);
        }

        public async Task<List<LabAccessLogCountDTO>> GetLabAccessCount2()
        {
            var filter = Builders<LabAccessLog>.Filter.Gte(l => l.LaunchedAt, DateTime.Now.AddDays(-7));

            var daysPipeline = new BsonDocument[]
{
            new BsonDocument("$match", new BsonDocument("LaunchedAt", new BsonDocument("$gte", DateTime.Now.AddDays(-7)))),
            new BsonDocument("$group", new BsonDocument
            {
                {"_id", new BsonDocument
                {
                    {"labId", "$LabID"},
                    {"day", new BsonDocument("$dateToString", new BsonDocument
                    {
                        {"format", "%Y-%m-%d"},
                        {"date", "$LaunchedAt"}
                    })}
                }},
                {"count", new BsonDocument("$sum", 1)}
            }),
            new BsonDocument("$sort", new BsonDocument("_id.day", 1))
};

            var result = await _collectionLabAccessLog.Aggregate<BsonDocument>(daysPipeline).ToListAsync();

            var labLaunchCounts = new Dictionary<string, Dictionary<string, int>>();

            foreach (var doc in result)
            {
                var labId = doc["_id"]["labId"].AsString;
                var day = doc["_id"]["day"].AsString;
                var count = doc["count"].AsInt32;

                if (!labLaunchCounts.TryGetValue(labId, out var dayCounts))
                {
                    dayCounts = new Dictionary<string, int>();
                    labLaunchCounts[labId] = dayCounts;
                }

                dayCounts[day] = count;
            }
            return null;
        }

        public async Task<(int, TimeSpan)> GetLabAccessDetailByLab(string labHostname)
        {
            var collection = await _collectionLabAccessLog.Find(m => m.LabName == labHostname).ToListAsync();
            var totalcount=collection.Count();
            var totalDuration = new TimeSpan();
            foreach (var item in collection)
            {
                var ts = new TimeSpan();
                TimeSpan.TryParse(item.Duration,out ts);
                totalDuration += ts;
            }
            return (totalcount, totalDuration);
        }

        public async Task<LabAccessLogCountDTO> GetLabAccessCount(string filter = "all")
        {

            var labAccessLogCountDTO = new LabAccessLogCountDTO();
            DateTime dt = (filter == "all" || filter == "year") ? DateTime.Now.AddYears(-1) : DateTime.Now.AddDays(-38);

            var pipeline = new BsonDocument[]
            {
               new BsonDocument { { "$match", new BsonDocument { { "launchedAt", new BsonDocument { { "$gte", new DateTime(dt.Year,dt.Month,1) } } } } } }
            };
            var logs = await _collectionLabAccessLog.Aggregate<LabAccessLogRepoDTO>(pipeline).ToListAsync();
            var labs = await _collectionLabs.Find(_ => true).ToListAsync();

            List<DateTime> days = Enumerable.Range(0, 7)
            .Select(i => DateTime.Now.AddDays(-i))
            .ToList();

            if (filter == "all" || filter == "day")
            {
                days = days.OrderBy(m => m.Date).ToList();
                labAccessLogCountDTO.Days = new List<Data>();
                foreach (var day in days)
                {
                    List<LabData> labData = new List<LabData>();
                    foreach (var lab in labs)
                    {
                        int count = logs.Where(m => m.LabName == lab.HostPoolName && m.LaunchedAt.Day == day.Day).Count();
                        labData.Add(new LabData { LabName = lab.Name, Count = count });
                    }
                    labAccessLogCountDTO.Days.Add(new Data { TimeFrame = day.ToString("dd-MM-yyyy"), LabData = labData });
                }
            }

            if (filter == "all" || filter == "week")
            {
                DateTime today = DateTime.Now;
                DateTime mostRecentMonday = today.AddDays(-(int)today.DayOfWeek + 1);

                var weeks = Enumerable.Range(0, 4)
                    .Select(i => mostRecentMonday.AddDays(-7 * i))
                   .ToList();

                weeks = weeks.OrderBy(m => m.Date).ToList();
                labAccessLogCountDTO.Weeks = new List<Data>();
                foreach (var week in weeks)
                {
                    List<LabData> labData = new List<LabData>();
                    foreach (var lab in labs)
                    {
                        int count = logs.Where(m => m.LabName == lab.HostPoolName && m.LaunchedAt > week && m.LaunchedAt < week.AddDays(7)).Count();
                        labData.Add(new LabData { LabName = lab.Name, Count = count });
                    }
                    labAccessLogCountDTO.Weeks.Add(new Data { TimeFrame = week.ToString("dd-MM-yyyy") + " to " + week.AddDays(7).ToString("dd-MM-yyyy"), LabData = labData });
                }
            }

            if (filter == "all" || filter == "month")
            {
                List<DateTime> months = new List<DateTime>();

                for (int i = 11; i >=0; i--)
                {
                    DateTime month = DateTime.Now.AddMonths(-i);
                    month = new DateTime(month.Year, month.Month, 1); 
                    months.Add(month);
                }
                 
                labAccessLogCountDTO.Months = new List<Data>();
                foreach (var month in months)
                {
                    List<LabData> labData = new List<LabData>();
                    foreach (var lab in labs)
                    {
                        int count = logs.Where(m => m.LabName == lab.HostPoolName && m.LaunchedAt > month && m.LaunchedAt < month.AddMonths(1)).Count();
                        labData.Add(new LabData { LabName = lab.Name, Count = count });
                    }
                    labAccessLogCountDTO.Months.Add(new Data { TimeFrame = month.ToString("MMM") + " " + month.Year, LabData = labData });
                }
            }

            return labAccessLogCountDTO;
        }
        public async Task<BsonDocument[]> GetLabAccessCountPerDayLastWeek()
        {
            var pipeline = new BsonDocument[]
            {
            new BsonDocument { { "$match", new BsonDocument { { "launchedAt", new BsonDocument { { "$gte", DateTime.Now.AddDays(-7) } } } } } },
            new BsonDocument("$group", new BsonDocument
            {
                {"_id", new BsonDocument
                {
                    {"labname", "$LabName"},
                    {"day", new BsonDocument("$dateToString", new BsonDocument
                    {
                        {"format", "%Y-%m-%d"},
                        {"date", "$LaunchedAt"}
                    })}
                }},
                {"count", new BsonDocument("$sum", 1)}
            }),
            new BsonDocument { { "$sort", new BsonDocument { { "_id.date", 1 } } } }
            };

            var result = await _collectionLabAccessLog.Aggregate<BsonDocument>(pipeline).ToListAsync();
            return result.ToArray();
        }

        public async Task<BsonDocument[]> GetLabAccessCountPerWeekLastMonth()
        {
            var pipeline = new BsonDocument[]
            {
            new BsonDocument { { "$match", new BsonDocument { { "launchedAt", new BsonDocument { { "$gte", DateTime.Now.AddDays(-30) } } } } } },
            new BsonDocument { { "$group", new BsonDocument { { "_id", new BsonDocument { { "labname", "$LabName" }, { "week", new BsonDocument { { "$week", "$LaunchedAt" } } } } }, { "count", new BsonDocument { { "$sum", 1 } } } } } } ,
            new BsonDocument { { "$sort", new BsonDocument { { "_id.week", 1 } } } }
            };

            var result = await _collectionLabAccessLog.Aggregate<BsonDocument>(pipeline).ToListAsync();
            return result.ToArray();
        }
        public async Task<BsonDocument[]> GetLabAccessCountPerMonthLastYear()
        {
            var pipeline = new BsonDocument[]
            {
            new BsonDocument { { "$match", new BsonDocument { { "launchedAt", new BsonDocument { { "$gte", DateTime.Now.AddYears(-1) } } } } } },
            new BsonDocument { { "$group", new BsonDocument { { "_id", new BsonDocument { { "labname", "$LabName" }, { "month", new BsonDocument { { "$month", "$LaunchedAt" } } }, { "year", new BsonDocument { { "$year", "$LaunchedAt" } } } } }, { "count", new BsonDocument { { "$sum", 1 } } } } } } ,
            new BsonDocument { { "$sort", new BsonDocument { { "_id.year", 1 }, { "_id.month", 1 } } } }
        };

            var result = await _collectionLabAccessLog.Aggregate<BsonDocument>(pipeline).ToListAsync();
            return result.ToArray();
        }

    }
}
