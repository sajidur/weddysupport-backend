namespace IWeddySupport.ViewModel
{
    public class UpdateProfilePhotoViewModel
    {
        public string Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; } // File size in bytes
        public DateTime UploadedAt { get; set; }
    }
}
