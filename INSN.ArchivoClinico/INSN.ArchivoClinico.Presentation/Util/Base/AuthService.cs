using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using INSN.ArchivoClinico.Domain.Entities;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System;

namespace INSN.ArchivoClinico.UtilFactory.Base
{
    public class AuthService
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration, HttpClient client)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<string> AuthenticateAsync()
        {
            var loginData = new
            {
                usuario = _configuration["SIS:Login:Usuario"],
                contrasena = _configuration["SIS:Login:Contrasena"],
                ip = _configuration["SIS:Login:IP"]
                //usuario = "setisis",
                //contrasena = "1234",
                //ip = "*"
            };

            var content = new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json");
            var url = _configuration["SIS:Login:AuthUrl"];
            var response = await _client.PostAsync(url, content);

            //var response = await _client.PostAsync("https://desarrollo15.sis.gob.pe/api/v1/autenticacion/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                var responseData = JsonConvert.DeserializeObject<ResponseData>(responseContent);

                if (responseData.Success)
                {
                    return responseData.Data.Token;
                }
                else
                {
                    Console.WriteLine($"Error: {string.Join(", ", responseData.Mensaje)}");
                    return null;
                }
            }
            else
            {
                Console.WriteLine($"Error en la solicitud: {response.StatusCode}");
                return null;
            }

        }

        public async Task<HttpResponseMessage> ProcesarAsync(List<Atencion> atencion, string tokenSIS) {
            var data = new
            {
                atencion = atencion,
            };

            if (!_client.DefaultRequestHeaders.Contains("Authorization"))
            {
                _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenSIS);
            }
            //_client.BaseAddress = new Uri("https://desarrollo15.sis.gob.pe");
            //  _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenSIS);

            //var content = new StringContent(JsonConvert.SerializeObject(atencion), Encoding.UTF8, "application/json");
            //var jsonContent = System.Text.Json.JsonSerializer.Serialize(new { atencion });

            var jsonContent = System.Text.Json.JsonSerializer.Serialize(new { atencion });            
            //var jsonContent = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var url = _configuration["SIS:ProcesarUrl"];
            var response = await _client.PostAsync(url, content);

            return response;

        }

        public async Task<HttpResponseMessage> ConsultarAsync(string guidFua, string tokenSIS)
        {
            var data = new
            {
                guid = guidFua,
            };

            if (!_client.DefaultRequestHeaders.Contains("Authorization"))
            {
                _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenSIS);
            }
            //_client.BaseAddress = new Uri("https://desarrollo15.sis.gob.pe");
            //_client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenSIS);

            //var content = new StringContent(JsonConvert.SerializeObject(atencion), Encoding.UTF8, "application/json");
            var jsonContent = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var url = _configuration["SIS:ConsultarUrl"];
            var response = await _client.PostAsync(url, content);

            return response;

        }

        
    }
}
