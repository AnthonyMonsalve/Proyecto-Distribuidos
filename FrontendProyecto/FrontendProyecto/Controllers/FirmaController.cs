﻿using FrontendProyecto.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using FrontendProyecto.Servicios;
using Newtonsoft.Json.Linq;

namespace FrontendProyecto.Controllers
{
    public class FirmaController : Controller
    {
        private readonly IServicio_Proxy _servicioProxy;

        public FirmaController(IServicio_Proxy servicioProxy)
        {
            _servicioProxy = servicioProxy;
        }

        public async Task<IActionResult> CrearUsuario()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrearUsuarioProxy(String nombreUsuario)
        {

            UsuarioResultado respuesta;

            try
            {
                respuesta = await _servicioProxy.CrearUsuario(nombreUsuario);

                if (respuesta.Id != null)
                {
                    return RedirectToAction("CrearUsuario", new { message = "nuevo usuario", clave = respuesta.Clave, nombre = respuesta.Nombre, userKey = respuesta.Id });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return NoContent();
        }

        public async Task<IActionResult> Autenticar()
        {
            EntradaAutenticar Autenticar = new();
            return View(Autenticar);
        }

        [HttpPost]
        public async Task<IActionResult> AutenticarProxy(EntradaAutenticar entradaAutenticar)
        {
            Console.WriteLine(entradaAutenticar.Clave);
            Console.WriteLine(entradaAutenticar.NombreUsuario);

            try
            {
                string respuesta = await _servicioProxy.Autenticar(entradaAutenticar);
                return RedirectToAction("Autenticar", new { message = "Autenticacion", autorizacion = respuesta});
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return NoContent();
        }

        public async Task<IActionResult> Firmar()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> FirmarProxy(EntradaFirmar entradaFirmar)
        {
            Console.WriteLine(entradaFirmar.Clave);
            Console.WriteLine(entradaFirmar.Mensaje);
            Console.WriteLine(entradaFirmar.NombreUsuario);
            try
            {
                FirmaResultado respuesta = await _servicioProxy.Firmar(entradaFirmar);
                
                if (respuesta.Firma != null)
                {
                    return RedirectToAction("Firmar", new { message = "Firma", clave = respuesta.Clave, firma = respuesta.Firma, firmaKey = respuesta.Id });
                }
                else
                {
                    return RedirectToAction("Firmar", new { message = "NoFirma"});
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return NoContent();
        }

        public async Task<IActionResult> Integridad()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IntegridadProxy(EntradaIntegridad entradaIntegridad)
        {
            Console.WriteLine(entradaIntegridad.Clave);
            Console.WriteLine(entradaIntegridad.Mensaje);
            Console.WriteLine(entradaIntegridad.HashEncriptado);

            try
            {
                string respuesta = await _servicioProxy.Integridad(entradaIntegridad);
                return RedirectToAction("Integridad", new { message = "Integridad", integro = respuesta });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return NoContent();
        }

    }
}