global using AutoMapper;
using API.Entities.Models.DTOs.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class BaseController : ControllerBase
    {
        public IUnitOfWork _unitOfWork;
        public UserManager<IdentityUser> _userManager;
        public readonly IMapper _mapper;

        public BaseController(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            UserManager<IdentityUser> userManager)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        internal ErrorDTO PopulateError(int code, string message, string type)
        {
            return new ErrorDTO
            {
                Code = code,
                Message = message,
                Type = type
            };
        }
    }
}
