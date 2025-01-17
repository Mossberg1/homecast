using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreamingApplication.Data.Entities;
using StreamingApplication.Interfaces;

namespace StreamingApplication.Controllers;


[ApiController]
[Route("api/v1/[controller]")]
public class StreamingController : ControllerBase {


    private readonly IMediaService _mediaService;
    private readonly IMapper _mapper;

    private static readonly int s_bufferSize = 4096;


    public StreamingController(IMediaService mediaService, IMapper mapper) {
        _mediaService = mediaService;
        _mapper = mapper;
    }


    /*
     * Controller to stream a video file.
     */
    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Stream([FromRoute] int id) {
        var video = _mapper.Map<Media>(await _mediaService.GetByIdAsync(id));
        if (video == null || !System.IO.File.Exists(video.Path)) {
            return NotFound();
        }

        var fs = new FileStream(
            video.Path, 
            FileMode.Open, 
            FileAccess.Read, 
            FileShare.Read,
            bufferSize: s_bufferSize,
            useAsync: true
        );

        return File(fs, "video/mp4", enableRangeProcessing: true);
    }

}