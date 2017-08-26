using EndlessConfiguration;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using EndlessBackend.Services;
using EndlessConfiguration.Models.Backend.Result;

namespace EndlessBackend.Controllers
{
    public class ServerController : Controller
    { 
        private readonly GameServerService _service;
        public ServerController(GameServerService service)
        {
            _service = service;
        }
        [HttpPost]
        public async Task<JsonResult> StartServer(string serverName, string secretKey)
        {
            if (secretKey != Config.SecretCode)
            {
                return Json(new GameServerResponceModel("Fuck you.", true));
            }

            return Json(await _service.StartServer(serverName));
        }

        [HttpPost]
        public JsonResult StopServer(string serverName, string secretKey)
        {
            if (secretKey != Config.SecretCode)
            {
                return Json(new GameServerResponceModel("Fuck you again fagget.", true));
            }

            return Json(_service.StopServer(serverName));
        }

    }
}