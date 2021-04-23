using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace Samples.WinAuth.External.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController : ControllerBase
    {

        private readonly ILogger<ClaimsController> _logger;
        private readonly InternalApiClient _client;

        public ClaimsController(ILogger<ClaimsController> logger,
            InternalApiClient client)
        {
            _logger = logger;
            _client = client;
        }


        [HttpGet]
        public Dictionary<string,ObjectResult> Get()
        {
            var dict = new Dictionary<string, ObjectResult>();

            if (User == null)
            {
                ModelState.AddModelError("", "User is null");
                _logger.LogWarning("User is null");
                dict.Add("External",new ObjectResult(ModelState) { StatusCode = 401 });
            }
            else if (!User.Identity.IsAuthenticated)
            {
                ModelState.AddModelError("", "User isn't authenticated null");
                _logger.LogWarning("User isn't authenticated");
                dict.Add("External", new ObjectResult(ModelState) { StatusCode = 401 });
            }
            else if (User.Claims == null || User.Claims.Count() == 0)
            {
                ModelState.AddModelError("", "User has no claims");
                _logger.LogWarning("User has no claims");
                dict.Add("External", new ObjectResult(ModelState) { StatusCode = 401 });
            }
            else
            {
                var claims = User.Claims
                        .GroupBy(c => new { c.Type })
                        .Select(g => new { g.Key.Type, Value = g.Select(i => i.Value).ToArray() })
                        .ToDictionary(d => d.Type, d => (object)d.Value);

                using var scope = _logger.BeginScope(claims);
                _logger.LogInformation("User authenticated");


                dict.Add("External", new ObjectResult(claims) { StatusCode = 200 });
            }

            try
            {
                WindowsIdentity identity = (WindowsIdentity)HttpContext.User.Identity;
                var internalClaimsResult = _client.GetClaims(identity);
                dict.Add("Internal", internalClaimsResult);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "User has no Windows Identity");
                _logger.LogError(ex, "User has no Windows Identity");
                dict.Add("Internal", new ObjectResult(ModelState) { StatusCode = 401 });
            }


            return dict;
        }


    }
}
