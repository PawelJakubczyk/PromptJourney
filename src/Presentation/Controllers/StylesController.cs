using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyStyles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StylesController : ControllerBase
{
    private readonly IStyleRepository _styleRepository;

    public StylesController(IStyleRepository styleRepository)
    {
        _styleRepository = styleRepository;
    }

    // GET api/styles
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _styleRepository.GetAllStylesAsync();
        if (result.IsFailed)
            return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);
        return Ok(result.Value);
    }

    // GET api/styles/{name}
    [HttpGet("{name}")]
    public async Task<IActionResult> GetByName(string name)
    {
        var result = await _styleRepository.GetStyleByNameAsync(name);
        if (result.IsFailed)
            return NotFound(result.Errors);
        return Ok(result.Value);
    }

    // GET api/styles/by-type/{type}
    [HttpGet("by-type/{type}")]
    public async Task<IActionResult> GetByType(string type)
    {
        var result = await _styleRepository.GetStylesByTypeAsync(type);
        if (result.IsFailed)
            return NotFound(result.Errors);
        return Ok(result.Value);
    }

    // GET api/styles/by-tags?tags=tag1&tags=tag2
    [HttpGet("by-tags")]
    public async Task<IActionResult> GetByTags([FromQuery] List<string> tags)
    {
        var result = await _styleRepository.GetStylesByTagsAsync(tags);
        if (result.IsFailed)
            return NotFound(result.Errors);
        return Ok(result.Value);
    }

    // GET api/styles/by-description?keyword=forest
    [HttpGet("by-description")]
    public async Task<IActionResult> GetByDescription([FromQuery] string keyword)
    {
        var result = await _styleRepository.GetStylesByDescriptionKeywordAsync(keyword);
        if (result.IsFailed)
            return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);
        return Ok(result.Value);
    }

    // GET api/styles/{name}/exists
    [HttpGet("{name}/exists")]
    public async Task<IActionResult> CheckExists(string name)
    {
        var result = await _styleRepository.CheckStyleExistsAsync(name);
        if (result.IsFailed)
            return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);
        return Ok(new { exists = result.Value });
    }

    // POST api/styles
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] MidjourneyStyle style)
    {
        var result = await _styleRepository.AddStyleAsync(style);
        if (result.IsFailed)
            return BadRequest(result.Errors);
        return CreatedAtAction(nameof(GetByName), new { name = result.Value.Name }, result.Value);
    }

    // PUT api/styles/{name}
    [HttpPut("{name}")]
    public async Task<IActionResult> Update(string name, [FromBody] MidjourneyStyle style)
    {
        if (name != style.Name)
            return BadRequest("Route name and payload name must match");

        var result = await _styleRepository.UpdateStyleAsync(style);
        if (result.IsFailed)
            return BadRequest(result.Errors);
        return Ok(result.Value);
    }

    // DELETE api/styles/{name}
    [HttpDelete("{name}")]
    public async Task<IActionResult> Delete(string name)
    {
        var result = await _styleRepository.DeleteStyleAsync(name);
        if (result.IsFailed)
            return NotFound(result.Errors);
        return NoContent();
    }

    // POST api/styles/{name}/tags
    [HttpPost("{name}/tags")]
    public async Task<IActionResult> AddTag(string name, [FromBody] string tag)
    {
        var result = await _styleRepository.AddTagToStyleAsync(name, tag);
        if (result.IsFailed)
            return BadRequest(result.Errors);
        return Ok(result.Value);
    }

    // DELETE api/styles/{name}/tags/{tag}
    [HttpDelete("{name}/tags/{tag}")]
    public async Task<IActionResult> RemoveTag(string name, string tag)
    {
        var result = await _styleRepository.DeleteTagFromStyleAsync(name, tag);
        if (result.IsFailed)
            return BadRequest(result.Errors);
        return Ok(result.Value);
    }

    // PUT api/styles/{name}/description
    [HttpPut("{name}/description")]
    public async Task<IActionResult> UpdateDescription(string name, [FromBody] string description)
    {
        var result = await _styleRepository.UpadteStyleDescription(name, description);
        if (result.IsFailed)
            return BadRequest(result.Errors);
        return Ok(result.Value);
    }
}