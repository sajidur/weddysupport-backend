using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace IWeddySupport.Model
{
    public class PartnerExpectation:BaseEntity
    {
        public string UserId {  get; set; }
         public string ProfileId {  get; set; } 
        [Required]
        [Range(18, 30)]
        public int MinAge { get; set; } // Minimum age of the partner

        [Required]
        [Range(18, 50)]
        public int MaxAge { get; set; } // Maximum age of the partner
        public string SkinTone { get; set; } // Preferred skin tones (e.g., "শ্যামলা, উজ্জল শ্যামলা, ফর্সা")
        public string HeightRange { get; set; } // Preferred height range (e.g., "৫'৪-৫'১০")
        public string EducationalQualification { get; set; } // Required education level
        public string PreferredDistricts { get; set; } // District preferences (e.g., "ঢাকা, গাজীপুর")
        public string MaritalStatus { get; set; } // Marital status (e.g., "অবিবাহিত")
        public string PreferredProfessions { get; set; } // Acceptable professions (e.g., "ব্যবসা, সরকারি চাকরি, বেসরকারি চাকরি")
        public string FinancialStatus { get; set; } // Economic status (e.g., "মধ্যবিও, উচ্চ মধ্যবিও")
        public string DesiredCharacteristics { get; set; } // Desired traits or qualities in a partner
    }
}
