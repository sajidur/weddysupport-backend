﻿using System.ComponentModel.DataAnnotations;

namespace IWeddySupport.Model
{
    public class Profile : BaseEntity
    {
        public bool isPublic { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Nationality { get; set; }
        public string Religion { get; set; }
        public string Occupation { get; set; } // "পেশা"
        public int YearlySalary { get; set; }
        public string CompanyOrInstituteName { get; set; }
        public string MaritalStatus { get; set; }
        public string Declaration { get; set; }
        public string SkinTone { get; set; } // Preferred skin tones (e.g., "শ্যামলা, উজ্জল শ্যামলা, ফর্সা")
                                             // Shamla (শ্যামলা): Dusky or wheatish complexion. Forsha(ফর্সা) : Fair complexion. Kala(কালা): Dark or black complexion.
        public int Height { get; set; }
        public string Weight { get; set; }
        public string BloodGroup { get; set; }
      // public string ClothingPreferenceOutside { get; set; } // "ঘরের বাহিরে সাধারণত কি ধরণের পোষাক পরেন?"
        public string PrayerHabit { get; set; } // "প্রতিদিন পাঁচ ওয়াক্ত নামাজ পড়েন কি? কবে থেকে পড়ছেন?"
        public bool CanReciteQuranProperly { get; set; } // "শুদ্ধভাবে কুরআন তিলওয়াত করতে পারেন?"
        public bool HasMentalOrPhysicalIllness { get; set; } // "আপনার মানসিক বা শারীরিক কোনো রোগ আছে?"
        public bool IsFatherAlive { get; set; } // "পিতা জীবিত?"
        public string FatherOccupationDetails { get; set; } // "পিতার পেশার বিস্তারিত বিবরণ"
        public bool IsMotherAlive { get; set; } // "মাতা জীবিত?"
        public string MotherOccupationDetails { get; set; } // "মাতার পেশার বিস্তারিত বিবরণ"
        public int NumberOfBrothers { get; set; } // "ভাইয়ের সংখ্যা"

        [Range(0, int.MaxValue, ErrorMessage = "Number of sisters must be a non-negative integer.")]
        public int NumberOfSisters { get; set; } // "বোনের সংখ্যা"
        public string FamilyDetails { get; set; }
        public string FamilyEconomicsCondition { get; set; } // "পরিবারের আর্থিক অবস্থা"
       // public string FamilyEconomicsDetails { get; set; } // "পরিবারের আর্থিক অবস্থা বিস্তারিত"
        public string FamilyReligiousEnvironment { get; set; } // "পরিবারের ধর্মীয় পরিবেশ"
        public string UserId { get; set; }
    }
    //public class MaritalInfo {
    //    public bool IsGuardianConsenting { get; set; } // "অভিভাবক আপনার বিয়েতে রাজি কি না?"
    //    public string PostMarriageCareerPlan { get; set; } // "আপনি কি বিয়ের পর চাকরি করতে ইচ্ছুক?"
    //    public string PostMarriageEducationPlan { get; set; } // "বিয়ের পর পড়াশোনা চালিয়ে যেতে চান?"
    //    public string ReasonForMarriage { get; set; } // "কেন বিয়ে করছেন? বিয়ে সম্পর্কে আপনার ধারণা কি?"
    //}
    //public class Declaration
    //{
    //    public bool IsGuardianAware { get; set; } // "অভিভাবক জানেন?"
    //    public bool IsInformationTrue { get; set; } // "তথ্যগুলো সত্য?"
    //    public bool AgreesToTerms { get; set; } // "দায়ভার সম্মত?"
    //}



}
