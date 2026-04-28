using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PiiScanner.Application.Service;
using PiiScanner.Domain.Entity;

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

        [HttpPost]
        public async Task<IActionResult> CreateScan([FromBody] Scan scan)
        {
            if (scan == null)
                return BadRequest("Scan payload is required.");

            var detected = await _scanService.CreateScan(scan);

            // Return the detected PII types. Use200 OK for now. Change to201 Created with location if persistence is added.
            return Ok(detected);
        }
    }
}
