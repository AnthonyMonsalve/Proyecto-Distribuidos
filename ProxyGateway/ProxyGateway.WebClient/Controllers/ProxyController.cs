
using Microsoft.AspNetCore.Mvc;
using ProxyGateway.WebClient.Models;
using System.Text;

namespace ProxyGateway.WebClient.Controllers
{
    [ApiController]
    [Route("Proxy")]
    public class ProxyController : ControllerBase
    {
        private readonly IProxy proxy;
        public ProxyController(IProxy proxy)
        {
            this.proxy = proxy;
        }

        [HttpPost]
        [Route("Firmar/")]
        public async Task<Firma> Firmar([FromBody] EntradaFirma entrada)
        {
            try
            {
                var request = await proxy.Firmar(entrada);
                return request;
            }
            catch (Exception ex)
            {
                // Aquí se puede manejar la excepción de la forma que se desee
                // Por ejemplo, registrando el error en un archivo de logs,
                // mostrando un mensaje de error al usuario, etc.
                return null;
            }
        }

        [HttpPost]
        [Route("Integridad/")]
        public async Task<string> verificarIntegridad([FromBody] EntradaIntegridad entrada)
        {
            try
            {
                var request = await proxy.verificarIntegridad(entrada);
                return request;
            }
            catch (Exception ex)
            {
                // Aquí se puede manejar la excepción de la forma que se desee
                // Por ejemplo, registrando el error en un archivo de logs,
                // mostrando un mensaje de error al usuario, etc.
                return ": no se pudo verificar la Integridad";
            }
        }



        [HttpPost]
        [Route("Autenticar/")]
        public async Task<string> Autenticar([FromBody] EntradaAutenticar entrada)
        {
            try
            {
                var request = await proxy.Autenticar(entrada);
                return request;
            }
            catch (Exception ex)
            {
                // Aquí se puede manejar la excepción de la forma que se desee
                // Por ejemplo, registrando el error en un archivo de logs,
                // mostrando un mensaje de error al usuario, etc.
                return ex.Message;
            }
        }

        [HttpPost]
        [Route("CrearUsuario/")]
        public async Task<Usuario> CrearUsuario([FromBody] string nombre)
        {
            try
            {
                var request = (Usuario)await proxy.CrearUsuario(nombre);
                return request;
            }
            catch (Exception ex)
            {
                // Aquí se puede manejar la excepción de la forma que se desee
                // Por ejemplo, registrando el error en un archivo de logs,
                // mostrando un mensaje de error al usuario, etc.
                return null;
            }
        }
    }
}
