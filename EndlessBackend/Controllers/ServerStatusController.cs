using System.Threading.Tasks;
using EndlessBackend.Models;
using EndlessBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace EndlessBackend.Controllers
{
    [Produces("application/json")]
    [Route("api/ServerStatus")]
    public class ServerStatusController : Controller
    {
        private readonly ServerStatusService _service;
        public ServerStatusController(ServerStatusService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get(ServerMonitorModel serverMonitor)
        {
            return Ok(await _service.GetInfo(serverMonitor));
        }


    }
}