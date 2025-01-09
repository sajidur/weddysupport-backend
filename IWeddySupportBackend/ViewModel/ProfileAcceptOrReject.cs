namespace IWeddySupport.ViewModel
{
    public class ProfileAcceptOrReject
    {
        public string MyProfileId { get; set; }
        public string RequesterProfileId { get; set; }
        public string RequesterUserId { get; set; }
        public string RequestAccepted { get; set; }
        public string RequestRejected { get; set; }
    }
    public class ProfileResponse
    {
        public string MyProfileId { get; set; }
        public string ResponserProfileId { get; set; }
        public string ResponserUserId { get; set; }
    
    }
}
