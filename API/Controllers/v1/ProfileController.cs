using API.Configuration.Messages;
using API.Entities.Models.DTOs.Errors;
using API.Entities.Models.DTOs.Generic;
using API.Entities.Models.DTOs.Requests.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.v1
{
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
            var result = new ResultDTO<UserModel>();

            var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);

            if (loggedInUser == null)
            {
                result = new ResultDTO<UserModel>()
                {
                    Error = PopulateError(404, ErrorMessages.NotFound.AccountNotFound, ErrorMessages.Types.NotFound)
                };
                return NotFound(result);
            }

            var identityId = new Guid(loggedInUser.Id);

            var profile = await _unitOfWork.UserRepo.GetByIdentityIdAsync(identityId);

            profile.IdentityId = identityId;

            if (profile == null)
            {
                result = new ResultDTO<UserModel>()
                {
                    Error = PopulateError(404, ErrorMessages.NotFound.ProfileNotFound, ErrorMessages.Types.NotFound)
                };
                return NotFound(result);
            }

            result = new ResultDTO<UserModel>()
            {
                Content = profile
            };
            return Ok(result);
        }

        // Update user profile
        [HttpPut]
        [Route("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromBody]UpdateProfileRequestDTO profile)
        {
            var result = new ResultDTO<UserModel>();

            if (!ModelState.IsValid)
            {
                result = new ResultDTO<UserModel>()
                {
                    Error = PopulateError(400, ErrorMessages.Generic.InvalidBody, ErrorMessages.Types.BadRequest)
                };
                return BadRequest(result);
            }

            var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);

            if (loggedInUser == null)
            {
                result = new ResultDTO<UserModel>()
                {
                    Error = PopulateError(404, ErrorMessages.NotFound.AccountNotFound, ErrorMessages.Types.NotFound)
                };
                return NotFound(result);
            }

            var identityId = new Guid(loggedInUser.Id);
            var userProfile = await _unitOfWork.UserRepo.GetByIdentityIdAsync(identityId);

            if (userProfile == null)
            {
                result = new ResultDTO<UserModel>()
                {
                    Error = PopulateError(404, ErrorMessages.NotFound.ProfileNotFound, ErrorMessages.Types.NotFound)
                };
                return NotFound(result);
            }

            userProfile.FirstName = profile.FirstName;
            userProfile.LastName = profile.LastName;
            userProfile.PhoneNumber = profile.PhoneNumber;
            userProfile.Address = profile.Address;
            userProfile.DateOfBirth = profile.DateOfBirth;

            var updated = await _unitOfWork.UserRepo.UpdateUserProfileAsync(userProfile);

            if (!updated)
            {
                result = new ResultDTO<UserModel>()
                {
                    Error = PopulateError(503, ErrorMessages.Generic.RequestFailed, ErrorMessages.Types.BadRequest)
                };
                return BadRequest(result);
            }
                
            await _unitOfWork.CompleteAsync();

            result = new ResultDTO<UserModel>()
            {
                Content = userProfile
            };
            return Ok(result);
        }
    }
}
