﻿using Microsoft.EntityFrameworkCore;
using SolucionServidorA.Entities;

namespace SolucionServidorA.Data
{
    public class UsuarioContext : DbContext
    {
        public UsuarioContext(DbContextOptions<UsuarioContext> options) : base(options)
        {

        }
        public DbSet<Usuario> Usuarios { get; set; }
    }
}
