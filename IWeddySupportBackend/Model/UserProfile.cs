namespace IWeddySupport.Model
{
    public class UserProfile:BaseEntity
    {
        public string UserId {  get; set; } 
        public string? ProfileId { get; set; }  
        public string Relationship { get; set; } 
    }
}
