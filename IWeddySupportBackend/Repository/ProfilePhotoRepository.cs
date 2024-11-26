using IWeddySupport.Model;

namespace IWeddySupport.Repository
{
  

        public interface IProfilePhotoRepository : IGenericRepository<ProfilePhoto>
        {

        }

        public class ProfilePhotoRepository : GenericRepository<ProfilePhoto>, IProfilePhotoRepository
        {
            public ProfilePhotoRepository(IWeddySupportDbContext context) : base(context)
            {

            }
        }
    
}
