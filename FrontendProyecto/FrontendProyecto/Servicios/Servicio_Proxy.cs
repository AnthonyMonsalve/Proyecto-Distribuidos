using FrontendProyecto.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography;
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
                Console.WriteLine("Es posible que ya exista un usuario con el mismo nombre o ha ocurrido algun otro problema");
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

        public async Task<string> Integridad(EntradaIntegridad entradaIntegridad)
        {
            HttpClient cliente = new()
            {
                BaseAddress = new Uri(_baseurl)
            };

            try
            {
                var respuesta = verificarIntegridad(entradaIntegridad);

                return await respuesta;

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

        public async Task<string> ConsultarTxt()
        {
            HttpClient cliente = new()
            {
                BaseAddress = new Uri("http://localhost:5152")
            };
            try
            {
                var response = await cliente.GetAsync("ConsultaTxt/");
                if (response.IsSuccessStatusCode)
                {
                    var respuesta = await response.Content.ReadAsStringAsync();
                    return respuesta;
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

            return null;
        }

        public async Task<string> verificarIntegridad(EntradaIntegridad entrada)
        {
            //Desencriptando Mensaje llegada
            Aes aes = Aes.Create();
            aes.Key = Encoding.UTF32.GetBytes(entrada.Clave.ToString());
            byte[] IV = Encoding.UTF8.GetBytes("ClaveSecreta1234");
            aes.IV = IV;
            byte[] hashDesencriptado = Convert.FromHexString(entrada.HashEncriptado);
            string hashEntrante = DecryptStringFromBytes_Aes(hashDesencriptado, aes.Key, aes.IV);

            //Hasheando Mensaje
            var bytemensaje = Encoding.UTF32.GetBytes(entrada.Mensaje);
            var mySHA256 = SHA256.Create();
            byte[] hashCalculado = mySHA256.ComputeHash(bytemensaje);
            string hashCalculadostring = Convert.ToHexString(hashCalculado);

            if (hashCalculadostring.Equals(hashEntrante))
            {
                return "Integro";
            }
            return "No Integro";
        }

        static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

        
    }
}
