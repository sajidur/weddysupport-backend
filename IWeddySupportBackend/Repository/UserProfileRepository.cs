using IWeddySupport.Model;

namespace IWeddySupport.Repository
{
  
    public interface IUserProfileRepository : IGenericRepository<UserProfile>
    {

    }

    public class UserProfileRepository : GenericRepository<UserProfile>, IUserProfileRepository
    {
        public UserProfileRepository(IWeddySupportDbContext context) : base(context)
        {

        }
    }
}
