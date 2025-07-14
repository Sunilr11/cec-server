

namespace GDT.CEC.Repository.Models.Configurations
{
    public class CacheConfig
    {
        public bool CacheCheck { get; set; }
        public int AllUser_Cache_ExpiryTime { get; set; }
        public int CSP_Cache_ExpiryTime { get; set; }
        public int OTP_ExpiryTime { get; set; }
    }
}
