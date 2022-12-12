using Microsoft.EntityFrameworkCore;
using SolucionServidorB.Entities;

namespace SolucionServidorB.Data
{
    public class UsuarioContext : DbContext
    {
        public UsuarioContext(DbContextOptions<UsuarioContext> options) : base(options)
        {

        }
        public DbSet<Usuario> Usuarios { get; set; }
    }
}
