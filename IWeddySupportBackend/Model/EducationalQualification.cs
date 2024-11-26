namespace IWeddySupport.Model
{
    public class EducationalQualification:BaseEntity
    {

        public string UserId {  get; set; } 
        public String ProfileId {  get; set; }   
        public string HighestEducationLevel { get; set; }
        public string InstituteName { get; set; }
        public int GraduationYear { get; set; }
        public string DivisionOrDepartment { get; set; }
        public string ResultOrGrade { get; set; }
 


    }
   
}
