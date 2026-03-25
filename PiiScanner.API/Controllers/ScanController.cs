using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PiiScanner.Application.Service;

namespace PiiScanner.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScanController : ControllerBase
    {
        private readonly ScanService _scanService;

        public ScanController(ScanService scanService)
        {
            _scanService = scanService;
        }

        [HttpGet]
        public async Task<IActionResult> Scan()
        {
            var result = await _scanService.ScanAsync();
            return Ok(result);
        }
    }
}
