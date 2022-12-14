using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SolucionServidorB.Data;
using SolucionServidorB.Entities;
using SolucionServidorB.Models;

namespace SolucionServidorB.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly UsuarioContext _context;

        public UsuariosController(UsuarioContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        public async Task<List<Usuario>> Index()
        {
            return await _context.Usuarios.ToListAsync();
        }

        [HttpPost]
        [Route("Autenticar/")]
        public async Task<string> Autenticar([FromBody] EntradaAutenticar entrada)
        {
            //verificar usuario
            var user = _context.Usuarios.Where(x => x.nombre == entrada.nombreUsuario && x.clave == entrada.clave).FirstOrDefault();
            if (user == null)
            {
                return "NO AUTORIZADO";
            }
            return "AUTORIZADO";
        }

    }
}
