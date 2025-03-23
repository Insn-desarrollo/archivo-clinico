using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace INSN.ArchivoClinico.Presentation.Util.Base
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Verifica si la cookie con el token está presente
            if (context.Request.Cookies.TryGetValue("AuthToken", out var token))
            {
                // Agrega el token al encabezado Authorization como Bearer Token
                context.Request.Headers.Add("Authorization", $"Bearer {token}");
            }

            await _next(context);
        }
    }
}
