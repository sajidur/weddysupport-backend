using Org.BouncyCastle.Bcpg;

namespace IWeddySupport.ViewModel
{
    public class ProfileUpdateViewModel
    {
        public string Id { get; set; }
        public bool isPublic { get; set; }
        public string PhoneNumber { get; set; }
        public string Occupation { get; set; }
        public int YearlySalary { get; set; }
        public string CompanyOrInstituteName { get; set; }
        public string MaritalStatus { get; set; }
        public string Declaration { get; set; }
        public string SkinTone { get; set; }
        public int Height { get; set; }
        public string Weight { get; set; }
        public string BloodGroup { get; set; }
        // public string ClothingPreferenceOutside { get; set; }
        public string PrayerHabit { get; set; }
        public bool CanReciteQuranProperly { get; set; }
        public bool HasMentalOrPhysicalIllness { get; set; }
        public bool IsFatherAlive { get; set; }
        public string FatherOccupationDetails { get; set; }
        public bool IsMotherAlive { get; set; }

        public string MotherOccupationDetails { get; set; }
        public int NumberOfBrothers { get; set; }

        public int NumberOfSisters { get; set; }

        public string FamilyDetails { get; set; }

        public string FamilyEconomicsCondition { get; set; }

        // public string FamilyEconomicsDetails { get; set; }

        public string FamilyReligiousEnvironment { get; set; }
        public string? FCMToken { get; set; }
    }
}
