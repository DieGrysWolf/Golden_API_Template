using API.Configuration;
using API.DataServer;
using API.Entities.Models.DTOs;
using API.Entities.Models.DTOs.Generic;
using API.Entities.Models.DTOs.Requests;
using API.Entities.Models.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace API.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class AccountController : BaseController
    {
        private readonly JwtConfig _jwtConfig;
        private readonly TokenValidationParameters _tokenValidationParams;

        public AccountController(
            IMapper mapper,
            UserManager<IdentityUser> userManager,
            IOptionsMonitor<JwtConfig> optionsMonitor,
            TokenValidationParameters TokenValidationParams,
            IUnitOfWork unitOfWork) : base(mapper, unitOfWork, userManager)
        {
            _userManager = userManager;
            _jwtConfig = optionsMonitor.CurrentValue;
            _tokenValidationParams = TokenValidationParams;
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
                    UserName = user.Email
                };

                var userInfo = new UserInfoModel
                {
                    EmailAddress = user.Email,
                    Alive = true,
                    DateJoined = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,
                    IdentityId = new Guid(newUser.Id)                    
                };

                var isCreated = await _userManager.CreateAsync(newUser, user.Password);

                if (isCreated.Succeeded)
                {
                    // Create entry in UserData also
                    await _unitOfWork.UserRepo.AddAsync(userInfo);
                    await _unitOfWork.CompleteAsync();

                    var jwtToken = await GenerateJwtToken(newUser);

                    return Ok(jwtToken);
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

                var jwtToken = await GenerateJwtToken(existingUser);

                return Ok(jwtToken);
            }

            return BadRequest(new UserResponseDTO()
            {
                Errors = new List<string>()
                        { "Invalid Payload" },
                Success = false
            });
        }

        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDTO tokenRequest)
        {
            if (ModelState.IsValid)
            {
                var result = await ValidateAndGenerateToken(tokenRequest);

                if (result == null)
                {
                    return BadRequest(new UserResponseDTO()
                    {
                        Errors = new List<string>()
                        { "Invalid tokens" },
                        Success = false
                    });
                }                    

                return Ok(result);
            }

            //ToDo: make own badrequest response model
            return BadRequest(new UserResponseDTO()
            {
                Errors = new List<string>()
                        { "Invalid Payload" },
                Success = false
            });
        }

        [HttpDelete]
        [Route("Delete")]
        [Authorize]
        public async Task<IActionResult> DeleteAccount()
        {
            IdentityUser user = await _userManager.GetUserAsync(HttpContext.User);

            if(user == null)
                return NotFound();

            //start of profile stuff
            var userProfile = await _unitOfWork.UserRepo.GetByIdentityIdAsync(new Guid(user.Id));
            
            if(userProfile == null)
                return BadRequest("No profile found");

            userProfile.Alive = false;

            await _unitOfWork.UserRepo.UpdateAsync(userProfile);
            await _unitOfWork.CompleteAsync();
            //end of profile stuff

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
                return Ok();

            return BadRequest("Account not removed");
        }

        private async Task<TokenDataDTO> GenerateJwtToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            // Descriptor reads/writes values from claims
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // get new token using refresh token and jti
                }),
                Expires = DateTime.UtcNow.Add(_jwtConfig.ExpiryTimeFrame),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                Used = false,
                Revoked = false,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                Token = RandomString(35) + Guid.NewGuid()
            };

            await _unitOfWork.RefreshTokenRepo.AddAsync(refreshToken);
            await _unitOfWork.CompleteAsync();

            return new TokenDataDTO()
            {
                Token = jwtToken,
                RefreshToken = refreshToken.Token
            };
        }

        private async Task<AuthResult> ValidateAndGenerateToken(TokenRequestDTO tokenRequest)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                // Validation 1 - Validation JWT token format
                var tokenInVerification = jwtTokenHandler.ValidateToken(tokenRequest.Token, _tokenValidationParams, out var validatedToken);

                // Validation 2 - Validate encryption alg
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

                    if (result == false)
                        return null;
                }

                // Validation 3 - validate expiry date
                var utcExpiryDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expiryDate = UnixToDateTime(utcExpiryDate);

                if (expiryDate > DateTime.UtcNow)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Token has not yet expired"
                        }
                    };
                }

                // validation 4 - validate existence of the token
                var storedToken = await _unitOfWork.RefreshTokenRepo.GetByRefreshTokenAsync(tokenRequest.RefreshToken);

                if (storedToken == null)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Token is invalid"
                        }
                    };
                }

                // Validation 5 - validate if used
                if (storedToken.Used)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Token has been used"
                        }
                    };
                }

                // Validation 6 - validate if revoked
                if (storedToken.Revoked)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Token has been revoked"
                        }
                    };
                }

                // Validation 7 - validate the id
                var jti = tokenInVerification.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

                if (storedToken.JwtId != jti)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Token doesn't match"
                        }
                    };
                }

                // update current token 
                storedToken.Used = true;
                var updated = await _unitOfWork.RefreshTokenRepo.UpdateAsync(storedToken);
                await _unitOfWork.CompleteAsync();

                // Generate a new token
                if (!updated)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "An error occurred"
                        }
                    };
                }                

                var dbUser = await _userManager.FindByIdAsync(storedToken.UserId);

                if(dbUser == null)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "No matching user found"
                        }
                    };
                }

                // Success result
                var tokens = await GenerateJwtToken(dbUser);
                return new AuthResult
                {
                    Success = true,
                    Token = tokens.Token,
                    RefreshToken = tokens.RefreshToken
                };

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Lifetime validation failed. The token is expired."))
                {

                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Token has expired please re-login"
                        }
                    };

                }
                else
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Something went wrong."
                        }
                    };
                }
            }
        }

        private DateTime UnixToDateTime(long unix)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unix).ToUniversalTime();

            return dateTime;
        }

        private string RandomString(int length)
        {
            var random = new Random();
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            return new string(Enumerable.Repeat(chars, length)
                .Select(x => x[random.Next(x.Length)]).ToArray());
        }
    }
}
