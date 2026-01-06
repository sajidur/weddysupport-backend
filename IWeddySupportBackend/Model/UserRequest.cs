namespace IWeddySupport.Model
{
    public class UserRequest:BaseEntity
    {
        public string RequesterProfileId { get; set; }
        public string RequesterUserId { get; set; }
        public string ExpacterProfileId { get; set; }
        public string ExpacterUserId { get; set; }
        public string? UserRequestAccepted { get; set; }
        public string? UserRequestRejected { get; set; }
        public string ApplicationStatus {  get; set; }  
        public string? Message { get; set; } = string.Empty;

    }
}
