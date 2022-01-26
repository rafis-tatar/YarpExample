using Auth.Shared;
using AuthService;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Auth.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly JwtHandler _jwtHandler;
        public AuthController(ILogger<AuthController> logger, JwtHandler jwtHandler)
        {
            _logger = logger;
            _jwtHandler = jwtHandler;
        }


        [HttpPost("token")] 
        public TokenResult GetToken([FromBody] Auth.Shared.AuthModel auth) => _jwtHandler.CreateToken(auth);

        [HttpGet("{token}/validate")]
        public IActionResult TokenValidate(string token)
        {
            var result = _jwtHandler.Validate(token);
            return result != null ? Ok(): BadRequest("Token is invalid.");
        }

        [HttpGet("{token}/info")]
        public IActionResult GetInfoByToken(string token)
        {
            var result = _jwtHandler.Validate(token);
            if (result == null) return BadRequest("Token is invalid.");

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            var model = new AuthModel
            {
                Email = jwtToken.Claims.First(clam => clam.Type == "Email").Value,
                IsDomain = bool.Parse(jwtToken.Claims.First(clam => clam.Type == "IsDomain").Value)
            };
            return Ok(model);
        }
        [HttpGet("open-key")]
        public string GetPublickKey([FromServices] RsaKey rsaKey)
        {
           var key= Convert.ToBase64String(rsaKey.PublicKey);
            return key;
        }
        
    }
}