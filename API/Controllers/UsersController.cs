using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUnitOfWork _unitOfWork;

        public UsersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _unitOfWork.UserRepo.GetAllAsync();

            if(users == null)
                return NotFound();

            return Ok(users);
        }

        [HttpGet]
        [Route("GetById", Name = "GetById")]
        public async Task<IActionResult> GetById([FromQuery]Guid id)
        {
            var user = await _unitOfWork.UserRepo.GetByIdAsync(id);

            if(user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPut("UpdateById")]
        public async Task<IActionResult> UpdateById([FromBody]UserModel user)
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
        public async Task<IActionResult> DeleteById([FromQuery] Guid id)
        {
            await _unitOfWork.UserRepo.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();

            return Ok("Deletion succesful");
        }
    }
}
