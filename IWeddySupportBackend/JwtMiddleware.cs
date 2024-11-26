
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace IWeddySupport
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
      
        public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
        {
           
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["token"].FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrWhiteSpace(token))
            {
                AttachUserToContext(context, token);

            }
            // Check if the user is authenticated
            if (context.User?.Identity?.IsAuthenticated != true)
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("User not authenticated.");
                return; // Exit to prevent further processing
            }

            // Proceed to the next middleware in the pipeline

            await _next(context);

        }

        private void AttachUserToContext(HttpContext context, string token)
        {
           
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:AppSecret"]);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                // Safely retrieve claims
                var userId = jwtToken.Claims.FirstOrDefault(x => x.Type == "Id")?.Value;
                var username = jwtToken.Claims.FirstOrDefault(x => x.Type == "Email")?.Value
                              ?? jwtToken.Claims.FirstOrDefault(x => x.Type == "UserName")?.Value
                              ?? jwtToken.Claims.FirstOrDefault(x => x.Type == "PhoneNumber")?.Value;

                if (userId != null || username != null)
                {
                    var identity = new ClaimsIdentity(jwtToken.Claims, "jwt");
                    var principal = new ClaimsPrincipal(identity);
                    context.User = principal;

                }
            }
            catch (Exception ex)
            {
                // Return an Unauthorized response with the error message
                context.Response.StatusCode = 401; // Unauthorized
                context.Response.WriteAsync($"Token validation failed: {ex.Message}");
            }
        }
    }
}

