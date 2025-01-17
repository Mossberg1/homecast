using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StreamingApplication.Interfaces;

namespace StreamingApplication.Services;

public class FileService : IFileService {
    private readonly string[] _allowedExtensions = {
        ".mp4"
    };

    private readonly long _maxSize = 8589934592; // 8GB

    private readonly ILogger<FileService> _logger;


    public FileService(ILogger<FileService> logger) {
        _logger = logger;
    }


    /* Method to get size of a file from a certain path. */
    public long GetSize(string path) {
        FileInfo info;

        try {
            info = new FileInfo(path);
        } catch (Exception ex) {
            _logger.LogWarning($"Failed to get file size. Exception: {ex.GetType()}");
            return -1;
        }

        return info.Length;
    }


    /* Method to validate a file before uploading to storage. */
    public bool Validate(IFormFile file) {
        if (!_allowedExtensions.Contains(Path.GetExtension(file.FileName))) {
            return false;
        }

        if (file.Length > _maxSize) {
            return false;
        }

        return true;
    }


    /* Method to save a file to disk. */
    public async Task<string> SaveAsync(IFormFile file, string uploadDirectory) {
        if (!Directory.Exists(uploadDirectory)) {
            throw new DirectoryNotFoundException("[FileService.SaveAsync()] Upload directory does not exist.");
        }

        var filePath = Path.Combine(
            uploadDirectory,
            file.FileName
        );

        using (var fs = new FileStream(filePath, FileMode.Create)) {
            await file.CopyToAsync(fs);
        }

        return filePath;
    }
}