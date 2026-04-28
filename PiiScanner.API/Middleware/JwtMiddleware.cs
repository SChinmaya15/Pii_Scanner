using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace PiiScanner.API.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _secret;
        private readonly ILogger<JwtMiddleware>? _logger;

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<JwtMiddleware>? logger = null)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _secret = configuration?["Jwt:Key"] ?? "ChangeThisDemoSecretToAStrongKey";
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Allow anonymous access to login endpoint
            var path = context.Request.Path;
            if (path.HasValue && path.Value.StartsWith("/api/users/login", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            // Allow swagger and open endpoints
            if (path.HasValue && (path.Value.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase) || path.Value.StartsWith("/swagger/index.html", StringComparison.OrdinalIgnoreCase)))
            {
                await _next(context);
                return;
            }

            var authHeader = context.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authHeader))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized: missing Authorization header");
                return;
            }

            // Extract token robustly
            var token = authHeader.Trim();
            // header may be like: Bearer <token>
            if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = token.Substring("Bearer ".Length).Trim();
            }

            // Remove surrounding quotes if present: some clients include quotes
            if (token.Length >=2 && token.StartsWith("\"") && token.EndsWith("\""))
            {
                token = token.Trim('"');
            }

            // URL-decode in case the token was encoded
            try { token = Uri.UnescapeDataString(token); } catch { /* ignore */ }

            // Quick validation: JWT compact serialization must be three parts separated by '.'
            if (token.Count(c => c == '.') !=2)
            {
                _logger?.LogWarning("Malformed token received: does not contain3 segments.");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid token format");
                return;
            }

            if (string.IsNullOrEmpty(token))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var keyBytes = Encoding.UTF8.GetBytes(_secret);
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                // Optionally ensure token uses expected algorithm
                if (validatedToken is JwtSecurityToken jwt && !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
                {
                    _logger?.LogWarning("Unexpected token algorithm: {alg}", jwt.Header.Alg);
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }

                // Attach user principal to HttpContext so controllers can use [Authorize] or HttpContext.User
                context.User = principal;

                await _next(context);
            }
            catch (SecurityTokenException ex)
            {
                _logger?.LogWarning(ex, "Token validation failed");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid token");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Unexpected error validating token");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
            }
        }
    }
}
