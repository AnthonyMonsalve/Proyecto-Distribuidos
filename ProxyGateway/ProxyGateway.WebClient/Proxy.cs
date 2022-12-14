using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ProxyGateway.WebClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProxyGateway.WebClient
{



    public class Proxy : IProxy
    {
        private readonly ApiUrls apiUrls;
        private readonly HttpClient httpClient;

        public Proxy(
            HttpClient httpClient,
            IOptions<ApiUrls> apiUrls,
            IHttpContextAccessor httpContextAccessor)
        {
            this.httpClient = httpClient;
            this.apiUrls = apiUrls.Value;
        }

        public async Task<Firma> Firmar(EntradaFirma entrada)
        {
            var content = JsonConvert.SerializeObject(entrada);
            var httpContent = new StringContent(content, Encoding.UTF8, "application/json");
            var Url = apiUrls.ServidorAUrl + "/Firmar";
            var request = await httpClient.PostAsync(Url, httpContent);
            request.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<Firma>(
                await request.Content.ReadAsStringAsync());

        }

        public async Task<string> Autenticar(EntradaAutenticar entrada)
        {
            var content = JsonConvert.SerializeObject(entrada);
            var httpContent = new StringContent(content, Encoding.UTF8, "application/json");
            var Url = apiUrls.ServidorBUrl + "/Autenticar";
            var request = await httpClient.PostAsync(Url, httpContent);
            request.EnsureSuccessStatusCode();

            return await request.Content.ReadAsStringAsync();

        }

        public async Task<Usuario> CrearUsuario(string nombre)
        {
            var content = JsonConvert.SerializeObject(nombre);
            var httpContent = new StringContent(content, Encoding.UTF8, "application/json");
            var Url = apiUrls.ServidorAUrl + "/CrearUsuario";
            var request = await httpClient.PostAsync(Url, httpContent);
            request.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<Usuario>(
                await request.Content.ReadAsStringAsync());
        }
    }
}
