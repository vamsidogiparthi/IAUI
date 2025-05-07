namespace IAUI.Agent.Models;

public class UserLoginHistory
{
    public long UserId { get; set; }
    public long LoginId { get; set; }
    public DateTime LoginTime { get; set; }
    public DateTime LogoutTime { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public DeviceInformation DeviceInfo { get; set; } = new();
    public Location LoginLocation { get; set; } = new();
    public PageInformation[] PagesVisited { get; set; } = [];
}
