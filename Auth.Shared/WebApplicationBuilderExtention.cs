using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Auth.Shared
{
    public record RsaKey(byte[] PrivateKey, byte[] PublicKey);
    public static class WebApplicationBuilderExtention
    {
        public static void AddRsaKey(this IServiceCollection service)
        {
            //service.AddSingleton<RsaSecurityKey>(provider => {
            //    // It's required to register the RSA key with depedency injection.
            //    // If you don't do this, the RSA instance will be prematurely disposed.

            //    RSA rsa = RSA.Create();
            //    rsa.ImportRSAPublicKey(
            //        source: Convert.FromBase64String(configuration["Jwt:Asymmetric:PublicKey"]),
            //        bytesRead: out int _
            //    );

            //    return new RsaSecurityKey(rsa);
            //});

            service.AddScoped<JwtRsa>();
            using RSA rsa = RSA.Create();
            var key = new RsaKey(PrivateKey: rsa.ExportRSAPrivateKey(), PublicKey: rsa.ExportRSAPublicKey());
            service.AddSingleton(key);

        }
        public static void AddRsa(this IServiceCollection service)
        {
            service.AddScoped<JwtRsa>();            
        }
    }
}
