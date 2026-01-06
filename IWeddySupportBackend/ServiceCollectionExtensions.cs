
using IWeddySupport.Repository;
using IWeddySupport.Service;

namespace IWeddySupport
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register your application services here
            // Register the generic repository and the specific repository
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            // Register specific repositories
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IEducationalQualificationRepository, EducationalQualificationRepository>();
            services.AddScoped<IProfilePhotoRepository, ProfilePhotoRepository>();
            services.AddScoped<IPartnerExpectationRepository, PartnerExpectationRepository>();
            services.AddScoped<IProfileRepository, ProfileRepository>();
            services.AddScoped<IUserProfileRepository, UserProfileRepository>();
            services.AddScoped<IUserRequestReository, UserRequestReository>();
            services.AddScoped<IUserDeviceRepository, UserDeviceRepository>();
            // Add other service registrations here
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IFirebaseNotificationService, FirebaseNotificationService>();

            return services;
        }
    }
}
