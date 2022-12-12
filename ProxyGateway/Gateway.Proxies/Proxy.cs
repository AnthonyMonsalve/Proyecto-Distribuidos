using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Proxies
{
    internal class Proxy
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
    }
}
