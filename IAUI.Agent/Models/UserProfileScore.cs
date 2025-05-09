using System.Text.Json.Serialization;

namespace IAUI.Agent.Models
{
    public class UserProfileScore
    {
        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("scoreId")]
        public long ScoreId { get; set; }

        [JsonPropertyName("score")]
        public int Score { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;

        [JsonPropertyName("algorithm")]
        public string Algorithm { get; set; } = string.Empty;

        [JsonPropertyName("reason")]
        public string Reason { get; set; } = string.Empty;
    }
}
