using API.Configuration.Messages;
using API.Entities.Models.DTOs.Generic;
using API.Entities.Models.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.v1.UserManagement
{
    [Authorize]
    public class UserController : BaseController
    {
        public UserController(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            UserManager<IdentityUser> userManager)
            : base(mapper, unitOfWork, userManager)
        {
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _unitOfWork.UserRepo.GetAllAsync();

            if (users == null)
            {
                var error = new ResultDTO<UserInfoModel>()
                {
                    Error = PopulateError(404, ErrorMessages.NotFound.ProfileNotFound, ErrorMessages.Types.NotFound)
                };
                return NotFound(error);
            }

            var result = new PagedResultDTO<UserInfoModel>()
            {
                Content = users.ToList(),
                ResultCount = users.Count()
            };

            return Ok(result);
        }

        [HttpGet]
        [Route("GetAllDead")]
        public async Task<IActionResult> GetAllDead()
        {
            var users = await _unitOfWork.UserRepo.GetAllDeadAsync();

            if (users == null)
            {
                var error = new ResultDTO<UserInfoModel>()
                {
                    Error = PopulateError(404, ErrorMessages.NotFound.ProfileNotFound, ErrorMessages.Types.NotFound)
                };
                return NotFound(error);
            }

            var result = new PagedResultDTO<UserInfoModel>()
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
                var error = new ResultDTO<UserInfoModel>()
                {
                    Error = PopulateError(404, ErrorMessages.NotFound.ProfileNotFound, ErrorMessages.Types.NotFound)
                };
                return NotFound(error);
            }

            var result = new ResultDTO<UserInfoModel>()
            {
                Content = user
            };

            return Ok(result);
        }

        [HttpGet]
        [Route("GetByEmail")]
        public async Task<IActionResult> GetByEmail([FromQuery] string email)
        {
            var user = await _unitOfWork.UserRepo.GetUserByEmail(email);

            if (user == null)
            {
                var error = new ResultDTO<UserInfoModel>
                {
                    Error = PopulateError(404, ErrorMessages.NotFound.ProfileNotFound, ErrorMessages.Types.NotFound)
                };
                return NotFound(error);
            }

            var result = new ResultDTO<UserInfoModel> { Content = user };
            return Ok(result);
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update([FromBody]UserInfoRequestDTO user)
        {
            var _mappedUser = _mapper.Map<UserInfoModel>(user);

            /* var _user = new UserInfoModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailAddress = user.EmailAddress,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                DateOfBirth = user.DateOfBirth
            }; */

            await _unitOfWork.UserRepo.UpdateAsync(_mappedUser);
            await _unitOfWork.CompleteAsync();

            // Comply with REST API
            return CreatedAtRoute("GetById", new { id = _mappedUser.Id }, user);
        }

        // ToDo - make an update status only endpoint
        [HttpPut]
        [Route("UndoDeleteById")]
        public async Task<IActionResult> UndoDeleteById([FromQuery]Guid id)
        {
            var user = await _unitOfWork.UserRepo.GetByIdAsync(id);

            user.Alive = true;

            await _unitOfWork.UserRepo.UpdateAsync(user);
            await _unitOfWork.CompleteAsync();

            return Ok();
        }

        // This endpoint is written but has no current use
        /*
        [HttpPost]
        public async Task<IActionResult> CreateNewUser([FromBody]UserInfoRequestDTO user)
        {
            var mappedUser = _mapper.Map<UserInfoModel>(user);

            await _unitOfWork.UserRepo.AddAsync(mappedUser);
            await _unitOfWork.CompleteAsync();

            var result = new ResultDTO<UserInfoRequestDTO>
            {
                Content = user
            };

            return CreatedAtRoute("GetById", new { id = mappedUser.Id }, result);
        }
        */

        [HttpDelete]
        [Route("DeleteById")]
        public async Task<IActionResult> DeleteById([FromQuery] Guid id)
        {
            var user = await _unitOfWork.UserRepo.GetByIdAsync(id);

            user.Alive = false;

            await _unitOfWork.UserRepo.UpdateAsync(user);
            await _unitOfWork.CompleteAsync();

            return Ok();
        }
    }
}
