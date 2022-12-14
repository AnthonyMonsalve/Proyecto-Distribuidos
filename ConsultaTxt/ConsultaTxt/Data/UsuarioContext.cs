using Microsoft.EntityFrameworkCore;
using ConsultaTxt.Entities;

namespace ConsultaTxt.Data
{
    public class UsuarioContext : DbContext
    {
        public UsuarioContext(DbContextOptions<UsuarioContext> options) : base(options)
        {

        }
        public DbSet<Usuario> Usuarios { get; set; }
    }
}
