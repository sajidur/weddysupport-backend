namespace IWeddySupport.Model
{
    public class ProfilePhoto:BaseEntity
    {
        public string UserId {  get; set; }
        public string? ProfileId {  get; set; }  
        public string PhotoUrl {  get; set; }   
    }
}
