using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using StreamingApplication.Data.Entities;
using StreamingApplication.Enumerations;
using StreamingApplication.Helpers.Parameters;
using StreamingApplication.Interfaces;


namespace StreamingApplication.Data.Repositories;

public class MediaRepository : IRepository<Media> {
    
    private readonly ApplicationDbContext _dbContext;


    public MediaRepository(ApplicationDbContext dbContext) {
        _dbContext = dbContext;
    }


    /* Method to add a media entity to the database. */
    public async Task<Media> CreateAsync(Media entity) {
        await _dbContext.Media.AddAsync(entity);
        await _dbContext.SaveChangesAsync();

        return entity;
    }


    /* Method to delete a media entity from the database. */
    public async Task<bool> DeleteAsync(int id) {
        var entity = await _dbContext.Media.FindAsync(id);
        if (entity == null) {
            return false;
        }

        _dbContext.Media.Remove(entity);
        await _dbContext.SaveChangesAsync();

        return true;
    }


    /* Method to get all media entities from the database. */
    public async Task<List<Media>> GetAllAsync(RequestParameters parameters) {
        var mediaParams = (MediaParameters)parameters;

        var entities = _dbContext.Media.AsQueryable();

        if (!string.IsNullOrEmpty(mediaParams.Query)) {
            entities = entities.Where(e =>
                EF.Functions.Like(e.Name, $"%{mediaParams.Query}%") ||
                EF.Functions.Like(e.Path, $"%{mediaParams.Query}%")
            );
        }

        if (mediaParams.MinDuration != 0) {
            entities = entities.Where(e => e.Duration.Seconds >= mediaParams.MinDuration);
        }

        if (mediaParams.MaxDuration != long.MaxValue) {
            entities = entities.Where(e => e.Duration.Seconds <= mediaParams.MaxDuration);
        }

        if (mediaParams.MinSize != 0) {
            entities = entities.Where(e => e.Size >= mediaParams.MinSize);
        }

        if (mediaParams.MaxSize != long.MaxValue) {
            entities = entities.Where(e => e.Size <= mediaParams.MaxSize);
        }

        if (!string.IsNullOrEmpty(mediaParams.Type)) {
            entities = _Filter(entities, mediaParams.Type);
        }

        if (!string.IsNullOrEmpty(mediaParams.SortBy)) {
            entities = _Sort(entities, mediaParams.SortBy, mediaParams.Descending);
        }

        return await entities.Skip(mediaParams.CalculateSkip())
            .Take(mediaParams.PageSize)
            .ToListAsync();
    }


    /* Method to get a specific media entity from the database. */
    public async Task<Media?> GetByIdAsync(int id) {
        return await _dbContext.Media.FindAsync(id);
    }


    /* Method to update a media entity in the database. */
    public async Task<Media?> UpdateAsync(int id, Media entity) {
        var currentEntity = await _dbContext.Media.FindAsync(id);
        if (currentEntity == null) {
            return null;
        }

        currentEntity.Name = entity.Name;

        var directoryPath = Path.GetDirectoryName(currentEntity.Path);
        if (directoryPath == null) {
            return null;
        }

        currentEntity.Path = Path.Combine(directoryPath, currentEntity.Name);

        await _dbContext.SaveChangesAsync();
        return currentEntity;
    }


    private static IQueryable<Media> _Filter(IQueryable<Media> entities, string type) {
        if (type.Equals("Movie", StringComparison.OrdinalIgnoreCase)) {
            return entities.Where(e => e.Type == MediaType.Movie);
        }

        if (type.Equals("Show", StringComparison.OrdinalIgnoreCase)) {
            return entities.Where(e => e.Type == MediaType.Show);
        }

        return entities;
    }


    // Method to sort media entities based on parameters.
    private static IQueryable<Media> _Sort(IQueryable<Media> entities, string sortBy, bool descending) {
        if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase)) {
            return descending ? entities.OrderByDescending(e => e.Name) : entities.OrderBy(e => e.Name);
        }

        if (sortBy.Equals("CreatedAt", StringComparison.OrdinalIgnoreCase)) {
            return descending ? entities.OrderByDescending(e => e.CreatedAt) : entities.OrderBy(e => e.CreatedAt);
        }

        if (sortBy.Equals("UpdatedAt", StringComparison.OrdinalIgnoreCase)) {
            return descending ? entities.OrderByDescending(e => e.UpdatedAt) : entities.OrderBy(e => e.UpdatedAt);
        }

        if (sortBy.Equals("Size", StringComparison.OrdinalIgnoreCase)) {
            return descending ? entities.OrderByDescending(e => e.Size) : entities.OrderBy(e => e.Size);
        }

        if (sortBy.Equals("Duration", StringComparison.OrdinalIgnoreCase)) {
            return descending ? entities.OrderByDescending(e => e.Duration) : entities.OrderBy(e => e.Duration);
        }

        return entities;
    }
}