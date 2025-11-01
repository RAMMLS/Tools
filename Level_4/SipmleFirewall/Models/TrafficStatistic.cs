namespace SimpleFirewall.Models
{
    public class TrafficStatistic
    {
        public string RuleId { get; set; } = string.Empty;
        public string RuleName { get; set; } = string.Empty;
        public long PacketsAllowed { get; set; }
        public long PacketsBlocked { get; set; }
        public long BytesAllowed { get; set; }
        public long BytesBlocked { get; set; }
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime LastUpdate { get; set; } = DateTime.Now;

        public void Update(bool allowed, long bytes)
        {
            if (allowed)
            {
                PacketsAllowed++;
                BytesAllowed += bytes;
            }
            else
            {
                PacketsBlocked++;
                BytesBlocked += bytes;
            }
            LastUpdate = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{RuleName}: {PacketsAllowed} allowed, {PacketsBlocked} blocked";
        }
    }
}
