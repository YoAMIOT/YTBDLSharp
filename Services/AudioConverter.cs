using System.Diagnostics;
using YTBDLSharp.Helpers;

namespace YTBDLSharp.Services
{
    /// <summary>
    /// Service class responsible for converting audio files to MP3 format using FFmpeg.
    /// </summary>
    public class AudioConverter
    {
        /// <summary>
        /// Converts the input audio file to MP3 format and saves it to the specified output path using FFmpeg.
        /// </summary>
        /// <param name="inputPath">The path to the input audio file.</param>
        /// <param name="outputPath">The path where the MP3 file will be created.</param>
        /// <returns>A task.</returns>
        /// <exception cref="FileNotFoundException">When the input audio file does not exist.</exception>
        /// <exception cref="DirectoryNotFoundException">When the output directory does not exist.</exception>
        /// <exception cref="InvalidOperationException">When FFmpeg fails to convert the audio file.</exception>
        public async Task ConvertToMp3Async(string inputPath, string outputPath)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(inputPath);
            ArgumentNullException.ThrowIfNullOrEmpty(outputPath);

            // Check if the input audio file exists. If not, throw a FileNotFoundException.
            if (File.Exists(inputPath) == false)
            {
                throw new FileNotFoundException($"Input audio file does not exist: {inputPath}", inputPath);
            }

            // Get the directory of the output path to ensure it exists before attempting to convert the audio file.
            string? outputDirectory = Path.GetDirectoryName(outputPath);

            // If the directory does not exist, throw a DirectoryNotFoundException.
            if (Directory.Exists(outputDirectory) == false){
                throw new DirectoryNotFoundException($"Output directory does not exist {outputDirectory}");
            }

            // Set up the process start information for FFmpeg, including the input and output paths.
            var startInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            // Add the necessary arguments for FFmpeg to convert the audio file to MP3 format.
            startInfo.ArgumentList.Add("-y");
            startInfo.ArgumentList.Add("-i");
            startInfo.ArgumentList.Add(inputPath);
            startInfo.ArgumentList.Add("-vn");
            startInfo.ArgumentList.Add("-codec:a");
            startInfo.ArgumentList.Add("libmp3lame");
            startInfo.ArgumentList.Add("-q:a");
            startInfo.ArgumentList.Add("2");
            startInfo.ArgumentList.Add(outputPath);

            // Create a new process to run FFmpeg with the specified start information.
            using var process = new Process
            {
                StartInfo = startInfo
            };

            try
            {
                // Start the FFmpeg process to convert the audio file.
                process.Start();
            }
            catch
            {
                ConsoleHelper.PrintFFMPEGWarning();
                throw new InvalidOperationException("Could not start FFmpeg.");
            }

            // Read the standard error output from FFmpeg to capture any error messages during the conversion process.
            string errorOutput = await process.StandardError.ReadToEndAsync();
            await process.StandardOutput.ReadToEndAsync();

            // Wait for the FFmpeg process to exit.
            await process.WaitForExitAsync();

            // If the exit code is not 0, it indicates that FFmpeg failed to convert the audio file, so we throw an InvalidOperationException with the error output.
            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException($"FFmpeg failed with exit code {process.ExitCode}.{Environment.NewLine}" + errorOutput);
            }
        }
    }
}
