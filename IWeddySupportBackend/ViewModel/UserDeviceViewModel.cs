namespace IWeddySupport.ViewModel
{
    public class UserDeviceViewModel
    {
        public string ProfileId { get; set; }
        public string? DeviceId { get; set; }
        public string? DeviceType { get; set; } // e.g., "iOS", "Android", "Web"
        public string FCMToken { get; set; } // For push notifications
    }
}
