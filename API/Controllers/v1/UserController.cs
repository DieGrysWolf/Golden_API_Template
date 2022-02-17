using API.Configuration.Messages;
using API.Entities.Models.DTOs.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.v1
{
    public class UserController : BaseController
    {
        public UserController(
            IUnitOfWork unitOfWork,
            UserManager<IdentityUser> userManager)
            : base(unitOfWork, userManager)
        {
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _unitOfWork.UserRepo.GetAllAsync();

            if (users == null)
            {
                var error = new ResultDTO<UserModel>()
                {
                    Error = PopulateError(404, ErrorMessages.NotFound.ProfileNotFound, ErrorMessages.Types.NotFound)
                };
                return NotFound(error);
            }

            var result = new PagedResultDTO<UserModel>()
            {
                Content = users.ToList(),
                ResultCount = users.Count()
            };

            return Ok(result);
        }

        [HttpGet]
        [Route("GetById", Name = "GetById")]
        public async Task<IActionResult> GetById([FromQuery]Guid id)
        {
            var user = await _unitOfWork.UserRepo.GetByIdAsync(id);

            if (user == null)
            {
                var error = new ResultDTO<UserModel>()
                {
                    Error = PopulateError(404, ErrorMessages.NotFound.ProfileNotFound, ErrorMessages.Types.NotFound)
                };
                return NotFound(error);
            }

            var result = new ResultDTO<UserModel>()
            {
                Content = user
            };

            return Ok(result);
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update([FromBody]UserModel user)
        {
            var _user = new UserModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailAddress = user.EmailAddress,
                Address = user.Address,
                Alive = user.Alive,
                DateJoined = user.DateJoined,
                DateOfBirth = user.DateOfBirth,
                DateUpdated = user.DateUpdated,
                Id = user.Id,
                PhoneNumber = user.PhoneNumber
            };

            await _unitOfWork.UserRepo.UpdateAsync(_user);
            await _unitOfWork.CompleteAsync();

            // Comply with REST API
            return CreatedAtRoute("GetById", new { id = _user.Id }, user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewUser([FromBody]UserModel user)
        {
            await _unitOfWork.UserRepo.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            return CreatedAtRoute("GetById", user.Id, user);
        }

        [HttpDelete]
        [Route("DeleteById")]
        public async Task<IActionResult> DeleteById([FromQuery] Guid id)
        {
            await _unitOfWork.UserRepo.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();

            return Ok("Deletion succesful");
        }
    }
}
