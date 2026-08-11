using System.Diagnostics;

namespace YTBDLSharp.Helpers
{
    /// <summary>
    /// A static helper class for checking the availability of FFmpeg.
    /// </summary>
    public static class FfmpegHelper
    {
        /// <summary>
        /// Checks if FFmpeg is available in the system's PATH by attempting to run "ffmpeg -version".
        /// </summary>
        /// <returns><c>true</c> if FFmpeg is available; otherwise, <c>false</c>.</returns>
        public static bool IsFfmpegAvailable()
        {
            try
            {
                using var process = Process.Start(new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = "-version",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                });

                if (process == null)
                {
                    return false;
                }

                process.WaitForExit();

                return process.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
