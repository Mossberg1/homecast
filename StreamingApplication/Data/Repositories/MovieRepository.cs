using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StreamingApplication.Data.Entities;
using StreamingApplication.Helpers.Parameters;
using StreamingApplication.Interfaces;

namespace StreamingApplication.Data.Repositories;

public class MovieRepository : IRepository<Movie> {

    private readonly ApplicationDbContext _dbContext;


    public MovieRepository(ApplicationDbContext dbContext) {
        _dbContext = dbContext;
    }


    /* Method to add a movie to the database. */
    public async Task<Movie> CreateAsync(Movie entity) {
        await _dbContext.Movie.AddAsync(entity);
        await _dbContext.SaveChangesAsync();

        return entity;
    }
    
    
    /* Method to delete a movie entity from the database. */ 
    public async Task<bool> DeleteAsync(int id) {
        var entity = await _dbContext.Movie.FindAsync(); 
        
        if (entity == null) {
            return false;
        }

        _dbContext.Movie.Remove(entity);
        await _dbContext.SaveChangesAsync();

        return true;
    }
    
    
    /* Method to get all movies from the database. */
    public async Task<List<Movie>> GetAllAsync(RequestParameters parameters) {
        var movieParameters = (MovieParameters)parameters;
        var entities = _dbContext.Movie.AsQueryable();

        if (!string.IsNullOrEmpty(movieParameters.Query)) {
            entities = entities.Where(e => 
                EF.Functions.Like(e.Name, $"%{movieParameters.Query}%") ||
                EF.Functions.Like(e.Genre.ToString(), $"%{movieParameters.Query}%")
            );
        }

        if (!string.IsNullOrEmpty(movieParameters.Genre)) {
            entities = entities.Where(e => 
                string.Equals(e.Genre.ToString(), movieParameters.Genre, StringComparison.OrdinalIgnoreCase)
            );
        }

        if (movieParameters.MinDuration > 0) {
            entities = entities.Where(e => e.MediaFile.Duration.Seconds >= movieParameters.MinDuration);
        }
        
        if (movieParameters.MaxDuration < long.MaxValue) {
            entities = entities.Where(e => e.MediaFile.Duration.Seconds <= movieParameters.MaxDuration);
        }
        
        if (!string.IsNullOrEmpty(movieParameters.SortBy)) {
            entities = _Sort(entities, movieParameters.SortBy, movieParameters.Descending);
        }

        return await entities.Skip(movieParameters.CalculateSkip())
            .Take(movieParameters.PageSize)
            .ToListAsync();
    }
    
    
    /* Method to get a movie entity by id. */
    public async Task<Movie?> GetByIdAsync(int id) {
        return await _dbContext.Movie.Include(m => m.MediaFile)
            .FirstOrDefaultAsync(m => m.Id == id);
    }
    
    
    /* Method to update a movie. */
    public async Task<Movie?> UpdateAsync(int id, Movie entity) {
        var currentEntity = await _dbContext.Movie.FindAsync(id);
        if (currentEntity == null) {
            return null;
        }

        currentEntity.Name = entity.Name;
        currentEntity.Description = entity.Description;

        await _dbContext.SaveChangesAsync();

        return currentEntity;
    }


    /* Method to sort entities based on paerameters. */
    private static IQueryable<Movie> _Sort(IQueryable<Movie> entities, string sortBy, bool descending) {
        if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase)) {
            return descending ? entities.OrderByDescending(e => e.Name) : entities.OrderBy(e => e.Name);
        }

        if (sortBy.Equals("CreatedAt", StringComparison.OrdinalIgnoreCase)) {
            return descending ? entities.OrderByDescending(e => e.CreatedAt) : entities.OrderBy(e => e.CreatedAt);
        }

        if (sortBy.Equals("UpdatedAt", StringComparison.OrdinalIgnoreCase)) {
            return descending ? entities.OrderByDescending(e => e.UpdatedAt) : entities.OrderBy(e => e.UpdatedAt);
        }

        if (sortBy.Equals("Genre", StringComparison.OrdinalIgnoreCase)) {
            return descending ? entities.OrderByDescending(e => e.Genre.ToString()) : entities.OrderBy(e => e.Genre.ToString());
        }

        if (sortBy.Equals("Duration", StringComparison.OrdinalIgnoreCase)) {
            return descending ? entities.OrderByDescending(e => e.MediaFile.Duration) : entities.OrderBy(e => e.MediaFile.Duration);
        }
        
        return entities;
    }
}