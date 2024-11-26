using IWeddySupport.Model;
using IWeddySupport.Repository;
namespace IWeddySupport.Repository
{
    public interface IAddressRepository : IGenericRepository<Address>
    {

    }
    public class AddressRepository: GenericRepository<Address>, IAddressRepository
    {
        public AddressRepository(IWeddySupportDbContext context) : base(context)
        {

        }
    }
}
