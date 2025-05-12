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
        public int Score { get; set; } = 0;

        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;

        [JsonPropertyName("algorithm")]
        public string Algorithm { get; set; } = string.Empty;

        [JsonPropertyName("reason")]
        public string Reason { get; set; } = string.Empty;

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string AIModelUsed { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"UserId: {UserId}, ScoreId: {ScoreId}, Score: {Score}, Category: {Category}, Algorithm: {Algorithm}, Reason: {Reason}, CreatedAt: {CreatedAt}";
        }
    }
}
