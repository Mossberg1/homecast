using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StreamingApplication.Data.DTOs.Media;
using StreamingApplication.Forms;
using StreamingApplication.Helpers;
using StreamingApplication.Helpers.Parameters;
using StreamingApplication.Interfaces;


namespace StreamingApplication.Controllers;


[ApiController]
[Route("api/v1/[controller]")]
public class MediaController : ControllerBase {
    private readonly IMediaService _mediaService;


    public MediaController(IMediaService ms) {
        _mediaService = ms;
    }


    /* Controller to get all media file entries in the database. */
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll([FromQuery] MediaParameters parameters) {
        return Ok(await _mediaService.GetAllAsync(parameters));
    }


    /* Controller to create a media file entity for the database
     * if the media file is already uploaded, but does not have a corresponding entity. */
    [HttpPost]
    [Authorize(Roles = UserRole.Admin)]
    public async Task<IActionResult> Create([FromBody] MediaCreateDTO createDTO) {
        var result = await _mediaService.CreateAsync(createDTO);
        return result < 0
            ? StatusCode(StatusCodes.Status500InternalServerError)
            : CreatedAtAction(nameof(GetById), new { id = result }, null);
    }


    /* Controller to get a specific media entity by id */
    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> GetById([FromRoute] int id) {
        var dto = await _mediaService.GetByIdAsync(id);
        return dto == null
            ? NotFound()
            : Ok(dto);
    }


    /* Controller to update a specific media entity */
    [HttpPut("{id:int}")]
    [Authorize(Roles = UserRole.Admin)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] MediaUpdateDTO updateDTO) {
        var updatedEntity = await _mediaService.UpdateAsync(id, updateDTO);
        return updatedEntity == null
            ? NotFound()
            : Ok(updatedEntity);
    }


    /* Controller to delete a specific media entity */
    [HttpDelete("{id:int}")]
    [Authorize(Roles = UserRole.Admin)]
    public async Task<IActionResult> Delete([FromRoute] int id) {
        return !await _mediaService.DeleteAsync(id)
            ? NotFound()
            : NoContent();
    }


    /* Controller to upload a file to storage and store the metadata in the database. */
    [HttpPost("upload")]
    [Authorize(Roles = UserRole.Admin)]
    public async Task<IActionResult> Upload([FromForm] UploadMediaForm mediaForm) {
        var result = await _mediaService.UploadAsync(mediaForm.File, mediaForm.UploadDTO);
        return result < 0
            ? StatusCode(StatusCodes.Status500InternalServerError)
            : CreatedAtAction(nameof(GetById), new { id = result }, null);
    }
}