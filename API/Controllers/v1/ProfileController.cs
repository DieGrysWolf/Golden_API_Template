using API.Entities.Models.DTOs.Requests.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.v1
{
    [Authorize]
    public class ProfileController : BaseController
    {
        public ProfileController(
            IUnitOfWork unitOfWork,
            UserManager<IdentityUser> userManager) 
            : base(unitOfWork, userManager)
        {

        }

        [HttpGet]
        [Route("GetProfile")]
        public async Task<IActionResult> GetProfile()
        {
            var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);

            if(loggedInUser == null)
                return NotFound("User not found");

            var identityId = new Guid(loggedInUser.Id);

            var profile = await _unitOfWork.UserRepo.GetByIdentityIdAsync(identityId);

            if (profile == null)
                return NotFound("Profile not found");

            return Ok(profile);
        }

        // Update user profile
        [HttpPut]
        [Route("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromBody]UpdateProfileRequestDTO profile)
        {
            if(!ModelState.IsValid)
                return BadRequest("Invalid Payload");

            var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);

            if (loggedInUser == null)
                return NotFound("User not found");

            var identityId = new Guid(loggedInUser.Id);
            var userProfile = await _unitOfWork.UserRepo.GetByIdentityIdAsync(identityId);

            if (userProfile == null)
                return NotFound("Profile not found");

            userProfile.FirstName = profile.FirstName;
            userProfile.LastName = profile.LastName;
            userProfile.PhoneNumber = profile.PhoneNumber;
            userProfile.Address = profile.Address;
            userProfile.DateOfBirth = profile.DateOfBirth;

            var updated = await _unitOfWork.UserRepo.UpdateUserProfileAsync(userProfile);

            if (!updated)
            {
                return BadRequest("Something went wrong");
            }
                
            await _unitOfWork.CompleteAsync();
            return Ok(userProfile);
        }
    }
}
