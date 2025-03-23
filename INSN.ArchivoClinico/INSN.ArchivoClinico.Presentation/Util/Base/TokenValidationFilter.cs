using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace INSN.ArchivoClinico.Presentation.Util.Base
{
    public class TokenValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var token = context.HttpContext.Request.Cookies["AuthToken"];

            if (string.IsNullOrEmpty(token) || IsTokenExpired(token))
            {
                // Redirige al Login del controlador Account
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            await next(); // Continúa con la ejecución del método si el token es válido
        }

        private bool IsTokenExpired(string token)
        {
            try
            {
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
                if (expClaim != null && long.TryParse(expClaim, out var expTimestamp))
                {
                    var expirationDate = DateTimeOffset.FromUnixTimeSeconds(expTimestamp).UtcDateTime;
                    return expirationDate < DateTime.UtcNow;
                }

                return true; // Si no tiene "exp", considéralo vencido
            }
            catch
            {
                return true; // Si no se puede analizar, considéralo inválido
            }
        }
    }

}
