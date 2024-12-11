using IWeddySupport.Model;
using IWeddySupport.Repository;
using IWeddySupport.Service;
using IWeddySupport.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace IWeddySupport.Controller
{
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserProfileController: ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IProfilePhotoRepository _profilePhotoRepository;   
        public UserProfileController(IUserService userService,IProfilePhotoRepository profilePhotoRepository)
        {
            _userService = userService;
            _profilePhotoRepository = profilePhotoRepository;   
        }

  
        [HttpGet("getAllProfile")]
        public async Task<IActionResult> GetAllProfiles()
        {
            // Retrieve user from context
            var user = HttpContext.User;
            // Optionally retrieve user ID if needed
            var userId = user.FindFirst("Id")?.Value;
            var email = user.FindFirst("Email")?.Value;
            var profiles = await _userService.GetAllProfilesAsync(userId);
            return Ok(profiles);
        }

  
        [HttpGet("getSingleProfile")]
        public async Task<IActionResult> GetProfileById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Profile ID cannot be null or empty.");
            }

            var profile = await _userService.GetProfileByIdAsync(id);
            if (profile == null)
            {
                return NotFound($"Profile with ID {id} not found.");
            }

            return Ok(profile);
        }
     
        [HttpPost("addProfile")]
        public async Task<IActionResult> CreateProfile([FromBody] ProfileViewModel profile)
        {
            // Validate the incoming model
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Invalid profile data.",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }
            // Retrieve user from context
            var user = HttpContext.User;
            // Optionally retrieve user ID if needed
            var userId = user.FindFirst("Id")?.Value;
            var email = user.FindFirst("Email")?.Value;
           
            try
            {
                // Map the ProfileViewModel to a Profile entity
                var newProfile = new Profile
                {
                    Id=Guid.NewGuid(),
                    CreatedDate = DateTime.Now, 
                    UserId =userId,
                    isPublic = profile.isPublic,
                    SkinTone= profile.SkinTone, 
                    FullName = profile.FullName,
                    DateOfBirth = profile.DateOfBirth,
                    Gender = profile.Gender,
                    PhoneNumber = profile.PhoneNumber,
                    Email = profile.Email,
                    Nationality = profile.Nationality,
                    Religion = profile.Religion,
                    Occupation = profile.Occupation,
                    YearlySalary = profile.YearlySalary,
                    CompanyOrInstituteName = profile.CompanyOrInstituteName,
                    MaritalStatus = profile.MaritalStatus,
                    Declaration = profile.Declaration,
                    Height = profile.Height,
                    Weight = profile.Weight,
                    BloodGroup = profile.BloodGroup,
                    PrayerHabit = profile.PrayerHabit,
                    CanReciteQuranProperly = profile.CanReciteQuranProperly,
                    HasMentalOrPhysicalIllness = profile.HasMentalOrPhysicalIllness,
                    IsFatherAlive = profile.IsFatherAlive,
                    FatherOccupationDetails = profile.FatherOccupationDetails,
                    IsMotherAlive = profile.IsMotherAlive,
                    MotherOccupationDetails = profile.MotherOccupationDetails,
                    NumberOfBrothers = profile.NumberOfBrothers,
                    NumberOfSisters = profile.NumberOfSisters,
                    FamilyDetails = profile.FamilyDetails,
                    FamilyEconomicsCondition = profile.FamilyEconomicsCondition,
                    FamilyReligiousEnvironment = profile.FamilyReligiousEnvironment
                };

                // Create the profile using the service
                var createdProfile = await _userService.CreateProfileAsync(newProfile);

                // Return 201 Created with the created profile data
                return CreatedAtAction(nameof(GetProfileById), new { id = createdProfile.UserId }, createdProfile);
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                return StatusCode(500, new
                {
                    message = "An error occurred while creating the profile.",
                    error = ex.Message
                });
            }
        }


        [HttpPut("updateProfile")]
        public async Task<IActionResult> UpdateProfile([FromBody] ProfileUpdateViewModel profile)
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Check if the profile exists
                var existingProfile = await _userService.GetProfileByIdAsync(profile.Id);
                if (existingProfile == null)
                {
                    return BadRequest("No such profile exists!");
                }
               
                // Map the data from ProfileUpdateViewModel to the existing Profile entity
                existingProfile.BloodGroup = profile.BloodGroup;
                existingProfile.CanReciteQuranProperly = profile.CanReciteQuranProperly;
                existingProfile.PhoneNumber = profile.PhoneNumber;
                existingProfile.Occupation = profile.Occupation;
                existingProfile.YearlySalary = profile.YearlySalary;
                existingProfile.SkinTone= profile.SkinTone; 
                existingProfile.CompanyOrInstituteName = profile.CompanyOrInstituteName;
                existingProfile.MaritalStatus = profile.MaritalStatus;
                existingProfile.Declaration = profile.Declaration;
                existingProfile.Height = profile.Height;
                existingProfile.Weight = profile.Weight;
                existingProfile.PrayerHabit = profile.PrayerHabit;
                existingProfile.HasMentalOrPhysicalIllness = profile.HasMentalOrPhysicalIllness;
                existingProfile.IsFatherAlive = profile.IsFatherAlive;
                existingProfile.FatherOccupationDetails = profile.FatherOccupationDetails;
                existingProfile.IsMotherAlive = profile.IsMotherAlive;
                existingProfile.MotherOccupationDetails = profile.MotherOccupationDetails;
                existingProfile.NumberOfBrothers = profile.NumberOfBrothers;
                existingProfile.NumberOfSisters = profile.NumberOfSisters;
                existingProfile.FamilyDetails = profile.FamilyDetails;
                existingProfile.FamilyEconomicsCondition = profile.FamilyEconomicsCondition;
                existingProfile.FamilyReligiousEnvironment = profile.FamilyReligiousEnvironment;
                existingProfile.isPublic=profile.isPublic;
                existingProfile.UpdatedDate = DateTime.Now;
                // Update the profile using the service
                var updatedProfile = await _userService.UpdateProfileAsync(existingProfile);

                return Ok(updatedProfile);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Generic exception handling
                return StatusCode(500, new { message = "An error occurred while updating the profile.", error = ex.Message });
            }
        }


        [HttpDelete("deleteProfile")]
       public async Task<IActionResult> DeleteProfile(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Profile ID cannot be null or empty.");
            }

            try
            {
                var isDeleted = await _userService.DeleteProfileAsync(id);
                if (isDeleted==null)
                {
                    return StatusCode(500, "Failed to delete the profile.");
                }

                return Ok(isDeleted);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
        
       [HttpGet("getAllAddress")]
        public async Task<IActionResult> GetAllAddress()
        {
            // Retrieve user from context
            var user = HttpContext.User;
            // Optionally retrieve user ID if needed
            var userId = user.FindFirst("Id")?.Value;
            var email = user.FindFirst("Email")?.Value;
            var addresses = await _userService.GetAllAddressesAsync(userId);
            
            return Ok(addresses);
        }

        [HttpPost("addAddress")]
        public async Task<IActionResult> CreateAddress([FromBody] AddressViewModel addressViewModel)
        {
            // Validate the incoming model
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Invalid address data.",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }
            // Retrieve user from context
            var user = HttpContext.User;
            // Optionally retrieve user ID if needed
            var userId = user.FindFirst("Id")?.Value;
            var email = user.FindFirst("Email")?.Value;
            try
            {
                // Map the AddressViewModel to an Address entity
                var newAddress = new Address
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    UserId = userId,
                    ProfileId = addressViewModel.ProfileId,
                    PermanentAddress = new AddressDetail
                    {
                        localAddress = addressViewModel.PermanentAddress.LocalAddress,
                        Thana = addressViewModel.PermanentAddress.Thana,
                        District = addressViewModel.PermanentAddress.District
                    },
                    CurrentAddress = new AddressDetail
                    {
                        localAddress = addressViewModel.CurrentAddress.LocalAddress,
                        Thana = addressViewModel.CurrentAddress.Thana,
                        District = addressViewModel.CurrentAddress.District
                    }
                };

                // Create the address using the service
                var createdAddress = await _userService.CreateAddressAsync(newAddress);

                // Return 201 Created with the created address data
                return Ok(createdAddress);  
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                return StatusCode(500, new
                {
                    message = "An error occurred while creating the address.",
                    error = ex.Message
                });
            }
        }


        [HttpPut("updateAddress")]
        public async Task<IActionResult> UpdateAddress([FromBody] UpdateAddressViewModel addressViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var existedAddress = await _userService.GetAddressAsync(addressViewModel.Id);
            if (existedAddress == null) { return BadRequest("No such address existed!"); }
            existedAddress.CurrentAddress.localAddress = addressViewModel.CurrentAddress.LocalAddress;
            existedAddress.CurrentAddress.Thana = addressViewModel.CurrentAddress.Thana;
            existedAddress.CurrentAddress.District = addressViewModel.CurrentAddress.District;
            existedAddress.UpdatedDate = DateTime.Now;
            var updatedAddress = await _userService.UpdateAddressAsync(existedAddress);
            
            return Ok(updatedAddress);
        }

        [HttpDelete("deleteAddress")]
        public async Task<IActionResult> DeleteAddress(string Id)
        {
            var result = await _userService.DeleteAddressAsync(Id);
           
            return Ok(result);
        }

        [HttpGet("getAllPhotos")]
        public async Task<IActionResult> GetAllProfilePhotos()
        {
            try
            {
                // Retrieve user from context
                var user = HttpContext.User;
                // Optionally retrieve user ID if needed
                var userId = user.FindFirst("Id")?.Value;
                var email = user.FindFirst("Email")?.Value;
                var photos = await _userService.GetAllProfilePhotoAsync(userId);
                return Ok(photos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while fetching profile photos.",
                    error = ex.Message
                });
            }
        }

        // POST: api/ProfilePhoto/addPhoto
        [HttpPost("uploadProfilePhoto")]
        public async Task<IActionResult> UploadProfilePhotoAsync(IFormFile file, string userId, string? profileId = null)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "No file uploaded or file is empty." });
            }

            // Validate file extension
            var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".ico", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest(new { message = "Invalid file type. Only .png, .jpg, .jpeg, .ico files are allowed." });
            }

            try
            {
                // Ensure the upload directory exists
                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Save the photo to the file system
                string fileName = Guid.NewGuid() + fileExtension; // Generate unique file name
                string filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Create a new profile photo record
                var profilePhoto = new ProfilePhoto
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ProfileId = profileId,
                    FileName = fileName,
                    FilePath = Path.Combine("uploads", fileName),
                    FileSize = file.Length,
                    UploadedAt = DateTime.UtcNow,
                    CreatedDate = DateTime.UtcNow,
                };

                // Save the photo details in the database
                var addedProfilePhoto=await _userService.CreateProfilePhotoAsync(profilePhoto);
                // Return success response with the photo details
                return Ok(new
                {
                    message = "File uploaded successfully.",
                    addedProfilePhoto
                });
            }
            catch (Exception ex)
            {
                // Log the exception (assuming a logger is configured)

                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An internal server error occurred while uploading the profile photo.",
                    error = ex.Message
                });
            }
        }

        // PUT: api/ProfilePhoto/updateProfilePhoto
        [HttpPut("updateProfilePhoto")]
        public async Task<IActionResult> UpdateProfilePhotoAsync(IFormFile file, string photoId)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "No file uploaded or the file is empty." });
            }

            if (string.IsNullOrWhiteSpace(photoId))
            {
                return BadRequest(new { message = "Photo ID is required." });
            }

            // Validate file extension
            var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".ico",".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest(new { message = "Invalid file type. Only .png, .jpg, .jpeg, .ico files are allowed." });
            }

            try
            {
                // Fetch the existing profile photo record using the provided photo ID
                var existingPhoto = await _userService.GetProfilePhotoAsync(photoId);
                if (existingPhoto == null)
                {
                    return NotFound(new { message = "Profile photo not found." });
                }

                // Ensure the upload directory exists
                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Save the new photo to the file system
                string newFileName = Guid.NewGuid() + fileExtension; // Generate a unique name
                string newFilePath = Path.Combine(uploadPath, newFileName);

                using (var stream = new FileStream(newFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Delete the old photo from the file system (optional)
                string oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), existingPhoto.FilePath);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                // Update the profile photo details
                existingPhoto.FileName = newFileName;
                existingPhoto.FilePath = Path.Combine("uploads", newFileName);
                existingPhoto.FileSize = file.Length;
                existingPhoto.UpdatedDate = DateTime.UtcNow;

                // Save the updated record to the database
                await _profilePhotoRepository.UpdateAsync(existingPhoto);

                // Return the updated photo details
                return Ok(new
                {
                    message = "Profile photo updated successfully.",
                    photo = existingPhoto
                });
            }
            catch (Exception ex)
            {
                // Log the exception (if a logger is configured)

                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An error occurred while updating the profile photo.",
                    error = ex.Message
                });
            }
        }

        // DELETE: api/ProfilePhoto/deletePhoto
        [HttpDelete("deleteProfilePhoto")]
        public async Task<IActionResult> DeleteProfilePhotoAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new { message = "Photo ID is required." });
            }

            try
            {
                // Fetch the existing photo record
                var existingPhoto = await _userService.GetProfilePhotoAsync(id);
                if (existingPhoto == null)
                {
                    return NotFound(new { message = "Profile photo not found." });
                }

                // Delete the file from the file system
                string fullFilePath = Path.Combine(Directory.GetCurrentDirectory(), existingPhoto.FilePath);
                if (System.IO.File.Exists(fullFilePath))
                {
                    System.IO.File.Delete(fullFilePath);
                }

                // Delete the photo record from the database
                await _userService.DeleteProfilePhotoAsync(id);

                // Return a success response
                return Ok(new { message = "Profile photo deleted successfully." });
            }
            catch (Exception ex)
            {
                // Log the exception (if a logger is configured)

                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An error occurred while deleting the profile photo.",
                    error = ex.Message
                });
            }
        }
        // GET: api/UserProfile/getAllUserProfiles
        [HttpGet("getAllUserProfiles")]
        public async Task<IActionResult> GetAllUserProfiles()
        {
            // Retrieve user from context
            var user = HttpContext.User;
            // Optionally retrieve user ID if needed
            var userId = user.FindFirst("Id")?.Value;
            var email = user.FindFirst("Email")?.Value;
            var userProfiles = await _userService.GetAllUserProfilesAsync(userId);
            return Ok(userProfiles);
        }

        // POST: api/UserProfile/addUserProfile
        [HttpPost("addUserProfile")]
        public async Task<IActionResult> CreateUserProfile([FromBody] UserProfileViewModel userProfileViewModel)
        {
            // Validate the incoming model
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Invalid user profile data.",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }
            // Retrieve user from context
            var user = HttpContext.User;
            // Optionally retrieve user ID if needed
            var userId = user.FindFirst("Id")?.Value;
            var email = user.FindFirst("Email")?.Value;
            try
            {
                // Map the UserProfileViewModel to a UserProfile entity
                var newUserProfile = new UserProfile
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    UserId = userId,
                    ProfileId = userProfileViewModel.ProfileId,
                    Relationship = userProfileViewModel.Relationship
                };

                // Create the user profile using the service
                var createdUserProfile = await _userService.CreateUserProfileAsync(newUserProfile);

                // Return 201 Created with the created user profile data
                return Ok(createdUserProfile);
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                return StatusCode(500, new
                {
                    message = "An error occurred while creating the user profile.",
                    error = ex.Message
                });
            }
        }

        // PUT: api/UserProfile/updateUserProfile
        [HttpPut("updateUserProfile")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateUserProfileViewModel userProfileViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Invalid user profile data.",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            try
            {
                var existingUserProfile = await _userService.GetUserProfileAsync(userProfileViewModel.Id);
                if (existingUserProfile == null)
                {
                    return NotFound("User profile not found.");
                }

                // Update the entity with new values
                existingUserProfile.UpdatedDate = DateTime.Now; 
                existingUserProfile.Relationship = userProfileViewModel.Relationship;
                var updatedUserProfile = await _userService.UpdateUserProfileAsync(existingUserProfile);
                return Ok(updatedUserProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while updating the user profile.",
                    error = ex.Message
                });
            }
        }

        // DELETE: api/UserProfile/deleteUserProfile
        [HttpDelete("deleteUserProfile")]
        public async Task<IActionResult> DeleteUserProfile(string Id)
        {
            try
            {
                var deletedUserProfile = await _userService.DeleteUserProfileAsync(Id);
                if (deletedUserProfile == null)
                {
                    return NotFound("User profile not found.");
                }

                return Ok(deletedUserProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while deleting the user profile.",
                    error = ex.Message
                });
            }
        }
        // Get all expected partners
        [HttpGet("getAllExpectedPartner")]
        public async Task<IActionResult> GetAllExpectedPartners()
        {
            // Retrieve user from context
            var user = HttpContext.User;
            // Optionally retrieve user ID if needed
            var userId = user.FindFirst("Id")?.Value;
            var email = user.FindFirst("Email")?.Value;
            var partners = await _userService.GetAllExpectedPartnersAsync(userId);
            return Ok(partners);
        }

        // Get a specific expected partner by UserId
        [HttpPost("getExpectedPartnerBySearchKey")]
        public async Task<IActionResult> GetExpectedPartner([FromBody] SearchKeyViewModel keyViewModel)
        {
            var partners = await _userService.GetExpectedPartnersByKeyAsync(keyViewModel);
            if (partners == null)
            {
                return NotFound("Partner preferences not found for the given UserId.");
            }

            return Ok(partners);
        }

        // Create a new expected partner
        [HttpPost("addExpectedPartner")]
        public async Task<IActionResult> CreateExpectedPartner([FromBody] ExpectedPartnerViewModel expectedPartnerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Invalid partner data.",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }
            // Retrieve user from context
            var user = HttpContext.User;
            // Optionally retrieve user ID if needed
            var userId = user.FindFirst("Id")?.Value;
            var email = user.FindFirst("Email")?.Value;
            try
            {
                // Map the view model to the entity
                var newPartner = new PartnerExpectation
                {
                    Id=Guid.NewGuid(),
                    CreatedDate= DateTime.Now,  
                    UserId = userId,
                    ProfileId = expectedPartnerViewModel.ProfileId,
                    MinAge = expectedPartnerViewModel.MinAge,
                    MaxAge = expectedPartnerViewModel.MaxAge,
                    SkinTone = expectedPartnerViewModel.SkinTone,
                    MinHeight = expectedPartnerViewModel.MinHeight,
                    MaxHeight = expectedPartnerViewModel.MaxHeight,
                    EducationalQualification = expectedPartnerViewModel.EducationalQualification,
                    PreferredDistricts = expectedPartnerViewModel.PreferredDistricts,
                    MaritalStatus = expectedPartnerViewModel.MaritalStatus,
                    PreferredProfessions = expectedPartnerViewModel.PreferredProfessions,
                    FinancialStatus = expectedPartnerViewModel.FinancialStatus,
                    DesiredCharacteristics = expectedPartnerViewModel.DesiredCharacteristics
                };

                var createdPartner = await _userService.CreateExpectedPartnerAsync(newPartner);
                return Ok(createdPartner);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the partner preference.", error = ex.Message });
            }
        }

        // Update an existing expected partner
        [HttpPut("updateExpectedPartner")]
        public async Task<IActionResult> UpdateExpectedPartner([FromBody] UpdateExpectedPartnerViewModel expectedPartnerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingPartner = await _userService.GetExpectedPartnerAsync(expectedPartnerViewModel.Id);
                if (existingPartner == null)
                {
                    return NotFound("Partner preferences not found for the given UserId.");
                }

                // Map the view model to the entity for update
                existingPartner.UpdatedDate= DateTime.Now;  
                existingPartner.MinAge = expectedPartnerViewModel.MinAge;
                existingPartner.MaxAge = expectedPartnerViewModel.MaxAge;
                existingPartner.SkinTone = expectedPartnerViewModel.SkinTone;
                existingPartner.MinHeight = expectedPartnerViewModel.MinHeight;
                existingPartner.MaxHeight = expectedPartnerViewModel.MaxHeight;
                existingPartner.EducationalQualification = expectedPartnerViewModel.EducationalQualification;
                existingPartner.PreferredDistricts = expectedPartnerViewModel.PreferredDistricts;
                existingPartner.MaritalStatus = expectedPartnerViewModel.MaritalStatus;
                existingPartner.PreferredProfessions = expectedPartnerViewModel.PreferredProfessions;
                existingPartner.FinancialStatus = expectedPartnerViewModel.FinancialStatus;
                existingPartner.DesiredCharacteristics = expectedPartnerViewModel.DesiredCharacteristics;
                var updatedPartner = await _userService.UpdateExpectedPartnerAsync(existingPartner);
                return Ok(updatedPartner);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the partner preference.", error = ex.Message });
            }
        }

        // Delete a specific expected partner by UserId
        [HttpDelete("deleteExpectedPartner")]
        public async Task<IActionResult> DeleteExpectedPartner(string Id)
        {
            var deletedPartner = await _userService.DeleteExpectedPartnerAsync(Id);
            if (deletedPartner == null)
            {
                return NotFound("Partner preferences not found for the given UserId.");
            }

            return Ok(new { message = "Partner preference deleted successfully.", data = deletedPartner });
        }


    }
    public class SearchKeyViewModel
    {
        public string SkinTon { get; set; }
        public string BloodGroup { get; set; }
        public string Occupation { get; set; }
        public string Religious { get; set; }
        public string MaritalStatus { get; set; }
        public string MotherOccupation { get; set; }
        public string FatherOccupation { get; set; }
        public string Gender { get; set; }
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public int MinHeight { get; set; }
        public int MaxHeight { get; set; }
        public int MinYearlySalary {  get; set; }  
        public int MaxYearlySalary { get; set; }
        public string LocalAddress {  get; set; }   
        public string Thana { get; set; }   
        public string District {  get; set; } 
        public bool CanReciteQuranProperly {  get; set; }   

    }



  

}
