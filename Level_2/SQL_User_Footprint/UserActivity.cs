using System;

namespace MysqlUserFootprint
{
    public class UserActivity
    {
        public DateTime Timestamp { get; set; }
        public string User { get; set; }
        public string Action { get; set; }
        public string Details { get; set; }
    }
}

