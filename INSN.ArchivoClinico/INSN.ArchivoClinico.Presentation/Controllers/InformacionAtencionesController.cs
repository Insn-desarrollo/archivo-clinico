using INSN.ArchivoClinico.Application.DTOs;
using INSN.ArchivoClinico.Models;
using INSN.ArchivoClinico.UtilFactory.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.Http;
using INSN.ArchivoClinico.Application.Interfaces;
using INSN.ArchivoClinico.Controllers;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.Extensions.Configuration;

namespace INSN.ArchivoClinico.Presentation.Controllers
{
    public class InformacionAtencionesController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<InformacionAtencionesController> _logger;
        private readonly IConfiguration _configuration;

        public InformacionAtencionesController(ILogger<InformacionAtencionesController> logger, HttpClient httpClient, IAntiforgery antiforgery, IConfiguration configuration)
        {
            _logger = logger;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var baseUrl = _configuration["ApiUrls:Auditoria"];
            var url = $"{baseUrl}/GetAtencionesInformacion?pageNumber={1}&pageSize={20}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var paginatedResponse = JsonConvert.DeserializeObject<PagedResponse<AtencionesInformacionPaginadoCEDTO>>(jsonResponse);
                    var atencionCEDTO = paginatedResponse.Data;

                    return View(atencionCEDTO);

                   // return Json(atencionCEDTO);
                }
                else
                {
                   return Json(new { success = false, message = "Error al obtener los datos del servicio REST." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al llamar al servicio REST.");
                return Json(new { success = false, message = "Ocurrió un error al comunicarse con el servicio REST." });
            }

        }

        // GET: PantallaInformacionAtencionesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PantallaInformacionAtencionesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PantallaInformacionAtencionesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(IndexAsync));
            }
            catch
            {
                return View();
            }
        }

        // GET: PantallaInformacionAtencionesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PantallaInformacionAtencionesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(IndexAsync));
            }
            catch
            {
                return View();
            }
        }

        // GET: PantallaInformacionAtencionesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PantallaInformacionAtencionesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(IndexAsync));
            }
            catch
            {
                return View();
            }
        }
    }
}
