namespace IWeddySupport.Model
{
    public class UserDevice : BaseEntity
    {
        public string UserId { get; set; }
        public string? DeviceId { get; set; }
        public string? DeviceType { get; set; } // e.g., "iOS", "Android", "Web"
        public string FCMToken { get; set; } // For push notifications
        public string ProfileId { get; set; }
    }
}
