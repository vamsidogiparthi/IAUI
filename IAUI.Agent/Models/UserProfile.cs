namespace IAUI.Agent.Models;

public class UserProfile
{
    public long UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public Location Address { get; set; } = new();
    public DateTime AccountCreationDate { get; set; }
    public UserLoginHistory[] LoginHistory { get; set; } = [];
}
