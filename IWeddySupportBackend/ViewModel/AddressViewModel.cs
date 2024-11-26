namespace IWeddySupport.ViewModel
{
    public class AddressViewModel
    {
        public AddressDetailViewModel PermanentAddress { get; set; }
        public AddressDetailViewModel CurrentAddress { get; set; }
        public string ProfileId { get; set; }
    }

    public class AddressDetailViewModel
    {
        public string LocalAddress { get; set; }
        public string Thana { get; set; }
        public string District { get; set; }
    }
}
