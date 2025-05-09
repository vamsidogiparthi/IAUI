namespace IAUI.Agent.Models.Dtos;

public class UserProfileInfoProfiScoringDto
{
    public long Id { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Location Address { get; set; } = new();
    public DateTime AccountCreationDate { get; set; }
    public UserLoginHistory[] LoginHistory { get; set; } = [];
    public int LoginTimes
    {
        get { return LoginHistory.Length; }
    }
    public long ActiveLoginTimes
    {
        get { return LoginHistory.Select(x => x.LogoutTime - x.LoginTime).Sum(x => x.Minutes); }
    }
}
