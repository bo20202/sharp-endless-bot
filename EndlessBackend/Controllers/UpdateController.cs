using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EndlessBackend.Services;
using EndlessConfiguration;
using EndlessConfiguration.Models.Backend.Result;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EndlessBackend.Controllers
{
    [Produces("application/json")]
    public class UpdateController : Controller
    {
        private UpdateService _service;

        public UpdateController(UpdateService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Update(string code, string serverName)
        {
            if (code != Config.SecretCode)
            {
                return Json(new GameServerResponceModel("Nope", true));
            }
            return Json(await _service.UpdateServer(serverName));
        }
    }
}