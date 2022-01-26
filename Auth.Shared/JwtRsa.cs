using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Auth.Shared
{
    public interface IJwtRsa
    {
        string CreateToken(byte[] privateKey, IEnumerable<Claim> claims, string? audience = null, string? issuer = null, int expiresMinute = 60);
        SecurityToken ValidateToken(string token, byte[] publicKey, string? audience = null, string? issuer = null);
        TokenValidationParameters GetTokenValidationParameters(byte[] publicKey, string? audience = null, string? issuer = null);

    }
    public class JwtRsa : IJwtRsa
    {
        public string CreateToken(byte[] privateKey, IEnumerable<Claim> claims,  string? audience=null, string? issuer=null, int expiresMinute=60)
        {
            using RSA rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(source:privateKey, bytesRead: out _);

            var signingCredentials = new SigningCredentials(
                key: new RsaSecurityKey(rsa),
                algorithm: SecurityAlgorithms.RsaSha256
            );            
            
            var now = DateTime.Now;
            var jwt = new JwtSecurityToken(
                audience: audience,
                issuer: issuer,
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(expiresMinute),
                signingCredentials: signingCredentials
            );
            string token = new JwtSecurityTokenHandler().WriteToken(jwt);
            return token;            
        }
        public SecurityToken ValidateToken(string token, byte[] publicKey, string? audience = null, string? issuer = null)
        {
            var validationParameters = GetTokenValidationParameters(publicKey, audience, issuer);
            var handler = new JwtSecurityTokenHandler();
            handler.ValidateToken(token, validationParameters, out var securityToken);
            return securityToken;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="publicKey">Key as Base64String</param>
        /// <param name="audience"></param>
        /// <param name="issuer"></param>
        /// <returns></returns>
        public TokenValidationParameters GetTokenValidationParameters(string publicKey, string? audience = null, string? issuer = null)
        {
            return GetTokenValidationParameters(Convert.FromBase64String(publicKey), audience, issuer);
        }
        public TokenValidationParameters GetTokenValidationParameters(byte[] publicKey, string? audience = null, string? issuer = null)
        {
            RSA rsa = RSA.Create();
            rsa.ImportRSAPublicKey(source:publicKey, bytesRead: out _);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = issuer != null,
                ValidateAudience = audience != null,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new RsaSecurityKey(rsa)               
            };
            return validationParameters;
        }
    }
}
