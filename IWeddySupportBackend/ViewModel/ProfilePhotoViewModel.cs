namespace IWeddySupport.ViewModel
{
    public class ProfilePhotoViewModel
    {
      
       
        public string ProfileId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; } // File size in bytes
        public DateTime UploadedAt { get; set; }
    }
}
