using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace CatalogService.API.Security;

public static class AuthExtensions
{
    public static IServiceCollection AddAuthSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication( options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer( options =>
        {
            var rsa = RSA.Create();
            rsa.ImportSubjectPublicKeyInfo(
                source: Convert.FromBase64String(configuration["JwtConfig:Key"]!),
                bytesRead: out int _);

            options.Authority = configuration["JwtConfig:Authority"]!;
            options.RequireHttpsMetadata = false;

            options.TokenValidationParameters = new()
            {
                ValidAudience = configuration["JwtConfig:Audience"]!,
                ValidIssuer = configuration["JwtConfig:Issuer"]!,
                IssuerSigningKey = new RsaSecurityKey(rsa),

                ValidateLifetime = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
            };
        });

        services.AddAuthorization( cfg =>
        {
            cfg.AddPolicy(Permissions.CreateCategory, policy => policy.RequireScope(Scopes.CreateCategory));
            cfg.AddPolicy(Permissions.ReadCategory, policy => policy.RequireScope(Scopes.ReadCategory));
            cfg.AddPolicy(Permissions.UpdateCategory, policy => policy.RequireScope(Scopes.UpdateCategory));
            cfg.AddPolicy(Permissions.DeleteCategory, policy => policy.RequireScope(Scopes.DeleteCategory));
        });

        return services;
    }

    private static readonly IEnumerable<string> _scopeClaimTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
        "http://schemas.microsoft.com/identity/claims/scope",
        "scope"
    };

    public static AuthorizationPolicyBuilder RequireScope(this AuthorizationPolicyBuilder builder, params string[] scopes) => 
        builder.RequireAssertion(context =>
            context.User
                .Claims
                .Where(c => _scopeClaimTypes.Contains(c.Type))
                .SelectMany(c => c.Value.Split(' '))
                .Any(s => scopes.Contains(s, StringComparer.Ordinal))
        );
}
