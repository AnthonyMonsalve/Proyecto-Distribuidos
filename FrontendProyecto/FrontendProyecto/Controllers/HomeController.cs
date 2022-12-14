using FrontendProyecto.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

using FrontendProyecto.Servicios;
using Newtonsoft.Json.Linq;

namespace FrontendProyecto.Controllers
{
    public class HomeController : Controller
    {
        private readonly IServicio_Proxy _servicioProxy;

        public HomeController(IServicio_Proxy servicioProxy)
        {

            _servicioProxy = servicioProxy;
        }

        public async Task<IActionResult> Index()
        {
            EntradaIndex entrada = new();
            entrada.Dato = "";
            try
            {
                entrada.Dato = await _servicioProxy.ConsultarTxt();
                Console.WriteLine(entrada.Dato);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return View(entrada);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}