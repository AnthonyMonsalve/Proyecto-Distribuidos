using FrontendProyecto.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace FrontendProyecto.Servicios
{
    public class Servicio_Proxy : IServicio_Proxy
    {
        private static string _baseurl;

        public Servicio_Proxy()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            _baseurl = builder.GetSection("ApiSettings:baseUrl").Value;
        }

        public async Task<UsuarioResultado> CrearUsuario(string nombreUsuario)
        {
            UsuarioResultado usuario = new();

            HttpClient cliente = new()
            {
                BaseAddress = new Uri(_baseurl)
            };

            var content = new StringContent(JsonConvert.SerializeObject(nombreUsuario), Encoding.UTF8, "application/json");

            try
            {
                var response = await cliente.PostAsync("Proxy/CrearUsuario/", content);
                if (response.IsSuccessStatusCode)
                {
                    var respuesta = await response.Content.ReadAsStringAsync();
                    JObject json_respuesta = JObject.Parse(respuesta);

                    //Obtengo la data del json respuesta
                    string stringDataRespuesta = json_respuesta.ToString();
                    var resultado = JsonConvert.DeserializeObject<UsuarioResultado>(stringDataRespuesta);

                    usuario = resultado;
                }

            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"ERROR de conexión con la API: '{ex.Message}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.WriteLine(usuario.Nombre);

            return usuario;

        }

        public async Task<string> Autenticar(EntradaAutenticar entradaAutenticar)
        {
        
            HttpClient cliente = new()
            {
                BaseAddress = new Uri(_baseurl)
            };

            var content = new StringContent(JsonConvert.SerializeObject(entradaAutenticar), Encoding.UTF8, "application/json");
            
            try
            {
                var response = await cliente.PostAsync("Proxy/Autenticar/", content);
                var respuesta = await response.Content.ReadAsStringAsync();
            
                return respuesta;

            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"ERROR de conexión con la API: '{ex.Message}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return "ERROR";

        }

        public async Task<string> Integridad(EntradaIntegridad entradaIntegridad)
        {
            HttpClient cliente = new()
            {
                BaseAddress = new Uri(_baseurl)
            };

            var content = new StringContent(JsonConvert.SerializeObject(entradaIntegridad), Encoding.UTF8, "application/json");

            try
            {
                var response = await cliente.PostAsync("Proxy/Integridad/", content);
                var respuesta = await response.Content.ReadAsStringAsync();

                return respuesta;

            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"ERROR de conexión con la API: '{ex.Message}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return "ERROR";
        }

        public async Task<FirmaResultado> Firmar(EntradaFirmar entradaFirmar)
        {
            FirmaResultado firmaResultado = new();

            HttpClient cliente = new()
            {
                BaseAddress = new Uri(_baseurl)
            };

            var content = new StringContent(JsonConvert.SerializeObject(entradaFirmar), Encoding.UTF8, "application/json");

            try
            {
                var response = await cliente.PostAsync("Proxy/Firmar/", content);
                if (response.IsSuccessStatusCode)
                {
                    var respuesta = await response.Content.ReadAsStringAsync();
                    JObject json_respuesta = JObject.Parse(respuesta);

                    //Obtengo la data del json respuesta
                    string stringDataRespuesta = json_respuesta.ToString();
                    var resultado = JsonConvert.DeserializeObject<FirmaResultado>(stringDataRespuesta);

                    firmaResultado = resultado;
                }

            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"ERROR de conexión con la API: '{ex.Message}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return firmaResultado;
        }
    }
}
