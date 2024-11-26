using IWeddySupport.Model;

namespace IWeddySupport.Repository
{
    public interface IProfileRepository : IGenericRepository<Profile>
    {
    }

    public class ProfileRepository : GenericRepository<Profile>, IProfileRepository
    {
        public ProfileRepository(IWeddySupportDbContext context) : base(context)
        {
        }
    }
}
