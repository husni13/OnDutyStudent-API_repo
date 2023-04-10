using API_dezurni_ucenik.Configurations;
using API_dezurni_ucenik.Data;
using API_dezurni_ucenik.Models;
using API_dezurni_ucenik.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;



namespace API_dezurni_ucenik.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        private readonly IConfiguration _configuration;

        // private readonly JwtConfig _jwtConfig;

        public AuthenticationController(
            //JwtConfig config,
            UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            // _jwtConfig= config;
            _userManager = userManager;
            _configuration = configuration;
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Secret").Value);

            //token descriptor

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())
                }),
                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = new SigningCredentials(new  SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDTO requestDto)
        {
            //Validate incoming request
            if(ModelState.IsValid)
            {
                //check if e-mail exists
                var userExist = await _userManager.FindByEmailAsync(requestDto.Email);

                if(userExist != null)
                {
                    return BadRequest(new AuthResult()
                    {
                        ResultStatus = false,
                        Errors= new List<string>()
                        {
                            "Email already exists"
                        }
                    });
                }

                //creating the user

                var newUser = new IdentityUser()
                {
                    Email = requestDto.Email,
                    UserName = requestDto.Email,
                };

                var isCreated = await _userManager.CreateAsync(newUser, requestDto.Password);

                if(isCreated.Succeeded)
                {
                    //Generate token
                    var token = GenerateJwtToken(newUser);

                    return Ok(new AuthResult()
                    {
                        Errors = new List<string>()
                        {
                            "User Registered"
                        },
                        ResultStatus = true,
                        Token = token

                    });
                }

                return BadRequest(new AuthResult()
                {
                    Errors = new List<string>()
                    {
                        "Server error",
                    },
                    ResultStatus = false
                });
            }

            return BadRequest();
        }

        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDTO loginRequest)
        {
            if (ModelState.IsValid)
            {
                //check if user exists
                var existingUser = await _userManager.FindByEmailAsync(loginRequest.Email);

                if(existingUser == null) 
                {
                    return BadRequest(new AuthResult()
                    {
                        ResultStatus= false,
                        Errors= new List<string>()
                        {
                            "User doesen't exist"
                        }
                    });
                }

                var isCorrect = await _userManager.CheckPasswordAsync(existingUser, loginRequest.Password);

                if (!isCorrect)
                {
                    return BadRequest(new AuthResult()
                    {
                        ResultStatus= false,
                        Errors= new List<string>()
                        {
                            "Invalid credentials"
                        }
                    });
                }

                var jwtToken = GenerateJwtToken(existingUser);

                return Ok(new AuthResult()
                {
                    ResultStatus = true,
                    Token = jwtToken,
                });
            }

            return BadRequest(new AuthResult()
            {
                ResultStatus = false,
                Errors= new List<string>()
                {
                    "Invalid payload"
                }
            });
        }
    }
}
