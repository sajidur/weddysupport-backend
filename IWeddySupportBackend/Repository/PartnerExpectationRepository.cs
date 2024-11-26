using IWeddySupport.Model;

namespace IWeddySupport.Repository
{
    public interface IPartnerExpectationRepository : IGenericRepository<PartnerExpectation>
    {
    }

    public class PartnerExpectationRepository : GenericRepository<PartnerExpectation>, IPartnerExpectationRepository
    {
        public PartnerExpectationRepository(IWeddySupportDbContext context) : base(context)
        {
        }
    }

}
