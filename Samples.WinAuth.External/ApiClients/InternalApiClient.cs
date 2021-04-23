using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Samples.WinAuth.External
{
    public class InternalApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<InternalApiClient> _logger;
        public InternalApiClient(IHttpClientFactory factory, ILogger<InternalApiClient> logger)
        {
            _client = factory.CreateClient("InternalApiClient");
            _logger = logger;
        }

        public ObjectResult GetClaims(WindowsIdentity identity)
        {

            ObjectResult result = null;
            try
            {
                WindowsIdentity.RunImpersonated(identity.AccessToken, () =>
                {
                    var response = _client.GetAsync("claims").Result;
                    result = new ObjectResult(response.Content.ReadAsStringAsync().Result)
                        { StatusCode = (int)response.StatusCode };
                });
            } catch(Exception ex)
            {
                if(identity == null)
                    _logger.LogError(ex, "Error running impersonated: null identity");
                else if (identity.AccessToken == null)
                    _logger.LogError(ex, "Error running impersonated: null access token");

                _logger.LogError(ex, "Error running impersonated");
            }

            return result;
        }

    }
}
