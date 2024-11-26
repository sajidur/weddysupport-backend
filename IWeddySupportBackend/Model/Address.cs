using Microsoft.EntityFrameworkCore;

namespace IWeddySupport.Model
{
    public class Address : BaseEntity
    {
        public AddressDetail PermanentAddress { get; set; }
        public AddressDetail CurrentAddress { get; set; }
        public string UserId { get; set; }
        public string ProfileId { get; set; }
    }

    [Owned]
    public class AddressDetail
    {
        public string localAddress { get; set; }
        public string Thana { get; set; }
        public string District { get; set; }
    }
}
