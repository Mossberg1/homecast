using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StreamingApplication.Data.DTOs.Media;
using StreamingApplication.Data.DTOs.Movie;
using StreamingApplication.Data.Entities;
using StreamingApplication.Forms;
using StreamingApplication.Helpers.Parameters;
using StreamingApplication.Interfaces;


namespace StreamingApplication.Services;


public class MovieService : IMovieService {

    private readonly IMapper _mapper;
    private readonly IRepository<Movie> _movieRepository;
    private readonly IMediaService _mediaService;
    private readonly ILogger<MovieService> _logger;


    public MovieService(IMapper mapper, IRepository<Movie> movieRepository, IMediaService mediaService, ILogger<MovieService> logger) {
        _mapper = mapper;
        _movieRepository = movieRepository;
        _mediaService = mediaService;
        _logger = logger;
    }

    
    // Method to create a entity.
    public async Task<int> CreateAsync(MovieCreateDTO createDTO) {
        var entity = _mapper.Map<Movie>(createDTO);

        Movie createdEntity;
        
        try {
            createdEntity = await _movieRepository.CreateAsync(entity);
        } catch (DbUpdateException ex) {
            _logger.LogWarning($"Failed to create entity, message: {ex.Message}");
            return -1;
        }

        return createdEntity.Id;
    }
    
    
    /* Method to create a movie entity and upload a file at the same time. */
    public async Task<int> CreateAndUploadAsync(MovieUploadDTO movieUploadDTO, MediaUploadDTO mediaUploadDTO, IFormFile file) {
        var mediaId = await _mediaService.UploadAsync(file, mediaUploadDTO);
        if (mediaId < 0) {
            _logger.LogWarning($"Failed to upload media file, filename: {file.FileName}");
            return -1;
        }

        var entity = _mapper.Map<Movie>(movieUploadDTO);
        entity.MediaFileId = mediaId;

        Movie createdEntity;

        try {
            createdEntity = await _movieRepository.CreateAsync(entity);
        } catch (DbUpdateException ex) {
            _logger.LogWarning($"Failed to create entity, message: {ex.Message}");
            return -2;
        }

        return createdEntity.Id;
    }
    
    
    // Method to delete a entity.
    public async Task<bool> DeleteAsync(int id) {
        return await _movieRepository.DeleteAsync(id);
    }
    
    
    // Method to get all entities.
    public async Task<List<MovieListDTO>> GetAllAsync(MovieParameters parameters) {
        var entities = await _movieRepository.GetAllAsync(parameters);
        return _mapper.Map<List<MovieListDTO>>(entities);
    }
    
    
    // Method to get a entity by id.
    public async Task<MovieDTO?> GetByIdAsync(int id) {
        var entity = await _movieRepository.GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<MovieDTO>(entity);
    }
    
    
    // Method to update a entity in the database.
    public async Task<MovieDTO?> UpdateAsync(int id, MovieUpdateDTO updateDTO) {
        var entity = _mapper.Map<Movie>(updateDTO);
        var updatedEntity = await _movieRepository.UpdateAsync(id, entity);
        if (updatedEntity == null) {
            _logger.LogWarning($"Failed to update entity with id: {id}");
            return null;
        }

        return _mapper.Map<MovieDTO>(updatedEntity);
    }
}