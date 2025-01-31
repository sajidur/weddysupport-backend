﻿using IWeddySupport.Controller;
using IWeddySupport.Model;
using IWeddySupport.Repository;
using IWeddySupport.Repository;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Org.BouncyCastle.Crypto.Prng;
using System.Linq;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IWeddySupport.Service
{
    public interface IUserService
    {
        Task<string> SavePhotoAsync(IFormFile file, string uploadPath);
        Task<IEnumerable<Profile>> GetAllProfilesAsync(string userId);
        Task<Profile?> GetProfileByIdAsync(string id);
        Task<Profile> CreateOrGetProfileAsync(Profile profile);
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
        Task<IEnumerable<Profile>> GetExpectedPartnersByKeyAsync(SearchKeyViewModel key,string userId);
        Task<IEnumerable<Profile>> GetAllProfilesAsyncByDefault();
        Task<IEnumerable<UserProfile>> GetAllUserProfilesByDefault();
        Task<IEnumerable<ProfilePhoto>> GetAllProfilePhotosAsyncByDefault();
        Task<IEnumerable<Address>> GetAllAddressAsyncByDefault();
        Task<IEnumerable<PartnerExpectation>> GetAllExpectedPartnersAsyncByDefault();
        Task<UserRequest> AddUserRequestAsync(UserRequest us);
        Task<ProfilePhoto> GetProfilePhotoByProfileIdAsync(string profileId);
        Task<UserProfile> GetUserProfileByProfileIdAsync(string profileId);
        Task<Address> GetProfileAddressByProfileIdAsync(string profileId);
        Task<Address> GetAddressByProfileIdAsync(string userId, string profileId);
        Task<ProfilePhoto> GetProfilePhotoByIdAsync(string userId, string profileId);
        Task<UserProfile> GetUserProfileByProfileIdAsync(string userId, string profileId);
        Task<PartnerExpectation> GetExpectedPartnerByProfileIdAsync(string userId, string profileId);
        Task<UserRequest> GetRequestedProfileAsync(string userId, string profileId);
        Task<UserRequest> GetResponserProfileAsync(string userId, string profileId);
        Task<UserRequest> UpdatedUserRequestAsync(UserRequest us);
        Task<List<UserRequest>> GetAllRequestedProfileAsync(string id);
        Task<List<UserRequest>> GetAllResponsedProfileAsync(string id);
        Task<UserRequest> GetUserRequestAsync(string userId, string profileId);
    }

    public class UserService : IUserService
    {
        private readonly IProfilePhotoRepository _profilePhotoRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IPartnerExpectationRepository _expectedPartnerRepository;
        private readonly IUserRequestReository _userRequestReository;
        public UserService(IPartnerExpectationRepository expectedPartnerRepository, IUserProfileRepository userProfileRepository,IProfileRepository profileRepository, 
            IAddressRepository addressRepository,IProfilePhotoRepository profilePhotoRepository,IUserRequestReository userRequest)
        {
            _profileRepository = profileRepository;
            _addressRepository = addressRepository;
            _profilePhotoRepository = profilePhotoRepository;
            _userProfileRepository = userProfileRepository;
            _expectedPartnerRepository = expectedPartnerRepository; 
            _userRequestReository = userRequest;
        }
        public async Task<List<UserRequest>> GetAllRequestedProfileAsync(string id)
        {
            var profiles=await _userRequestReository.FindAsync(a=>a.RequesterProfileId==id);
            if(profiles==null)
            {
                return null;
            }
            return profiles.ToList();   
        }
        public async Task<List<UserRequest>> GetAllResponsedProfileAsync(string id)
        {
            var profiles = await _userRequestReository.FindAsync(a => a.ExpacterProfileId == id);
            if (profiles == null)
            {
                return null;
            }
            return profiles.ToList();
        }
        public async Task<UserRequest> GetRequestedProfileAsync(string userId, string profileId)
        {
            var profiles=await _userRequestReository.FindAsync(a=>a.RequesterProfileId==profileId&&a.RequesterUserId==userId);
            return profiles.FirstOrDefault();
        }
        public async Task<UserRequest> GetResponserProfileAsync(string userId, string profileId)
        {
            var profiles = await _userRequestReository.FindAsync(a => a.ExpacterProfileId == profileId && a.ExpacterUserId == userId);
            return profiles.FirstOrDefault();
        }
        public async Task<UserRequest> UpdatedUserRequestAsync(UserRequest us)
        {
            await _userRequestReository.UpdateAsync(us);    
            return us;  
        }

        public async Task<Address> GetAddressByProfileIdAsync(string userId, string profileId)
        {
            var addresses = await _addressRepository.FindAsync(a => a.UserId == userId && a.ProfileId == profileId);

            return addresses.FirstOrDefault();
        }
        public async Task<ProfilePhoto> GetProfilePhotoByIdAsync(string userId, string profileId)
        {
            var photos = await _profilePhotoRepository.FindAsync(a => a.UserId == userId && a.ProfileId == profileId);

            return photos.FirstOrDefault();
        }
        public async Task<PartnerExpectation> GetExpectedPartnerByProfileIdAsync(string userId, string profileId)
        {
            var partners = await _expectedPartnerRepository.FindAsync(a => a.UserId == userId && a.ProfileId == profileId);

            return partners.FirstOrDefault();

        }


        public async Task<ProfilePhoto> GetProfilePhotoByProfileIdAsync(string profileId)
        {
            var profiles = await _profilePhotoRepository.FindAsync(a => a.ProfileId == profileId);
            if (profiles.Any())
            {
                return profiles.FirstOrDefault();
            }
            return null;
        }
        public async Task<UserProfile> GetUserProfileByProfileIdAsync(string userId, string profileId)
        {
            var UsProfiles = await _userProfileRepository.FindAsync(a => a.ProfileId == profileId);
            if (UsProfiles.Any())
            {
                return UsProfiles.FirstOrDefault();
            }
            return null;
        }

        public async Task<UserProfile> GetUserProfileByProfileIdAsync(string profileId)
        {
            var profiles = await _userProfileRepository.FindAsync(a => a.ProfileId == profileId);
            if (profiles.Any())
            {
                return profiles.FirstOrDefault();
            }
            return null;
        }
        public async Task<Address> GetProfileAddressByProfileIdAsync(string profileId)
        {
            var profiles = await _addressRepository.FindAsync(a => a.ProfileId == profileId);
            if (profiles.Any())
            {
                return profiles.FirstOrDefault();
            }
            return null;
        }

        public async Task<IEnumerable<PartnerExpectation>> GetAllExpectedPartnersAsyncByDefault()
        {
            return await _expectedPartnerRepository.GetAllAsync();

        }

        public async Task<IEnumerable<ProfilePhoto>> GetAllProfilePhotosAsyncByDefault()
        {
            return await _profilePhotoRepository.GetAllAsync();
        }
        public async Task<IEnumerable<Address>> GetAllAddressAsyncByDefault()
        {
            return await _addressRepository.GetAllAsync();
        }
        public async Task<UserRequest> GetUserRequestAsync(string userId, string profileId)
        {
            var users = await _userRequestReository.FindAsync(a => a.RequesterProfileId == profileId&&a.RequesterUserId==userId);
            if (users.Any())
            {
               return users.FirstOrDefault();
    
            }
            return null;
        }
        public async Task<UserRequest> AddUserRequestAsync(UserRequest us)
        {
           
                us.Message = "First time user has requested";
                await _userRequestReository.AddAsync(us);   
            
            return us;
        }

        public async Task<IEnumerable<UserProfile>> GetAllUserProfilesByDefault()
        {
            return await _userProfileRepository.GetAllAsync(); 
        }

        public async Task<IEnumerable<Profile>> GetAllProfilesAsyncByDefault()
        {
            return await _profileRepository.GetAllAsync();
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

        public async Task<Profile> CreateOrGetProfileAsync(Profile profile)
        {

           
            var profiles = await _profileRepository.FindAsync(a=>a.Email==profile.Email&&a.FullName==profile.FullName&&a.PhoneNumber==profile.PhoneNumber&&a.UserId==profile.UserId);
            if (!profiles.Any())
            {
                var result = await _profileRepository.AddAsync(profile);
                return profile; // Assuming AddAsync saves and returns the entity.
            }
            return profiles.FirstOrDefault();
        }

        public async Task<Profile> UpdateProfileAsync( Profile profile)
        {
            await _profileRepository.UpdateAsync(profile);
            return profile;
        }

        public async Task<Profile> DeleteProfileAsync(string id)
        {
           
            var profiles = await _profileRepository.FindAsync(u=>u.Id==Guid.Parse(id));
            if (!profiles.Any())
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
            if (!addresses.Any())
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

        public async Task<IEnumerable<Profile>> GetExpectedPartnersByKeyAsync(SearchKeyViewModel key,string userId)
        {
            bool presentData = false, presentAddress = false;
            var currentDate = DateTime.Now;

            // Calculate acceptable date of birth range only if MinAge and MaxAge are not 0
            DateTime? maxDateOfBirth = key.MinAge > 0 ? currentDate.AddYears(-key.MinAge) : (DateTime?)null;
            DateTime? minDateOfBirth = key.MaxAge > 0 ? currentDate.AddYears(-key.MaxAge) : (DateTime?)null;
            if (key.MinAge > 0 || key.MaxAge > 0|| !string.IsNullOrEmpty(key.SkinTon)|| !string.IsNullOrEmpty(key.BloodGroup)|| !string.IsNullOrEmpty(key.Occupation))
            {
                presentData = true; 
            }
            if (!string.IsNullOrEmpty(key.Religious)||!string.IsNullOrEmpty(key.MaritalStatus)||!string.IsNullOrEmpty(key.MotherOccupation)||!string.IsNullOrEmpty(key.FatherOccupation)) { presentData = true; }
            if (!string.IsNullOrEmpty(key.Gender)|| key.MinYearlySalary>0||key.MaxYearlySalary>0|| key.MinHeight>0||key.MaxHeight>0) {  presentData = true; }  
            // Query profiles based on key criteria
            var profiles = await _profileRepository.FindAsync(p =>
                ((string.IsNullOrEmpty(key.SkinTon) || (p.SkinTone != null && p.SkinTone.ToLower().Contains(key.SkinTon.ToLower()))) &&
                (string.IsNullOrEmpty(key.BloodGroup) || (p.BloodGroup != null && p.BloodGroup.ToLower().Contains(key.BloodGroup.ToLower()))) &&
                (string.IsNullOrEmpty(key.Occupation) || (p.Occupation != null && p.Occupation.ToLower().Contains(key.Occupation.ToLower()))) &&
                (string.IsNullOrEmpty(key.Religious) || (p.Religion != null && p.Religion.ToLower().Contains(key.Religious.ToLower()))) &&
                (string.IsNullOrEmpty(key.MaritalStatus) || (p.MaritalStatus != null && p.MaritalStatus.ToLower().Contains(key.MaritalStatus.ToLower()))) &&
                (string.IsNullOrEmpty(key.MotherOccupation) || (p.MotherOccupationDetails != null && p.MotherOccupationDetails.ToLower().Contains(key.MotherOccupation.ToLower()))) &&
                (string.IsNullOrEmpty(key.FatherOccupation) || (p.FatherOccupationDetails != null && p.FatherOccupationDetails.ToLower().Contains(key.FatherOccupation.ToLower()))) &&
                (string.IsNullOrEmpty(key.Gender) || (p.Gender != null && p.Gender.ToLower().Contains(key.Gender.ToLower()))) &&
                (!maxDateOfBirth.HasValue || (p.DateOfBirth != null && p.DateOfBirth <= maxDateOfBirth)) &&
                (!minDateOfBirth.HasValue || (p.DateOfBirth != null && p.DateOfBirth >= minDateOfBirth)) &&
                (key.MinYearlySalary<=0||key.MinYearlySalary>0&& p.YearlySalary >= key.MinYearlySalary) &&
                (key.MaxYearlySalary<=0||key.MaxYearlySalary>0&& p.YearlySalary <= key.MaxYearlySalary) &&
                (key.MinHeight<=0||key.MinHeight>0&& p.Height >= key.MinHeight) &&
                (key.MaxHeight<=0||key.MaxHeight>0 &&p.Height <= key.MaxHeight) &&
                (p.CanReciteQuranProperly == key.CanReciteQuranProperly))&&(p.UserId!=userId)
            );

            if (!string.IsNullOrEmpty(key.LocalAddress) || !string.IsNullOrEmpty(key.Thana)|| !string.IsNullOrEmpty(key.District)|| !string.IsNullOrEmpty(key.LocalAddress)|| !string.IsNullOrEmpty(key.Thana)|| !string.IsNullOrEmpty(key.District))
            {
                presentAddress = true;
            }


              //Query addresses based on address criteria
           var newProfiles = new List<Profile>();
            if (presentAddress)
            {
                var addressResults = await _addressRepository.FindAsync(a =>
                    ((string.IsNullOrEmpty(key.LocalAddress) || (!string.IsNullOrEmpty(key.LocalAddress) && a.CurrentAddress.localAddress != null && a.CurrentAddress.localAddress.ToLower().Contains(key.LocalAddress.ToLower()))&&
                    (string.IsNullOrEmpty(key.Thana) || !string.IsNullOrEmpty(key.Thana) && a.CurrentAddress.Thana != null && a.CurrentAddress.Thana.ToLower().Contains(key.Thana.ToLower()))&&
                    (string.IsNullOrEmpty(key.District) || !string.IsNullOrEmpty(key.District) && a.CurrentAddress.District != null && a.CurrentAddress.District.ToLower().Contains(key.District.ToLower())))||
                    (string.IsNullOrEmpty(key.LocalAddress) || (!string.IsNullOrEmpty(key.LocalAddress) && a.PermanentAddress.localAddress != null && a.PermanentAddress.localAddress.ToLower().Contains(key.LocalAddress.ToLower()))&&
                    (string.IsNullOrEmpty(key.Thana) || !string.IsNullOrEmpty(key.Thana) && a.PermanentAddress.Thana != null && a.PermanentAddress.Thana.ToLower().Contains(key.Thana.ToLower())) &&
                    (string.IsNullOrEmpty(key.District) || !string.IsNullOrEmpty(key.District) && a.PermanentAddress.District != null &&
                    a.PermanentAddress.District.ToLower().Contains(key.District.ToLower()))))
                    &&(a.UserId!=userId));

                if (addressResults.Any())
                {
                    var profileIds = addressResults
                        .Where(a => a.ProfileId != null)
                        .Select(a => a.ProfileId)
                        .Distinct()
                        .ToList();

                    var addressProfiles = await _profileRepository.FindAsync(p => profileIds.Contains(p.Id.ToString()));
                    if (addressProfiles != null)
                    {
                        newProfiles = addressProfiles.ToList();
                    }
                }
            }
            if (presentData && presentAddress)
            {
                return profiles.Intersect(newProfiles).ToList();
            }
            if (presentAddress)
            {
                return newProfiles;
            }
            return profiles?.ToList()?? new List<Profile>();
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
