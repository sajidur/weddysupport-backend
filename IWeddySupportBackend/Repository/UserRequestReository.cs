using IWeddySupport.Model;

namespace IWeddySupport.Repository
{
    public interface IUserRequestReository : IGenericRepository<UserRequest>
    {

    }
    public class UserRequestReository : GenericRepository<UserRequest>, IUserRequestReository
    {
        public UserRequestReository(IWeddySupportDbContext context) : base(context)
        {

        }
    }
    
}
