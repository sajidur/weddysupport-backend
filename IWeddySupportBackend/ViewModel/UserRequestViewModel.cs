namespace IWeddySupport.ViewModel
{
    public class UserRequestViewModel
    {
        public string RequesterProfileId { get; set; }
        public string RequesterUserId { get; set; }
        public string ExpacterProfileId { get; set; }
        public string ExpacterUserId { get; set; }
        public bool UserRequestAccepted { get; set; }
        public bool UserRequestRejected { get; set; }
    }
}
