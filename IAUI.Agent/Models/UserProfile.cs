using MongoDB.Bson.Serialization.Attributes;

namespace IAUI.Agent.Models;

public class UserProfile
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.Int64)]
    public long Id { get; set; }

    [BsonElement("Name")]
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public Location Address { get; set; } = new();
    public DateTime AccountCreationDate { get; set; }
    public UserLoginHistory[] LoginHistory { get; set; } = [];
    public UserProfileScore[] ProfileScores { get; set; } = [];
}
