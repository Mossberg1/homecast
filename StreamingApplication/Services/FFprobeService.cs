using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;


namespace StreamingApplication.Services;

public class FFprobeService {
    private readonly ILogger<FFprobeService> _logger;


    public FFprobeService(ILogger<FFprobeService> logger) {
        _logger = logger;
    }


    /* Method to calculate the duration of a mp4 file*/
    public async Task<TimeSpan?> GetDurationAsync(string path) {
        try {
            var pInfo = new ProcessStartInfo {
                FileName = "ffprobe",
                Arguments = $"-v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 \"{path}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = pInfo };

            process.Start();

            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (process.ExitCode == 0 && double.TryParse(output.Trim(), out double seconds)) {
                return TimeSpan.FromSeconds(seconds);
            }

            _logger.LogWarning($"Failed to parse duration from ffprobe output: {output}");
            return null;
        } catch (Exception ex) {
            _logger.LogError($"Failed to get media duration: {ex.Message}");
            return null;
        }
    }
}