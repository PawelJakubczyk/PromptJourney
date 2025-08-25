using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyVersions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VersionsController : ControllerBase
    {
        private readonly IVersionRepository _repo;

        public VersionsController(IVersionRepository repo)
        {
            _repo = repo;
        }

        // GET api/versions
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _repo.GetAllVersionsAsync();
            if (result.IsFailed)
                return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);

            return Ok(result.Value);
        }

        // GET api/versions/supported
        [HttpGet("supported")]
        public async Task<IActionResult> GetSupported()
        {
            var result = await _repo.GetAllSuportedVersionsAsync();
            if (result.IsFailed)
                return NotFound(result.Errors);

            return Ok(result.Value);
        }

        // GET api/versions/{version}
        [HttpGet("{version}")]
        public async Task<IActionResult> GetByVersion(string version)
        {
            var result = await _repo.GetMasterVersionByVersionAsync(version);
            if (result.IsFailed)
                return NotFound(result.Errors);

            return Ok(result.Value);
        }

        // GET api/versions/{version}/exists
        [HttpGet("{version}/exists")]
        public async Task<IActionResult> CheckExists(string version)
        {
            var result = await _repo.CheckVersionExistsInVersionsAsync(version);
            if (result.IsFailed)
                return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);

            return Ok(new { exists = result.Value });
        }

        //// POST api/versions
        //[HttpPost]
        //public async Task<IActionResult> Create([FromBody] MidjourneyVersions version)
        //{
        //    var result = await _repo.AddVersionAsync(version);
        //    if (result.IsFailed)
        //        return BadRequest(result.Errors);

        //    return CreatedAtAction(
        //        nameof(GetByVersion),
        //        new { version = result.Value.Version },
        //        result.Value
        //    );
        //}
    }
}