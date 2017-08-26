using EndlessBackend.Services;
using EndlessConfiguration;
using EndlessConfiguration.Models.Backend.Result;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EndlessBackend.Controllers
{
    [Produces("application/json")]
    public class UpdateController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Update(string code, string serverName)
        {
            if (code != Config.SecretCode)
            {
                return Json(new GameServerResponceModel("Nope", true));
            }
            var service = UpdateService.Inititalize(serverName);
            if (service == null)
            {
                return Json(new GameServerResponceModel("Invalid server name", true));
            }
            return Json(service.Update());
        }
    }
}