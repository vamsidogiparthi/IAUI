using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace IAUI.Agent.Models
{
    public class UIComponentLibrary
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.Int64)]
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [BsonElement("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("version")]
        [JsonPropertyName("version")]
        public string Version { get; set; } = string.Empty;

        [BsonElement("description")]
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("matchingUserProfileScoreMin")]
        [JsonPropertyName("matchingUserProfileScoreMin")]
        public int MatchingUserProfileScoreMin { get; set; }

        [BsonElement("matchingUserProfileScoreMax")]
        [JsonPropertyName("matchingUserProfileScoreMax")]
        public int MatchingUserProfileScoreMax { get; set; }

        [BsonElement("componentTag")]
        [JsonPropertyName("componentTag")]
        public string ComponentTag { get; set; } = string.Empty;
    }
}
