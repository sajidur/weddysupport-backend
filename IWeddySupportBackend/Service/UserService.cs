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
        Task<IEnumerable<Profile>> GetExpectedPartnersByKeyAsync(string key);
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
        //public async Task<IEnumerable<Profile>> GetExpectedPartnersByKeyAsync(string key)
        //{
        //    // First search in the ProfileRepository
        //    var profiles = await _profileRepository.FindAsync(a =>
        //       a.SkinTone==key||
        //       a.BloodGroup == key ||
        //        a.Occupation == key ||
        //        a.Religion == key ||
        //        a.YearlySalary == key ||
        //        a.MaritalStatus == key ||
        //        a.MotherOccupationDetails == key ||
        //        a.Gender == key ||
        //        a.Height == key ||
        //        a.FatherOccupationDetails == key ||
        //        a.CompanyOrInstituteName == key);

        //    // If no profiles are found, search by address fields in the AddressRepository
        //    if (profiles == null || !profiles.Any())
        //    {
        //        var addressResults = await _addressRepository.FindAsync(a =>
        //            a.CurrentAddress.Thana == key ||
        //            a.CurrentAddress.localAddress == key ||
        //            a.CurrentAddress.District == key ||
        //            a.PermanentAddress.localAddress == key ||
        //            a.PermanentAddress.Thana == key ||
        //            a.PermanentAddress.District == key);

        //        // If address search returns results, get ProfileIds and search again in the ProfileRepository
        //        if (addressResults != null && addressResults.Any())
        //        {
        //            // Extract ProfileIds from addressResults (assuming Address object has a reference to ProfileId)
        //            var profileIds = addressResults.Select(a => a.ProfileId).ToList();

        //            // Initialize a list to hold the search results
        //            var profileSearchResults = new List<Profile>();

        //            // Loop through each ProfileId and search for the corresponding Profile
        //            foreach (var profileId in profileIds)
        //            {
        //                // Attempt to find the profile by ProfileId (use FirstOrDefaultAsync to get a single profile)
        //                var profile = await _profileRepository.FindAsync(p => p.Id ==Guid.Parse( profileId));

        //                if (profile != null)
        //                {
        //                    profileSearchResults.Add(profile.FirstOrDefault()); // Add the found profile to the results
        //                }
        //            }

        //            // Combine both sets of results (initial profile search + address-based profile search)
        //            profiles = profiles.Concat(profileSearchResults);
        //        }
        //    }

        //    return profiles ?? Enumerable.Empty<Profile>();
        //}
        public async Task<IEnumerable<Profile>> GetExpectedPartnersByKeyAsync(string key)
        {
            // Parse the input key to check if it's numeric
            bool isNumeric = int.TryParse(key, out int numericKey);

            // Initial profile query
            var profiles = await _profileRepository.FindAsync(p =>
                (!string.IsNullOrEmpty(p.SkinTone) && p.SkinTone.Contains(key, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(p.BloodGroup) && p.BloodGroup.Contains(key, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(p.Occupation) && p.Occupation.Contains(key, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(p.Religion) && p.Religion.Contains(key, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(p.MaritalStatus) && p.MaritalStatus.Contains(key, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(p.MotherOccupationDetails) && p.MotherOccupationDetails.Contains(key, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(p.Gender) && p.Gender.Contains(key, StringComparison.OrdinalIgnoreCase))
            );

            // If the key is numeric, add height, age, and salary-based filters
            if (isNumeric && profiles == null)
            {
                profiles = profiles.Where(p =>
                    (p.DateOfBirth != default(DateTime) &&
                     Math.Abs(DateTime.Now.Year - p.DateOfBirth.Year) >= numericKey - 2 &&
                     Math.Abs(DateTime.Now.Year - p.DateOfBirth.Year) <= numericKey + 2) ||
                    (TryParseSalary(p.YearlySalary, out var parsedSalary) &&
                     parsedSalary >= numericKey - 10000 && parsedSalary <= numericKey + 10000) ||
                    (TryParseHeight(p.Height, out var parsedHeight) &&
                     parsedHeight >= numericKey - 5 && parsedHeight <= numericKey + 5)
                ).ToList();
            }

            // Search in AddressRepository if profiles list is still null or empty
            if (profiles == null || !profiles.Any())
            {
                var addressResults = await _addressRepository.FindAsync(a =>
                    (!string.IsNullOrEmpty(a.CurrentAddress.Thana) && a.CurrentAddress.Thana.Contains(key, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(a.CurrentAddress.District) && a.CurrentAddress.District.Contains(key, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(a.PermanentAddress.Thana) && a.PermanentAddress.Thana.Contains(key, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(a.PermanentAddress.District) && a.PermanentAddress.District.Contains(key, StringComparison.OrdinalIgnoreCase))
                );

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

        private bool TryParseSalary(object yearlySalary, out int parsedSalary)
        {
            parsedSalary = 0;
            if (yearlySalary == null || string.IsNullOrWhiteSpace(yearlySalary.ToString()))
                return false;

            string salaryString = yearlySalary.ToString().Trim().ToLower();
            if (salaryString.EndsWith("k"))
                salaryString = salaryString.Substring(0, salaryString.Length - 1) + "000";

            return int.TryParse(salaryString, out parsedSalary);
        }

        private bool TryParseHeight(string height, out int parsedHeight)
        {
            parsedHeight = 0;
            if (string.IsNullOrWhiteSpace(height))
                return false;

            height = height.Trim();
            var pattern = @"(?:(\d+)\s*'(\d+)\s*\"")|(\d+)\s*feet\s*(\d+)\s*inches?|(\d+)\s*inches?";
            var match = Regex.Match(height, pattern, RegexOptions.IgnoreCase);

            if (!match.Success)
                return false;

            if (match.Groups[1].Success && match.Groups[2].Success)
            {
                parsedHeight = int.Parse(match.Groups[1].Value) * 12 + int.Parse(match.Groups[2].Value);
            }
            else if (match.Groups[3].Success && match.Groups[4].Success)
            {
                parsedHeight = int.Parse(match.Groups[3].Value) * 12 + int.Parse(match.Groups[4].Value);
            }
            else if (match.Groups[5].Success)
            {
                parsedHeight = int.Parse(match.Groups[5].Value);
            }

            return parsedHeight > 0;
        }

    }
}
