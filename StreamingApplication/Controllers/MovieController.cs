using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StreamingApplication.Data.DTOs.Media;
using StreamingApplication.Data.DTOs.Movie;
using StreamingApplication.Forms;
using StreamingApplication.Helpers;
using StreamingApplication.Helpers.Parameters;
using StreamingApplication.Interfaces;


namespace StreamingApplication.Controllers;


[ApiController]
[Route("api/v1/[controller]")]
public class MovieController : ControllerBase {

    private readonly IMovieService _movieService;


    public MovieController(IMovieService movieService) {
        _movieService = movieService;
    }


    /*
     * Controller to get all movies from the database.
     * Can apply filterin, searching, sorting and pagination through parameters.
     */
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll([FromQuery] MovieParameters parameters) {
        var entities = await _movieService.GetAllAsync(parameters);
        return Ok(entities);
    }


    /*
     * Controller to get a single movie by id.
     */
    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> GetById([FromRoute] int id) {
        var entity = await _movieService.GetByIdAsync(id);
        return entity == null ? NotFound() : Ok(entity);
    }
    
    
    /*
     * Controller to create a new movie.
     */
    [HttpPost]
    [Authorize(Roles = UserRole.Admin)]
    public async Task<IActionResult> Create([FromBody] MovieCreateDTO createDTO) {
        var result = await _movieService.CreateAsync(createDTO);
        return result < 0
            ? StatusCode(StatusCodes.Status500InternalServerError)
            : CreatedAtAction(nameof(GetById), new { id = result }, null);
    }
    
    
    /*
     * Controller to update a movie.
     */
    [HttpPut("{id:int}")]
    [Authorize(Roles = UserRole.Admin)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] MovieUpdateDTO updateDTO) {
        var newEntity = await _movieService.UpdateAsync(id, updateDTO);
        return newEntity == null ? NotFound() : Ok(newEntity);
    }
    
    
    /*
     * Controller to delete a movie.
     * TODO: Maybe delete the corresponding media file as well?
     */
    [HttpDelete("{id:int}")]
    [Authorize(Roles = UserRole.Admin)]
    public async Task<IActionResult> Delete([FromRoute] int id) {
        var result = await _movieService.DeleteAsync(id);
        return !result ? NotFound() : NoContent();
    }
    
    
    /*
     * Controller to create a movie,
     * but also upload a corresponding mediafile at the same time.
     */
    [HttpPost("upload")]
    [Authorize(Roles = UserRole.Admin)]
    public async Task<IActionResult> CreateAndUpload([FromForm] UploadMovieForm uploadForm) {
        var result = await _movieService.CreateAndUploadAsync(uploadForm.MovieUpload, uploadForm.MediaUpload, uploadForm.File);
        return result < 0
            ? StatusCode(StatusCodes.Status500InternalServerError)
            : CreatedAtAction(nameof(GetById), new { id = result }, null);
    }

}