using ProxyGateway.WebClient.Models;

namespace ProxyGateway.WebClient
{
    public interface IProxy
    {
        Task<Firma> Firmar(EntradaFirma entrada);
        Task<string> Autenticar(EntradaAutenticar entrada);
        Task<string> verificarIntegridad(EntradaIntegridad entrada);
        Task<Usuario> CrearUsuario(string nombre);
    }
}
