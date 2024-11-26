using IWeddySupport.Model;
using IWeddySupport.Repository;

namespace IWeddySupport.Repository
{
    public interface IEducationalQualificationRepository : IGenericRepository<EducationalQualification>
    {
    }

    public class EducationalQualificationRepository : GenericRepository<EducationalQualification>, IEducationalQualificationRepository
    {
        public EducationalQualificationRepository(IWeddySupportDbContext context) : base(context)
        {
        }
    }
}