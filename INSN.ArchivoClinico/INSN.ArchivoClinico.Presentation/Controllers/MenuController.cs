using INSN.ArchivoClinico.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using INSN.ArchivoClinico.Application.Interfaces;
using INSN.ArchivoClinico.Application.DTOs;
using INSN.ArchivoClinico.Infrastructure.Services;
using INSN.ArchivoClinico.Domain.Entities;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using INSN.ArchivoClinico.UtilFactory.Base;
using INSN.ArchivoClinico.Domain.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using INSN.ArchivoClinico.Presentation.Util.Base;
using System.IdentityModel.Tokens.Jwt;

namespace INSN.ArchivoClinico.Controllers
{
    public class MenuController : Controller
    {
        private readonly ILogger<MenuController> _logger;
        private readonly IHistoriasService _atencionAppService;
        private readonly IEvaluacionService _evaluacionAppService;        
        private readonly ICuentaService _cuentaAppService;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly AtencionHceService _atencionHceService;


        public MenuController(ILogger<MenuController> logger,
            IHistoriasService atencionAppService,
            ICuentaService cuentaAppService,
            IEvaluacionService evaluacionAppService,
            HttpClient httpClient, 
            IConfiguration configuration, 
            AtencionHceService atencionHceService)
        {
            _logger = logger;
            _atencionAppService = atencionAppService;
            _evaluacionAppService = evaluacionAppService;
            _httpClient = httpClient;
            _configuration = configuration;
            _atencionHceService = atencionHceService;
            _cuentaAppService = cuentaAppService;
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


        public async Task<IActionResult> CargarVista(string vista)
        {
            try
            {
                var token = GetAuthToken();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            
            var listaUsuarios = new List<UsuarioAdmin>
            {                
                new UsuarioAdmin { Usuario = "Todos", Nombre = "Todos" },
                new UsuarioAdmin { Usuario = "Por asignar", Nombre = "Por asignar" } 
            };

            var lUsuariosDto = await _atencionAppService.ObtenerListaUsuariosAsync();
            var lUsuarioFilterDto = lUsuariosDto.Where(x => x.Cargo.Equals(1));
            foreach (var item in lUsuarioFilterDto)
            {
                listaUsuarios.Add(new UsuarioAdmin { Usuario = item.Usuario, Nombre = item.Usuario });
            }

            switch (vista)
            {
                case "HistoriasClinicas":
                    return PartialView("_HistoriasPartial", listaUsuarios);
                case "AuditoriaOrdenes":
                    return PartialView("_OrdenesPartial", listaUsuarios);
                case "AuditoriaCuentas":
                    return PartialView("_CuentasPartial", listaUsuarios);
                case "AuditoriaFUA":
                    return PartialView("_SISPartial", listaUsuarios);
                case "AuditoriaAdmin":
                    return PartialView("_AdminPartial", listaUsuarios);
                default:
                    return Content("<p>Vista no encontrada.</p>");
            }
        }


        private void ConfigureAuthorizationHeader()
        {
            var token = GetAuthToken();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var token = GetAuthToken();
                ViewBag.NombreUsuario = HttpContext.Session.GetString("Usuario");
                var usuario = GetUsuario();

                var usuariosDto = await _atencionAppService.ObtenerListaUsuariosAsync();
                //var contadores = await _atencionAppService.ObtenerContadoresBandejaAsync();
                //var contador = contadores.FirstOrDefault();

                var usuarioCargo = usuariosDto.Where(x => x.Usuario.Equals(usuario));

                var menus = new List<dynamic>();
                if (usuarioCargo.Any())
                {
                    menus.Add(new { Flag = "hisClinicas", Icon = "assignment", Text = "Historias Clínicas", Vista = "HistoriasClinicas", Badge = 2 });
                    menus.Add(new { Flag = "regPrestamos", Icon = "account_balance", Text = "Registro de préstamos", Vista = "RegistroPrestamos", Badge = 1 });
                }

                // Lista tipada para mayor claridad y seguridad
                
                //if (usuarioCargo.Any(x => x.Cargo == 2))
                //{
                //    menus.Add(new { Flag = "admin", Icon = "admin_panel_settings", Text = "Administrar Atenciones", Vista = "AuditoriaAdmin", Badge = contador?.cantidad_por_asignar });
                //}
                //
                //menus.Add(new { Flag = "ordenes", Icon = "fact_check", Text = "Ordenes en Linea", Vista = "AuditoriaOrdenes", Badge = contador?.cantidad_evaluaciones_pendiente });
                //menus.Add(new { Flag = "cuentas", Icon = "account_balance", Text = "Auditoría Cuenta", Vista = "AuditoriaCuentas", Badge = contador?.cantidad_cuentas_pendientes });
                //menus.Add(new { Flag = "sis", Icon = "assignment_ind", Text = "SIS - Atenciones", Vista = "AuditoriaFUA", Badge = contador?.cantidad_sis_pendientes });

                ViewBag.MenuItems = menus;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en la carga del menú de auditoría");
                return RedirectToAction("ErrorLogin", "Account");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

         
        public async Task<IActionResult> HistoriasClinicas([FromQuery] HistoriaFiltro filtro)
        {
            try
            {
                var token = GetAuthToken();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {

                var paginatedResponse = await _atencionAppService.ConsultarHistoriasAsync(filtro);
                if (paginatedResponse != null)
                {
                    var result = new
                    {
                        pacientes = paginatedResponse,
                        paginacion = new
                        {
                            currentPage = filtro.Page, 
                            totalPages = paginatedResponse?.FirstOrDefault()?.total_pages,
                            totalRecords = paginatedResponse?.FirstOrDefault()?.total_records
                        }
                    };
                    return Json(result);
                }
                else
                {
                    return Json(new { pacientes = new List<Domain.Entities.HistoriaClinicaDto>(), paginacion = new { currentPage = 1, totalPages = 1 } });
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Ocurrió un error al llamar servicio AuditoriaTriaje: {ex.Message}";
                return StatusCode(500, "Error interno del servidor");
            }
        }       


        private string GetUsuario()
        {
            var usuario = HttpContext.Session.GetString("Usuario");

            if (string.IsNullOrWhiteSpace(usuario))
            {
                throw new UnauthorizedAccessException("Usuario no disponible.");
            }

            return usuario;
        }


    }
}
