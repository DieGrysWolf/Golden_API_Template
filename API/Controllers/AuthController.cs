using API.Configuration;
using API.Entities.Models.DTOs.Requests;
using API.Entities.Models.DTOs.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig;

        public AuthController(UserManager<IdentityUser> userManager, IOptionsMonitor<JwtConfig> optionsMonitor)
        {
            _userManager = userManager;
            _jwtConfig = optionsMonitor.CurrentValue;
        }

        //Registration
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDTO user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(user.Email);

                if (existingUser != null)
                    return BadRequest(new UserResponseDTO()
                    {
                        Errors = new List<string>()
                            { "Email already in use" },
                        Success = false
                    });

                var newUser = new IdentityUser()
                {
                    Email = user.Email,
                    UserName = user.Username
                };

                var isCreated = await _userManager.CreateAsync(newUser, user.Password);

                if (isCreated.Succeeded)
                {
                    var jwtToken = GenerateJwtToken(newUser);

                    return Ok(new UserResponseDTO() { Success = true, Token = jwtToken });
                }
                else
                {
                    return BadRequest(new UserResponseDTO()
                    {
                        Errors = isCreated.Errors.Select(x => x.Description).ToList(),
                        Success = false
                    });
                }
            }

            return BadRequest(new UserResponseDTO()
            {
                Errors = new List<string>()
                        { "Invalid Payload" },
                Success = false
            });
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            // Descriptor reads/writes values from claims
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // get new token using refresh token and jti
                }),
                Expires = DateTime.UtcNow.AddHours(6),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;
        }

        //Login
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDTO user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(user.Email);

                if (existingUser == null)
                {
                    return BadRequest(new UserResponseDTO()
                    {
                        Errors = new List<string>()
                        { "Invalid Login Request" },
                        Success = false
                    });
                }

                var isCorrect = await _userManager.CheckPasswordAsync(existingUser, user.Password);

                if (!isCorrect)
                {
                    return BadRequest(new UserResponseDTO()
                    {
                        Errors = new List<string>()
                        { "Invalid Login Request" },
                        Success = false
                    });
                }

                var jwtToken = GenerateJwtToken(existingUser);

                return Ok(new UserResponseDTO() { Success = true, Token = jwtToken });
            }

            return BadRequest(new UserResponseDTO()
            {
                Errors = new List<string>()
                        { "Invalid Payload" },
                Success = false
            });
        }
    }
}
