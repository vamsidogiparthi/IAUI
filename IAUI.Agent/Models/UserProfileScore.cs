namespace IAUI.Agent.Models
{
    public class UserProfileScore
    {
        public long UserId { get; set; }
        public long ScoreId { get; set; }
        public int Score { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Algorithm { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }
}
