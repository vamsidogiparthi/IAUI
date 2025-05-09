namespace IAUI.Agent.Models
{
    public class DeviceInformation
    {
        public long DeviceId { get; set; }
        public string DeviceName { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty; // e.g., Mobile, Desktop, Tablet
        public string OperatingSystem { get; set; } = string.Empty;
        public string Browser { get; set; } = string.Empty;
        public DateTime LastActive { get; set; }
    }
}
