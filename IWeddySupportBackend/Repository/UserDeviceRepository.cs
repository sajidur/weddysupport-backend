using IWeddySupport.Model;

namespace IWeddySupport.Repository
{

    public interface IUserDeviceRepository : IGenericRepository<UserDevice>
    {

    }
    public class UserDeviceRepository : GenericRepository<UserDevice>, IUserDeviceRepository
    {
        public UserDeviceRepository(IWeddySupportDbContext context) : base(context)
        {

        }
    }
}
