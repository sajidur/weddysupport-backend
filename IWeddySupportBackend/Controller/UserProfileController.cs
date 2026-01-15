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
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IProfilePhotoRepository _profilePhotoRepository;
        private readonly IFirebaseNotificationService _notificationService;


        public UserProfileController(IUserService userService, IFirebaseNotificationService notificationService,
            IProfilePhotoRepository profilePhotoRepository)
        {
            _userService = userService;
            _profilePhotoRepository = profilePhotoRepository;
            _notificationService = notificationService;
        }
        [HttpGet("GetAllData")]
        public async Task<IActionResult> GetAllData()
        {
            var profiles = await _userService.GetAllProfilesAsyncByDefault();
            var addresses = await _userService.GetAllAddressAsyncByDefault();
            var expectedPartners = await _userService.GetAllExpectedPartnersAsyncByDefault();
            var profilePhotos = await _userService.GetAllProfilePhotosAsyncByDefault();
            var userProfiles = await _userService.GetAllUserProfilesByDefault();
            var data = new
            {
                profiles = profiles,
                addresses = addresses,
                userProfiles = userProfiles,
                profilePhotos = profilePhotos,
                expectedPartners = expectedPartners

            };
            return Ok(data);
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
            var photos = await _userService.GetAllProfilePhotoAsync(userId);
            //Map profiles with photo URLs
            var profilesWithPhotos = profiles.Select(profile =>
             {
                 var matchingPhoto = photos.FirstOrDefault(photo => photo.ProfileId == profile.Id.ToString());
                 return new ProfileWithPhoto
                 {
                     Id = profile.Id.ToString(),
                     isPublic = profile.isPublic,
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
                     SkinTone = profile.SkinTone,
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
                     FamilyReligiousEnvironment = profile.FamilyReligiousEnvironment,
                     UserId = profile.UserId,
                     PhotoUrl = matchingPhoto?.PhotoUrl // Attach the photo URL
                 };
             }).ToList();

            return Ok(profilesWithPhotos);
        }

        [HttpGet("autoProfilesRequestedOrResponsed")]
        public async Task<IActionResult> autoProfilesRequestedOrResponsed()
        {
            // Retrieve user from context
            var user = HttpContext.User;
            // Optionally retrieve user ID if needed
            var userId = user.FindFirst("Id")?.Value;
            var email = user.FindFirst("Email")?.Value;
            var myProfiles = await _userService.GetAllProfilesAsync(userId);//take all of my created profile.
            var requestedProfiles = new List<RequestedForYouDataModel>();
            var Profile2 = new RequestedForYouDataModel();
            var expectedProfiles = new List<RequestedByYouDataModel>();
            var Profile1 = new RequestedByYouDataModel();
            foreach (var pro in myProfiles)
            {
                if (pro == null) continue;
                var profilesRequestedByMe = await _userService.GetAllRequestedProfileAsyncByMe(pro.Id.ToString());//whom i requested by me
                var profilesRequestedForMe = await _userService.GetAllRequestedProfileAsyncForMe(pro.Id.ToString());//whose are requested for me.
                foreach (var profile in profilesRequestedByMe)
                {
                    Profile1 = new RequestedByYouDataModel();
                    Profile1.ProfileId = pro.Id.ToString();
                    Profile1.ProfileName = pro.FullName;
                    Profile1.RequestRejected = profile.UserRequestRejected;
                    Profile1.RequestAccepted = profile.UserRequestAccepted;
                    Profile1.Message = profile.Message;
                    Profile1.ExpecterProfileId = profile.ExpacterProfileId;
                    Profile1.ExpecterUserId = profile.ExpacterUserId;
                    var expacterProfile = await _userService.GetProfileByIdAsync(profile.ExpacterProfileId);
                    if (expacterProfile != null)
                    {
                        Profile1.ExpecterName = expacterProfile.FullName;
                        Profile1.ExpecterEmail = expacterProfile.Email;
                    }
                    var photos = await _userService.GetProfilePhotoAsync(profile.ExpacterProfileId);
                    if (photos != null)
                    {
                        Profile1.ExpecterPhotoUrl = photos.PhotoUrl;
                    }
                    // Profile1.ExpecterUserId = expacterProfile.UserId;
                    expectedProfiles.Add(Profile1);
                }
                foreach (var profile in profilesRequestedForMe)
                {
                    Profile2 = new RequestedForYouDataModel();
                    if (profile == null) continue;
                    Profile2.ProfileId = pro.Id.ToString();
                    Profile2.ProfileName = pro.FullName;
                    Profile2.RequesterProfileId = profile.RequesterProfileId;
                    Profile2.RequesterUserId = profile.RequesterUserId;
                    var Profile = await _userService.GetProfileByIdAsync(profile.RequesterProfileId);
                    if (Profile != null)
                    {
                        Profile2.RequesterName = Profile.FullName;
                        Profile2.RequesterEmail = Profile.Email;
                    }
                    Profile2.UserRequestRejected = profile.UserRequestRejected;
                    Profile2.UserRequestAccepted = profile.UserRequestAccepted;
                    Profile2.Message = profile.Message;
                    var photos = await _userService.GetProfilePhotoAsync(profile.RequesterProfileId);
                    if (photos != null)
                    {
                        Profile2.RequesterPhotoUrl = photos.PhotoUrl;
                    }

                    requestedProfiles.Add(Profile2);
                }

            }


            return Ok(new
            {
                RequestedByYou = expectedProfiles,
                RequestedForYou = requestedProfiles
            });
        }

        [HttpPost("profileRequest")]
        public async Task<IActionResult> ProfileRequestAsync([FromBody] UserRequestViewModel usR)
        {
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
            var existedRequester = await _userService.GetProfileByIdAsync(usR.RequesterProfileId);
            if (existedRequester == null)
            {
                return BadRequest(new
                {
                    message = "No such Profile has existed for sending request",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });

            }
            var existedExpecter = await _userService.GetProfileByIdAsync(usR.ExpacterProfileId);
            if (existedRequester == null)
            {
                return BadRequest(new
                {
                    message = "No such Profile has existed to response for this request",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }
            var existedUserRequest = await _userService.GetUserRequestAsync(userId, usR.RequesterProfileId,usR.ExpacterProfileId);
            var accepterUserDevice = await _userService.GetUserDeviceByUserIdAsync(usR.ExpacterUserId, usR.ExpacterProfileId);
            if (existedUserRequest != null)
            {
                existedUserRequest.UpdatedDate = DateTime.Now;
                existedUserRequest.ApplicationStatus = usR.ApplicationStatus;
                if (existedUserRequest.UserRequestAccepted == "yes")
                {
                    // us.UserRequestRejected = "no";
                    existedUserRequest.Message = "User has already responsed with affarmative way";
                }
                else if (existedUserRequest.UserRequestRejected == "yes")
                {
                    //us.UserRequestAccepted = "no";
                    existedUserRequest.Message = "User has already responsed with negative way";
                }
                else
                {
                    existedUserRequest.Message = "Already sent this request but has no response yet!";
                }
                await _userService.UpdatedUserRequestAsync(existedUserRequest);
                // 🔔 Push Notification
                await _notificationService.SendAsync(
                    accepterUserDevice?.FCMToken,
                    usR.Title,
                    usR.Body,
                    new Dictionary<string, string>
                    {
        { "type", "PROFILE_REQUEST" },
        { "requestUserId", userId},
        { "requesterProfileId", usR.RequesterProfileId.ToString() }
                    });

                if (existedUserRequest.UserRequestAccepted == "yes")
                {
                    var profileData = await GetProfileById(existedUserRequest.ExpacterProfileId);

                    return Ok(new
                    {
                        DeviceToken = accepterUserDevice?.FCMToken,
                        UserRequest = existedUserRequest,
                        ProfileData = profileData
                    });
                }
                return Ok(new
                {
                    message = "UserRequest data is sent again successfully",
                    UserRequest = existedUserRequest
                });

            }
            var usr = new UserRequest
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                RequesterUserId = userId,
                RequesterProfileId = usR.RequesterProfileId,
                ExpacterProfileId = usR.ExpacterProfileId,
                ExpacterUserId = usR.ExpacterUserId,
                UserRequestAccepted = null,
                UserRequestRejected = null,
                ApplicationStatus = usR.ApplicationStatus
            };

            var ussR = await _userService.AddUserRequestAsync(usr);
            // 🔔 Push Notification
            await _notificationService.SendAsync(
                accepterUserDevice?.FCMToken,
                usR.Title,//title
                usR.Body,//body
                new Dictionary<string, string>
                {
        { "type", "PROFILE_REQUEST" },
        { "requestUserId", userId },
        { "requesterProfileId", usR.RequesterProfileId.ToString() }
                });
            return Ok(new
            {
                DeviceToken = accepterUserDevice?.FCMToken,
                message = "UserRequest data is sent successfully.",
                UserRequest = ussR
            });


        }

        [HttpPost("acceptOrReject")]
        public async Task<IActionResult> profileAcceptOrReject(ProfileAcceptOrReject res)
        {
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
            var HasAnyRequest = await _userService.GetRequestedProfileAsync(res.RequesterUserId, res.RequesterProfileId);
            if (HasAnyRequest == null)
            {
                return Ok(null);
            }
            HasAnyRequest.UpdatedDate = DateTime.UtcNow;
            HasAnyRequest.UserRequestRejected = res.RequestRejected;
            HasAnyRequest.UserRequestAccepted = res.RequestAccepted;
            //HasAnyRequest.ExpacterProfileId = res.MyProfileId;
            //HasAnyRequest.ExpacterUserId = userId;
            //HasAnyRequest.RequesterProfileId = res.RequesterProfileId;
            //HasAnyRequest.RequesterUserId = res.RequesterUserId;
            if (res.RequestAccepted == "yes")
            {
                HasAnyRequest.Message = "Accepted the request!";
                // 🔔 SEND NOTIFICATION ONLY IF ACCEPTED
                if (res.RequestAccepted == "yes")
                {
                    // Get accepter profile (to show name)
                    var accepterProfile = await _userService
                        .GetProfileByIdAsync(HasAnyRequest.ExpacterProfileId);

                    string accepterName = accepterProfile?.FullName ?? "the user";

                    // Get requester device
                    var requesterDevice = await _userService
                        .GetUserDeviceByUserIdAsync(HasAnyRequest.RequesterUserId, HasAnyRequest.RequesterProfileId);

                    if (!string.IsNullOrWhiteSpace(requesterDevice?.FCMToken))
                    {
                        await _notificationService.SendAsync(
                            requesterDevice.FCMToken,
                            "Profile Request Accepted",
                            $"Your request has been accepted by {accepterName}.",
                            new Dictionary<string, string>
                            {
                    { "type", "PROFILE_REQUEST_ACCEPTED" },
                                        { "requestUserId", HasAnyRequest.RequesterUserId },

                    { "requestProfileId", HasAnyRequest.RequesterProfileId }

                            });
                    }
                }
            }
            else if (res.RequestRejected == "yes")
            {
                HasAnyRequest.Message = "Rejected the request!";

            }
            else
            {
                HasAnyRequest.Message = "No response!";
            }
            if (res.RequestAccepted == "yes")
            {
                HasAnyRequest.UserRequestRejected = "no";
            }
            var updatedRequest = await _userService.UpdatedUserRequestAsync(HasAnyRequest);

            if (res.RequestRejected == "yes")
            {
                HasAnyRequest.UserRequestAccepted = "no";
                await _userService.DeleteUserRequest(HasAnyRequest);
            }
            //// Prepare additional data if accepted
            //if (res.RequestAccepted == "yes")
            //{
            //    var profileData = await GetProfileById(res.MyProfileId);

            //    return Ok(new
            //    {
            //        Request = updatedRequest,
            //        ProfileData = profileData
            //    });
            //}

            return Ok(new
            {
                 
                Request = updatedRequest
            });

        }

        [HttpPost("getResponse")]
        public async Task<IActionResult> GetResponseFromUser(ProfileResponse res)
        {
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
            var HasAnyResponse = await _userService.GetResponserProfileAsync(res.ResponserUserId, res.ResponserProfileId);
            if (HasAnyResponse == null)
            {
                return Ok(null);
            }
            HasAnyResponse.UpdatedDate = DateTime.UtcNow;
            // Prepare additional data if accepted
            if (HasAnyResponse.UserRequestAccepted == "yes")
            {
                var profileData = await GetProfileById(res.ResponserProfileId);

                return Ok(new
                {
                    Request = HasAnyResponse,
                    ProfileData = profileData
                });
            }

            return Ok(new { Request = HasAnyResponse });
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
            var photos = await _userService.GetProfilePhotoByProfileIdAsync(id);
            var address = await _userService.GetProfileAddressByProfileIdAsync(id);
            var userRelationship = await _userService.GetUserProfileByProfileIdAsync(id);
            var data = new
            {
                profile = profile,
                photo = photos?.FilePath ?? null,
                address = address,
                userRelationship = userRelationship?.Relationship ?? null,

            };
            return Ok(data);
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
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    UserId = userId,
                    isPublic = profile.isPublic,
                    SkinTone = profile.SkinTone,
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
                var createdProfile = await _userService.CreateOrGetProfileAsync(newProfile);

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

        [HttpPost("addOrUpdateDeviceToken")]
        public async Task<IActionResult> AddOrUpdateDeviceToken([FromBody] UserDeviceViewModel userDeviceViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Invalid device data.",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            try
            {
                // Retrieve user from context
                var user = HttpContext.User;
                // Optionally retrieve user ID if needed
                var userId = user.FindFirst("Id")?.Value;
                var profile = await _userService.GetProfileByUserIdAsync(userId, userDeviceViewModel.ProfileId);
                if (profile == null)
                {
                    return BadRequest(new
                    {
                        message = "No such profile has existed for this user",
                        errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }
                var existedDevice = await _userService.GetUserDeviceByUserIdAsync(userId, userDeviceViewModel.ProfileId);
                if (existedDevice != null)
                {
                    existedDevice.UpdatedDate = DateTime.Now;
                    existedDevice.DeviceType = userDeviceViewModel.DeviceType;
                    existedDevice.FCMToken = userDeviceViewModel.FCMToken;
                    existedDevice.DeviceId = userDeviceViewModel.DeviceId;
                    existedDevice.ProfileId = profile.Id.ToString();
                    var updatedDevice = await _userService.UpdateUserDeviceAsync(existedDevice);
                    return Ok(updatedDevice);
                }
                //var email = user.FindFirst("Email")?.Value;
                var userDevice = new UserDevice
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    UserId = userId,
                    FCMToken = userDeviceViewModel.FCMToken,
                    DeviceType = userDeviceViewModel.DeviceType,
                    ProfileId = profile.Id.ToString(),
                    CreatedBy = DateTime.Now,
                    DeviceId = userDeviceViewModel.DeviceId,
                    IsActive = true,
                    IsDeleted = false,
                    UpdatedDate = DateTime.Now,
                    UpdatedBy = DateTime.Now,


                };
                var result = await _userService.CreateUserDeviceAsync(userDevice);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                return StatusCode(500, new
                {
                    message = "An error occurred while saving the device token.",
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
                existingProfile.isPublic = profile.isPublic;

                existingProfile.FullName = profile.FullName;
                existingProfile.DateOfBirth = profile.DateOfBirth;
                existingProfile.Gender = profile.Gender;
                existingProfile.PhoneNumber = profile.PhoneNumber;
                existingProfile.Email = profile.Email;

                existingProfile.Nationality = profile.Nationality;
                existingProfile.Religion = profile.Religion;

                existingProfile.Occupation = profile.Occupation;
                existingProfile.YearlySalary = profile.YearlySalary;
                existingProfile.CompanyOrInstituteName = profile.CompanyOrInstituteName;

                existingProfile.MaritalStatus = profile.MaritalStatus;
                existingProfile.Declaration = profile.Declaration;

                existingProfile.SkinTone = profile.SkinTone;
                existingProfile.Height = profile.Height;
                existingProfile.Weight = profile.Weight;
                existingProfile.BloodGroup = profile.BloodGroup;

                existingProfile.PrayerHabit = profile.PrayerHabit;
                existingProfile.CanReciteQuranProperly = profile.CanReciteQuranProperly;
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

                existingProfile.UpdatedDate = DateTime.UtcNow;

                // Update the profile
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
        [HttpDelete("deleteUserRequest")]
        public async Task<IActionResult> deleteUserRequest(string expecterProfileId)
        {
            if (string.IsNullOrWhiteSpace(expecterProfileId))
            {
                return BadRequest("Profile ID cannot be null or empty.");
            }
            try
            {
                var user = HttpContext.User;
                // Optionally retrieve user ID if needed
                var userId = user.FindFirst("Id")?.Value;
                var existedRequest = await _userService.GetExpecterProfileRequest(userId, expecterProfileId);
                var isDeleted = await _userService.DeleteUserRequest(existedRequest);
                if (isDeleted == null)
                {
                    return StatusCode(500, "Failed to delete the user request.");
                }

                return Ok("Deleted data successfully!");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
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
                if (isDeleted == null)
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
            var existingAddress = await _userService.GetAddressByProfileIdAsync(userId, addressViewModel.ProfileId);
            if (existingAddress != null)
            {
                return Ok(new
                {
                    message = "Already existed address data.",
                    Address = existingAddress
                });
            }
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
                return BadRequest(new
                {
                    message = "Invalid address data.",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }
            var existedAddress = await _userService.GetAddressAsync(addressViewModel.Id);
            if (existedAddress == null)
            {
                return BadRequest(new
                {
                    message = "No such address existed!.",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }
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
        public async Task<IActionResult> UploadProfilePhotoAsync(IFormFile file, string profileId)
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
            // Retrieve user from context
            var user = HttpContext.User;
            // Optionally retrieve user ID if needed
            var userId = user.FindFirst("Id")?.Value;
            var existedPhoto = await _userService.GetProfilePhotoByIdAsync(userId, profileId);
            if (existedPhoto != null)
            {
                return Ok(new
                {
                    message = "Already present profilePhoto with this profile.",
                    Photo = existedPhoto
                });
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
                // Generate the public URL for the uploaded file
                string publicUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";


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
                    PhotoUrl = publicUrl
                };

                // Save the photo details in the database
                var addedProfilePhoto = await _userService.CreateProfilePhotoAsync(profilePhoto);
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
            var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".ico", ".gif" };
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
                // Generate the public URL for the uploaded file
                string publicUrl = $"{Request.Scheme}://{Request.Host}/uploads/{newFileName}";

                // Update the profile photo details
                existingPhoto.FileName = newFileName;
                existingPhoto.FilePath = Path.Combine("uploads", newFileName);
                existingPhoto.FileSize = file.Length;
                existingPhoto.UpdatedDate = DateTime.UtcNow;
                existingPhoto.PhotoUrl = publicUrl;
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
                return Ok(new
                {
                    message = "Profile photo deleted successfully.",
                    existingPhoto
                });
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
            var existedUserProfile = await _userService.GetUserProfileByProfileIdAsync(userId, userProfileViewModel.ProfileId);
            if (existedUserProfile != null)
            {
                return Ok(new
                {
                    message = "Already existed user profile data.",
                    UserProfile = existedUserProfile
                });
            }
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

            // Retrieve user from context
            var user = HttpContext.User;
            // Optionally retrieve user ID if needed
            var userId = user.FindFirst("Id")?.Value;
            var email = user.FindFirst("Email")?.Value;
            var partners = await _userService.GetExpectedPartnersByKeyAsync(keyViewModel, userId);
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
            var existedPartner = await _userService.GetExpectedPartnerByProfileIdAsync(userId, expectedPartnerViewModel.ProfileId);
            if (existedPartner != null)
            {
                return Ok(new
                {
                    message = "Already existed partner data.",
                    ExpectedPartner = existedPartner
                });
            }
            try
            {
                // Map the view model to the entity
                var newPartner = new PartnerExpectation
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
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
                existingPartner.UpdatedDate = DateTime.Now;
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
    public class ProfileWithPhoto
    {
        public string Id { get; set; }
        public bool isPublic { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Nationality { get; set; }
        public string Religion { get; set; }
        public string Occupation { get; set; } // "পেশা"
        public int YearlySalary { get; set; }
        public string CompanyOrInstituteName { get; set; }
        public string MaritalStatus { get; set; }
        public string Declaration { get; set; }
        public string SkinTone { get; set; } // Preferred skin tones (e.g., "শ্যামলা, উজ্জল শ্যামলা, ফর্সা")
                                             // Shamla (শ্যামলা): Dusky or wheatish complexion. Forsha(ফর্সা) : Fair complexion. Kala(কালা): Dark or black complexion.
        public int Height { get; set; }
        public string Weight { get; set; }
        public string BloodGroup { get; set; }
        // public string ClothingPreferenceOutside { get; set; } // "ঘরের বাহিরে সাধারণত কি ধরণের পোষাক পরেন?"
        public string PrayerHabit { get; set; } // "প্রতিদিন পাঁচ ওয়াক্ত নামাজ পড়েন কি? কবে থেকে পড়ছেন?"
        public bool CanReciteQuranProperly { get; set; } // "শুদ্ধভাবে কুরআন তিলওয়াত করতে পারেন?"
        public bool HasMentalOrPhysicalIllness { get; set; } // "আপনার মানসিক বা শারীরিক কোনো রোগ আছে?"
        public bool IsFatherAlive { get; set; } // "পিতা জীবিত?"
        public string FatherOccupationDetails { get; set; } // "পিতার পেশার বিস্তারিত বিবরণ"
        public bool IsMotherAlive { get; set; } // "মাতা জীবিত?"
        public string MotherOccupationDetails { get; set; } // "মাতার পেশার বিস্তারিত বিবরণ"
        public int NumberOfBrothers { get; set; } // "ভাইয়ের সংখ্যা"

        [Range(0, int.MaxValue, ErrorMessage = "Number of sisters must be a non-negative integer.")]
        public int NumberOfSisters { get; set; } // "বোনের সংখ্যা"
        public string FamilyDetails { get; set; }
        public string FamilyEconomicsCondition { get; set; } // "পরিবারের আর্থিক অবস্থা"
                                                             // public string FamilyEconomicsDetails { get; set; } // "পরিবারের আর্থিক অবস্থা বিস্তারিত"
        public string FamilyReligiousEnvironment { get; set; } // "পরিবারের ধর্মীয় পরিবেশ"
        public string UserId { get; set; }
        public string PhotoUrl { get; set; }
    }
    public class SearchKeyViewModel
    {
        public string? SkinTon { get; set; }
        public string? BloodGroup { get; set; }
        public string? Occupation { get; set; }
        public string? Religious { get; set; }
        public string MaritalStatus { get; set; }
        public string? MotherOccupation { get; set; }
        public string? FatherOccupation { get; set; }
        public string Gender { get; set; }
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
        public int? MinHeight { get; set; }
        public int? MaxHeight { get; set; }
        public int? MinYearlySalary { get; set; }
        public int? MaxYearlySalary { get; set; }
        public string? LocalAddress { get; set; }
        public string? Thana { get; set; }
        public string? District { get; set; }
        public bool? CanReciteQuranProperly { get; set; }

    }

    public class RequestedForYouDataModel
    {
        public string ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string RequesterProfileId { get; set; }
        public string RequesterName { get; set; }
        public string RequesterEmail { get; set; }
        public string RequesterPhotoUrl { get; set; }
        public string RequesterUserId { get; set; }
        public string? UserRequestAccepted { get; set; }
        public string? UserRequestRejected { get; set; }
        public string ApplicationStatus { get; set; }
        public string? Message { get; set; } = string.Empty;
    }
    public class RequestedByYouDataModel
    {
        public string ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string ExpecterProfileId { get; set; }
        public string ExpecterName { get; set; }
        public string ExpecterEmail { get; set; }
        public string ExpecterPhotoUrl { get; set; }
        public string ExpecterUserId { get; set; }
        public string RequestAccepted { get; set; }
        public string RequestRejected { get; set; }
        public string Message { get; set; }
    }

}
