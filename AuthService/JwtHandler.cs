using Auth.Shared;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace AuthService
{
    public interface IJwtHandler
    {
        TokenResult CreateToken(AuthModel claims);
        SecurityToken? Validate(string token);
        TokenResult Revoke(string refreshToken);
    }
    public class JwtHandler : IJwtHandler
    {
        private readonly RsaKey _rsaKey;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IJwtRsa _jwtRsa;
        public JwtHandler(RsaKey rsaKey, JwtRsa jwtRsa, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _rsaKey = rsaKey;
            _jwtRsa = jwtRsa;
            _logger = loggerFactory.CreateLogger<JwtHandler>();
        }
        public TokenResult CreateToken(AuthModel auth)
        {
                            
            var unixTimeSeconds = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
            var claims = new Claim[] {
                    new Claim(JwtRegisteredClaimNames.Iat, unixTimeSeconds.ToString(), ClaimValueTypes.Integer64),
                    new Claim(nameof(auth.Email), auth.Email),
                    new Claim(nameof(auth.IsDomain), $"{auth.IsDomain}")
            };

            var section = _configuration.GetSection("ExternalClientServer");
            var token = _jwtRsa.CreateToken(_rsaKey.PrivateKey, claims , audience: section?["Audience"]??null, issuer: section?["Issuer"]??null);

            //TODO Save token and refresh token in DB

            return new TokenResult()
            {
                Token = token,
                RefreshToken = Guid.NewGuid().ToString("N")
            };
        }

        public TokenResult Revoke(string refreshToken)
        {
            throw new NotImplementedException();
        }

        public SecurityToken? Validate(string token)
        {
            var section = _configuration.GetSection("ExternalClientServer");
            try
            {
                return  _jwtRsa.ValidateToken(token, _rsaKey.PublicKey, audience: section?["Audience"]??null, issuer: section?["Issuer"]??null);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.Message);
                return null;
            }
            
        }
    }
}
