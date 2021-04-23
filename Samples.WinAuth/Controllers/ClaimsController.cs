using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Samples.WinAuth.Shared;
using System;
using System.Linq;
using System.Security.Principal;

namespace Samples.WinAuth.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController : ControllerBase
    {

        private readonly ILogger<ClaimsController> _logger;

        public ClaimsController(ILogger<ClaimsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            if (User == null)
            {
                ModelState.AddModelError("", "User is null");
                _logger.LogWarning("User is null");
                return new ObjectResult(ModelState) { StatusCode = 401 };
            }

            if (!User.Identity.IsAuthenticated)
            {
                ModelState.AddModelError("", "User isn't authenticated null");
                _logger.LogWarning("User isn't authenticated");
                return new ObjectResult(ModelState) { StatusCode = 401 };
            }

            if (User.Claims == null || User.Claims.Count() == 0)
            {
                ModelState.AddModelError("", "User has no claims");
                _logger.LogWarning("User has no claims");
                return new ObjectResult(ModelState) { StatusCode = 401 };
            }


            var claims = User.Claims.ToLoggerScope();
            using var scope = _logger.BeginScope(claims);
            _logger.LogInformation("User authenticated");

            try
            {
                WindowsIdentity identity = (WindowsIdentity)HttpContext.User.Identity;
            } catch (Exception ex)
            {
                ModelState.AddModelError("", "User has no Windows Identity");
                _logger.LogError(ex, "User has no Windows Identity");
                return new ObjectResult(ModelState) { StatusCode = 401 };
            }

            return new OkObjectResult(claims);
        }


    }
}
