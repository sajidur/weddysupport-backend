using IWeddySupport.Controller;
using IWeddySupport.Model;
using IWeddySupport.Repository;
using IWeddySupport.Repository;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Org.BouncyCastle.Crypto.Prng;
using System.Linq;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IWeddySupport.Service
{
    public interface IUserService
    {
        Task<string> SavePhotoAsync(IFormFile file, string uploadPath);
        Task<IEnumerable<Profile>> GetAllProfilesAsync(string userId);
        Task<Profile?> GetProfileByIdAsync(string id);
        Task<Profile> CreateProfileAsync(Profile profile);
        Task<Profile> UpdateProfileAsync(Profile profile);
        Task<Profile> DeleteProfileAsync(string id);
        Task<IEnumerable<Address>> GetAllAddressesAsync(string userId);
        Task<Address> CreateAddressAsync(Address address);
        Task<Address> GetAddressAsync(string id);   
        Task<Address> UpdateAddressAsync(Address address);
        Task<Address> DeleteAddressAsync(string id);
        Task<ProfilePhoto> GetProfilePhotoAsync(string id); 
        Task<ProfilePhoto> CreateProfilePhotoAsync( ProfilePhoto profilePhoto); 
        Task<ProfilePhoto> UpdateProfilePhotoAsync( ProfilePhoto profilePhoto); 
        Task<ProfilePhoto> DeleteProfilePhotoAsync( string id);
        Task<IEnumerable<ProfilePhoto>> GetAllProfilePhotoAsync(string userId);
        Task<IEnumerable<UserProfile>> GetAllUserProfilesAsync(string userId);
        Task<UserProfile> GetUserProfileAsync(string userId);
        Task<UserProfile> CreateUserProfileAsync(UserProfile userProfile);
        Task<UserProfile> UpdateUserProfileAsync(UserProfile userProfile);
        Task<UserProfile> DeleteUserProfileAsync(string userId);
        Task<IEnumerable<PartnerExpectation>> GetAllExpectedPartnersAsync(string userId);
        Task<PartnerExpectation> GetExpectedPartnerAsync(string Id);
        Task<PartnerExpectation> CreateExpectedPartnerAsync(PartnerExpectation expectedPartner);
        Task<PartnerExpectation> UpdateExpectedPartnerAsync(PartnerExpectation expectedPartner);
        Task<PartnerExpectation> DeleteExpectedPartnerAsync(string Id);
        Task<IEnumerable<Profile>> GetExpectedPartnersByKeyAsync(SearchKeyViewModel key);
    }

    public class UserService : IUserService
    {
        private readonly IProfilePhotoRepository _profilePhotoRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IPartnerExpectationRepository _expectedPartnerRepository;
        public UserService(IPartnerExpectationRepository expectedPartnerRepository, IUserProfileRepository userProfileRepository,IProfileRepository profileRepository, 
            IAddressRepository addressRepository,IProfilePhotoRepository profilePhotoRepository)
        {
            _profileRepository = profileRepository;
            _addressRepository = addressRepository;
            _profilePhotoRepository = profilePhotoRepository;
            _userProfileRepository = userProfileRepository;
            _expectedPartnerRepository = expectedPartnerRepository; 
        }

        public async Task<IEnumerable<Profile>> GetAllProfilesAsync(string userId)
        {
           return await _profileRepository.FindAsync(a=>a.UserId==userId);
        }

        public async Task<Profile> GetProfileByIdAsync(string id)
        {
   
            var result= await _profileRepository.FindAsync(u=>u.Id==Guid.Parse(id));
            return result.FirstOrDefault();
        }

        public async Task<Profile> CreateProfileAsync(Profile profile)
        {
            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile), "Profile cannot be null.");
            }

            var result = await _profileRepository.AddAsync(profile);
            return profile; // Assuming AddAsync saves and returns the entity.
        }

        public async Task<Profile> UpdateProfileAsync( Profile profile)
        {
            await _profileRepository.UpdateAsync(profile);
            return profile;
        }

        public async Task<Profile> DeleteProfileAsync(string id)
        {
           
            var profiles = await _profileRepository.FindAsync(u=>u.Id==Guid.Parse(id));
            if (profiles == null)
            {
                return null;
            }
            var profile = profiles.FirstOrDefault();    
            await _profileRepository.RemoveAsync(profile);
            return profile;
        }
        public async Task<IEnumerable<Address>> GetAllAddressesAsync(string userId)
        {
       
            return await _addressRepository.FindAsync(a => a.UserId == userId); 

        }
        public async Task<Address> CreateAddressAsync(Address address)
        {
                 // Save the address to the database
                var createdAddress = await _addressRepository.AddAsync(address);
                // You can add additional business logic here if needed
                return address;
           
         }
        public async Task<Address> GetAddressAsync(string id)
        {
            var addresses = await _addressRepository.FindAsync(a => a.Id == Guid.Parse(id));
            return addresses.FirstOrDefault();
        }
        public async Task<Address> UpdateAddressAsync(Address address)
        {
            await _addressRepository.UpdateAsync(address);
           
            return address;
        }
        public async Task<Address> DeleteAddressAsync(string id)
        {
            var addresses = await _addressRepository.FindAsync(u => u.Id == Guid.Parse(id));
            if (addresses == null)
            {
                return null;
            }
            var address = addresses.FirstOrDefault();
            await _addressRepository.RemoveAsync(address);
            return address;
        }
        // Get all profile photos
        public async Task<IEnumerable<ProfilePhoto>> GetAllProfilePhotoAsync(string userId)
        {
            return await _profilePhotoRepository.FindAsync(a => a.UserId == userId);
        }

        // Create a profile photo
        public async Task<ProfilePhoto> CreateProfilePhotoAsync(ProfilePhoto profilePhoto)
        {
            var createdPhoto = await _profilePhotoRepository.AddAsync(profilePhoto);
            return profilePhoto;
        }

        // Get a specific profile photo by ID
        public async Task<ProfilePhoto> GetProfilePhotoAsync(string id)
        {
            var profilePhotos = await _profilePhotoRepository.FindAsync(p => p.Id == Guid.Parse(id));
            return profilePhotos.FirstOrDefault();
        }

        // Update a profile photo
        public async Task<ProfilePhoto> UpdateProfilePhotoAsync(ProfilePhoto profilePhoto)
        {
            await _profilePhotoRepository.UpdateAsync(profilePhoto);
            return profilePhoto;
        }

        // Delete a profile photo by ID
        public async Task<ProfilePhoto> DeleteProfilePhotoAsync(string id)
        {
            var profilePhotos = await _profilePhotoRepository.FindAsync(p => p.Id == Guid.Parse(id));
            if (profilePhotos == null || !profilePhotos.Any())
            {
                return null;
            }

            var profilePhoto = profilePhotos.FirstOrDefault();
            await _profilePhotoRepository.RemoveAsync(profilePhoto);
            return profilePhoto;
        }
        // Get all user profiles
        public async Task<IEnumerable<UserProfile>> GetAllUserProfilesAsync(string userId)
        {
            return await _userProfileRepository.FindAsync(a => a.UserId == userId);
        }

        // Get a user profile by UserId
        public async Task<UserProfile> GetUserProfileAsync(string Id)
        {
            var userProfiles = await _userProfileRepository.FindAsync(u => u.Id == Guid.Parse(Id));
            return userProfiles.FirstOrDefault();
        }

        // Create a new user profile
        public async Task<UserProfile> CreateUserProfileAsync(UserProfile userProfile)
        {
            var createdUserProfile = await _userProfileRepository.AddAsync(userProfile);
            return userProfile;
        }

        // Update an existing user profile
        public async Task<UserProfile> UpdateUserProfileAsync(UserProfile userProfile)
        {
            await _userProfileRepository.UpdateAsync(userProfile);
            return userProfile;
        }

        // Delete a user profile by UserId
        public async Task<UserProfile> DeleteUserProfileAsync(string Id)
        {
            var userProfiles = await _userProfileRepository.FindAsync(u => u.Id == Guid.Parse(Id));
            if (!userProfiles.Any())
            {
                return null;
            }

            var userProfile = userProfiles.FirstOrDefault();
            await _userProfileRepository.RemoveAsync(userProfile);
            return userProfile;
        }
        // Get all expected partners
        public async Task<IEnumerable<PartnerExpectation>> GetAllExpectedPartnersAsync(string userId)
        {
            return await _expectedPartnerRepository.FindAsync(a => a.UserId == userId);
        }

        // Get a specific expected partner by Id
        public async Task<PartnerExpectation> GetExpectedPartnerAsync(string Id)
        {
            var partners = await _expectedPartnerRepository.FindAsync(p => p.Id ==Guid.Parse(Id));
            return partners.FirstOrDefault();
        }

        // Create a new expected partner
        public async Task<PartnerExpectation> CreateExpectedPartnerAsync(PartnerExpectation expectedPartner)
        {
            var createdPartner = await _expectedPartnerRepository.AddAsync(expectedPartner);
            return expectedPartner;
        }

        // Update an existing expected partner
        public async Task<PartnerExpectation> UpdateExpectedPartnerAsync(PartnerExpectation expectedPartner)
        {
            await _expectedPartnerRepository.UpdateAsync(expectedPartner);
            return expectedPartner;
        }

        // Delete an expected partner by Id
        public async Task<PartnerExpectation> DeleteExpectedPartnerAsync(string Id)
        {
            var partners = await _expectedPartnerRepository.FindAsync(p => p.Id == Guid.Parse(Id));
            if (!partners.Any())
            {
                return null;
            }

            var partnerToDelete = partners.FirstOrDefault();
            await _expectedPartnerRepository.RemoveAsync(partnerToDelete);
            return partnerToDelete;
        }
       
        public async Task<IEnumerable<Profile>> GetExpectedPartnersByKeyAsync(SearchKeyViewModel key)
        {

            // Initial profile query
          
            var currentDate = DateTime.Now;
            var maxDateOfBirth = currentDate.AddYears(-key.MinAge); // Oldest acceptable date of birth
            var minDateOfBirth = currentDate.AddYears(-key.MaxAge); // Youngest acceptable date of birth

            var profiles = await _profileRepository.FindAsync(p =>
                (!string.IsNullOrEmpty(p.SkinTone) && p.SkinTone.ToLower().Contains(key.SkinTon.ToLower())) ||
                (!string.IsNullOrEmpty(p.BloodGroup) && p.BloodGroup.ToLower().Contains(key.BloodGroup.ToLower())) ||
                (!string.IsNullOrEmpty(p.Occupation) && p.Occupation.ToLower().Contains(key.Occupation.ToLower())) ||
                (!string.IsNullOrEmpty(p.Religion) && p.Religion.ToLower().Contains(key.Religious.ToLower())) ||
                (!string.IsNullOrEmpty(p.MaritalStatus) && p.MaritalStatus.ToLower().Contains(key.MaritalStatus.ToLower())) ||
                (!string.IsNullOrEmpty(p.MotherOccupationDetails) && p.MotherOccupationDetails.ToLower().Contains(key.MotherOccupation.ToLower())) ||
                (!string.IsNullOrEmpty(p.FatherOccupationDetails) && p.FatherOccupationDetails.ToLower().Contains(key.FatherOccupation.ToLower())) ||
                (!string.IsNullOrEmpty(p.Gender) && p.Gender.ToLower().Contains(key.Gender.ToLower())) ||
                (p.DateOfBirth != null && p.DateOfBirth >= minDateOfBirth && p.DateOfBirth <= maxDateOfBirth) ||
                (p.YearlySalary >= key.MinYearlySalary && p.YearlySalary <= key.MaxYearlySalary) ||
                (p.Height >= key.MinHeight && p.Height <= key.MaxHeight)||
                (p.CanReciteQuranProperly == key.CanReciteQuranProperly));

            // Search in AddressRepository if profiles list is still null or empty
            if (profiles == null || !profiles.Any())
            {
                var addressResults = await _addressRepository.FindAsync(a =>
                     (!string.IsNullOrEmpty(a.CurrentAddress.localAddress) && a.CurrentAddress.localAddress.ToLower().Contains(key.LocalAddress.ToLower())) ||
                    (!string.IsNullOrEmpty(a.CurrentAddress.Thana) && a.CurrentAddress.Thana.ToLower().Contains(key.Thana.ToLower())) ||
                    (!string.IsNullOrEmpty(a.CurrentAddress.District) && a.CurrentAddress.District.ToLower().Contains(key.District.ToLower())) ||
                    (!string.IsNullOrEmpty(a.PermanentAddress.Thana) && a.PermanentAddress.Thana.ToLower().Contains(key.LocalAddress.ToLower())) ||
                    (!string.IsNullOrEmpty(a.PermanentAddress.District) && a.PermanentAddress.District.ToLower().Contains(key.Thana.ToLower()))||
                    (!string.IsNullOrEmpty(a.PermanentAddress.localAddress) && a.PermanentAddress.localAddress.ToLower().Contains(key.District.ToLower())));

                if (addressResults.Any())
                {
                    var profileIds = addressResults.Select(a => a.ProfileId).Distinct();
                    var profileSearchResults = new List<Profile>();

                    foreach (var profileId in profileIds)
                    {
                        var profile = await _profileRepository.FindAsync(p => p.Id == Guid.Parse(profileId));
                        if (profile != null)
                        {
                            profileSearchResults.Add(profile.FirstOrDefault());
                        }
                    }

                    profiles = profiles != null
                        ? profiles.Concat(profileSearchResults).ToList()
                        : profileSearchResults;
                }
            }

            return profiles?.Distinct().ToList() ?? new List<Profile>();
        }

        public async Task<string> SavePhotoAsync(IFormFile file, string uploadPath)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Invalid file. The file cannot be null or empty.", nameof(file));
            }

            try
            {
                // Generate a unique file name to prevent overwriting existing files
                string fileExtension = Path.GetExtension(file.FileName);
                string fileName = $"{Guid.NewGuid()}{fileExtension}";

                // Construct the full file path
                string filePath = Path.Combine(uploadPath, fileName);

                // Save the file to the specified path
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                return fileName;
            }
            catch (Exception ex)
            {
                // Optionally, log the exception (using a logger, if available)
                throw new IOException("An error occurred while saving the file.", ex);
            }
        }





    }
}
