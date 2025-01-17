using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StreamingApplication.Data.DTOs.Media;
using StreamingApplication.Data.Entities;
using StreamingApplication.Enumerations;
using StreamingApplication.Helpers.Parameters;
using StreamingApplication.Helpers.Response;
using StreamingApplication.Interfaces;


namespace StreamingApplication.Services;

public class MediaService : IMediaService {
    private readonly IFileService _fileService;
    private readonly FFprobeService _ffprobe;
    private readonly ILogger<MediaService> _logger;
    private readonly IMapper _mapper;
    private readonly IRepository<Media> _mediaRepository;
    private readonly IWebHostEnvironment _env;


    public MediaService(IFileService fs, FFprobeService ffp, ILogger<MediaService> log, IMapper mapper,
        IRepository<Media> mr, IWebHostEnvironment env) {
        _fileService = fs;
        _ffprobe = ffp;
        _logger = log;
        _mapper = mapper;
        _mediaRepository = mr;
        _env = env;
    }


    /* Method to create a media entity, if the file is already uploaded
     * and does not have a corresponding entity. */
    public async Task<int> CreateAsync(MediaCreateDTO createDTO) {
        if (!Path.Exists(createDTO.Path)) {
            _logger.LogWarning($"The file at: {createDTO.Path} is not uploaded to the server.");
            return -1;
        }

        var entity = _mapper.Map<Media>(createDTO);

        var duration = await _ffprobe.GetDurationAsync(entity.Path);
        if (duration == null) {
            _logger.LogWarning($"Failed to get duration of media file: {entity.Path}");
            return -2;
        }

        var size = _fileService.GetSize(entity.Path);
        if (size == -1) {
            _logger.LogWarning($"Failed to get size of media file: {entity.Path}");
            return -3;
        }

        entity.Name = Path.GetFileName(entity.Path);

        Media createdEntity;

        try {
            createdEntity = await _mediaRepository.CreateAsync(entity);
        } catch (DbUpdateException ex) {
            _logger.LogWarning($"Failed to save entity to database: {ex.Message}");
            return -4;
        }

        return createdEntity.Id;
    }


    /* Method to delete a entity. */
    public async Task<bool> DeleteAsync(int id) {
        return await _mediaRepository.DeleteAsync(id);
    }


    /* Method to get all media entries in the database. */
    public async Task<List<MediaListDTO>> GetAllAsync(MediaParameters parameters) {
        var entities = await _mediaRepository.GetAllAsync(parameters);
        return _mapper.Map<List<MediaListDTO>>(entities);
    }


    /* Method to get a specific media entity by id. */
    public async Task<MediaDTO?> GetByIdAsync(int id) {
        var entity = await _mediaRepository.GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<MediaDTO>(entity);
    }


    /* Method to update a media entity. */
    public async Task<MediaDTO?> UpdateAsync(int id, MediaUpdateDTO updateDTO) {
        var currentEntity = _mapper.Map<Media>(updateDTO);
        var updatedEntity = await _mediaRepository.UpdateAsync(id, currentEntity);

        return _mapper.Map<MediaDTO>(updatedEntity);
    }


    /* Method to upload a media file to storage, and store metadata in the database. */
    public async Task<int> UploadAsync(IFormFile file, MediaUploadDTO uploadDTO) {
        _logger.LogInformation($"Media file upload started. Filename: {file.FileName}, Size: {file.Length} bytes.");

        if (!_fileService.Validate(file)) {
            _logger.LogWarning($"Media file validation failed for: {file.FileName}.");
            return -1;
        }

        var entity = _mapper.Map<Media>(uploadDTO);

        var dir = entity.Type == MediaType.Movie ? "Movies" : "Shows";
        var uploadDirectory = Path.Combine(
            _env.ContentRootPath,
            "MediaStorage",
            dir
        );

        entity.Name = file.FileName;

        try {
            entity.Path = await _fileService.SaveAsync(file, uploadDirectory);
        } catch (DirectoryNotFoundException e) {
            _logger.LogError($"Upload directory not found: {uploadDirectory}");
            return -2;
        }

        entity.Size = file.Length;

        var duration = await _ffprobe.GetDurationAsync(entity.Path);
        if (duration == null) {
            return -3;
        }

        entity.Duration = duration.Value;

        await _mediaRepository.CreateAsync(entity);

        _logger.LogInformation("Media file upload completed.");

        return entity.Id;
    }
}