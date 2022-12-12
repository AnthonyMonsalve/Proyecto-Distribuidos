using FrontendProyecto.Models;

namespace FrontendProyecto.Servicios
{
    public interface IServicio_Proxy
    {
        Task<UsuarioResultado> CrearUsuario(string nombre);
        Task<string> Autenticar(EntradaAutenticar entradaAutenticar);
        Task<string> Integridad(EntradaIntegridad entradaIntegridad);
        Task<FirmaResultado> Firmar(EntradaFirmar entradaFirmar);  
    }
}
