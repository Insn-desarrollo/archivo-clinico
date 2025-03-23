using INSN.ArchivoClinico.Models;
using INSN.ArchivoClinico.UtilFactory.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Text;


namespace INSN.ArchivoClinico.Controllers
{
    //[Authorize]
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountController> _logger;
        private readonly HttpClient _httpClient;

        public AccountController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<AccountController> logger, HttpClient httpClient)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClient;
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult ErrorLogin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var token = await AuthenticateUserAsync(model);
                if (token == null)
                {
                    ViewBag.ErrorMessage = "Error en la autenticación. Inténtalo de nuevo.";
                    return View(model);
                }

                var usuarioData = await ObtenerDatosUsuarioAsync(model.Usuario, token);
                if (usuarioData == null)
                {
                    ViewBag.ErrorMessage = "Error al obtener los datos del usuario.";
                    return View(model);
                }

                // Guardar datos en sesión (mejor que TempData)
                HttpContext.Session.SetString("AuthToken", token);
                HttpContext.Session.SetString("Usuario", model.Usuario);
                HttpContext.Session.SetString("NombreUsuario", usuarioData.musu_nom);

                return RedirectToAction("Index", "Menu");
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "Error al conectar con la API.");
                ViewBag.ErrorMessage = "Error de red. No se pudo conectar al servidor.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado durante la autenticación.");
                ViewBag.ErrorMessage = "Ocurrió un error inesperado. Inténtalo de nuevo.";
            }

            return View(model);
        }

        // Método para autenticar usuario y obtener token
        private async Task<string> AuthenticateUserAsync(LoginViewModel model)
        {
            var authUrl = $"{_configuration["ApiUrls:Autenticacion"]}/login";
            var httpClient = _httpClientFactory.CreateClient();

            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(authUrl, content);

            if (!response.IsSuccessStatusCode) return null;

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(responseContent);

            return result?.success == true ? result.token.ToString() : null;
        }

        // Método para obtener datos del usuario
        private async Task<UsuarioPagModel> ObtenerDatosUsuarioAsync(string usuario, string token)
        {
            var usuarioUrl = $"{_configuration["ApiUrls:Usuario"]}/GetUsuarioId?codigo={usuario}";
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.GetAsync(usuarioUrl);
            if (!response.IsSuccessStatusCode) return null;

            var userResponseContent = await response.Content.ReadAsStringAsync();
            var userResult = JsonConvert.DeserializeObject<StatusResponse<UsuarioPagModel>>(userResponseContent);

            return userResult?.Success == true ? userResult.Data : null;
        }

        public IActionResult Logout()
        {
            // Eliminar datos de la sesión
            HttpContext.Session.Clear();

            // Eliminar cookies de autenticación
            if (Request.Cookies.ContainsKey("AuthToken"))
            {
                Response.Cookies.Delete("AuthToken");
            }

            if (Request.Cookies.ContainsKey("Usuario"))
            {
                Response.Cookies.Delete("Usuario");
            }

            // Registrar el logout (si tienes un sistema de logs)
            _logger.LogInformation("Usuario cerró sesión exitosamente.");

            // Redirigir a la página de login
            return RedirectToAction("Login", "Account");
        }



        private string GetAuthToken()
        {
            var token = HttpContext.Session.GetString("AuthToken");

            if (string.IsNullOrEmpty(token) || TokenExpirado(token))
            {
                throw new UnauthorizedAccessException("Token de autenticación no disponible o expirado.");
            }

            return token;
        }

        // Método para validar si el token ha expirado
        private bool TokenExpirado(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jsonToken == null)
                return true;  

            var exp = jsonToken.ValidTo; 
            return exp < DateTime.UtcNow; 
        }

    }
}
