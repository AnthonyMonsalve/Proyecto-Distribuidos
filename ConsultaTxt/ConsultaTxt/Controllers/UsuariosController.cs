using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ConsultaTxt.Data;
using ConsultaTxt.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SolucionServidorA.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly UsuarioContext _context;

        public UsuariosController(UsuarioContext context)
        {
            _context = context;
        }


        [HttpGet]
        [Route("ConsultaTxt/")]
        public async Task<string> ConsultaTxt()
        {

            var listaUsuarios = _context.Usuarios.ToList();
            string cadenaUsuario = "";
            foreach (var usuario in listaUsuarios)
            {
                cadenaUsuario += "nombre=" + usuario.nombre + ",clave=" + usuario.clave+";";
            }
            return cadenaUsuario;
        }
    }
}
