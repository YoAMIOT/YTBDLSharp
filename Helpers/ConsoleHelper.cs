namespace YTBDLSharp.Helpers
{
    /// <summary>
    /// A static helper class for console operations, including printing banners, separators, progress bars, and messages with colors.
    /// </summary>
    public static class ConsoleHelper
    {
        /// <summary>
        /// Prints the application banner in the console.
        /// </summary>
        public static void PrintBanner()
        {
            WriteLine("""
            ╔════════════════════════════════════════════════════════════╗
            ║                                                            ║
            ║         ██╗   ██╗████████╗██████╗ ██████╗ ██╗              ║
            ║         ╚██╗ ██╔╝╚══██╔══╝██╔══██╗██╔══██╗██║              ║
            ║          ╚████╔╝    ██║   ██████╔╝██║  ██║██║              ║
            ║           ╚██╔╝     ██║   ██╔══██╗██║  ██║██║              ║
            ║            ██║      ██║   ██████╔╝██████╔╝███████╗         ║
            ║            ╚═╝      ╚═╝   ╚═════╝ ╚═════╝ ╚══════╝         ║
            ║                                                            ║
            ║       Y O U T U B E   A U D I O   D O W N L O A D E R      ║
            ║                                                            ║
            ║                         YTBDLSharp                         ║
            ╚════════════════════════════════════════════════════════════╝
            """, ConsoleColor.DarkYellow);
        }

        /// <summary>
        /// Prints a separator line in the console.
        /// </summary>
        public static void PrintSeparator()
        {
            WriteLine("══════════════════════════════════════════════════════════════", ConsoleColor.DarkYellow);
        }

        /// <summary>
        /// Returns a progress bar that can be used to display download progress in the console.
        /// </summary>
        /// <returns>A progress bar that can be used to display download progress.</returns>        
        public static Progress<double> GetProgressBar()
        {
            return new Progress<double>(value =>
            {
                const int barWidth = 30;

                int filled = (int)(value * barWidth);

                string bar = new string('█', filled) +
                             new string('═', barWidth - filled);

                Console.Write($"\r    * Downloading file: [{bar}] {value:P0}");
            });
        }

        /// <summary>
        /// Prints a message indicating that the download has completed.
        /// </summary>
        public static void PrintFileDownloaded()
        {
            WriteLine("""
            ╭────────────────────────────────────────────────────────────╮
            │                     ✓ FILE DOWNLOADED                      │
            ╰────────────────────────────────────────────────────────────╯
            """, ConsoleColor.Green);
        }

        /// <summary>
        /// Prints a message indicating that the playlist has been downloaded, along with the number of tracks downloaded.
        /// </summary>
        /// <param name="fileCount">The number of downloaded files.</param>
        public static void PrintPlaylistDownloaded(int fileCount)
        { 
            const int width = 60; 
            string message = $"✓ {fileCount} track(s) downloaded";
            int totalPadding = width - message.Length;
            int leftPadding = totalPadding / 2; 
            int rightPadding = totalPadding - leftPadding;
            WriteLine("╭────────────────────────────────────────────────────────────╮", ConsoleColor.Green);
            WriteLine("│                                                            │", ConsoleColor.Green);
            WriteLine("│                 ♪  PLAYLIST DOWNLOADED  ♪                  │", ConsoleColor.Green);
            WriteLine("│                                                            │", ConsoleColor.Green);
            WriteLine($"│{new string(' ', leftPadding)}{message}{new string(' ', rightPadding)}│", ConsoleColor.Green);
            WriteLine("│                                                            │", ConsoleColor.Green);
            WriteLine("│                        YTBDLSharp                          │", ConsoleColor.Green);
            WriteLine("│                                                            │", ConsoleColor.Green);
            WriteLine("╰────────────────────────────────────────────────────────────╯", ConsoleColor.Green);
        }

        /// <summary>
        /// Prints a list of failed downloads in the console, if any.
        /// </summary>
        /// <param name="failedDownloads">The list of failed downloads video titles.</param>
        public static void PrintPlaylistFailedDownloads(List<string> failedDownloads)
        {
            if (failedDownloads != null && failedDownloads.Count() > 0)
            {
                WriteLine("  Failed downloads:");
                foreach (string dl in failedDownloads)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("    ✗");
                    Console.ResetColor();
                    Console.WriteLine($" {dl}");
                }
            }
        }

        /// <summary>
        /// Prints a line with a given color.
        /// </summary>
        /// <param name="line">The line to print.</param>
        /// <param name="color">The color to use.</param>
        public static void WriteLine(string? line = null, ConsoleColor? color = null)
        {
            if (color != null)
            {
                Console.ForegroundColor = color.Value;
            }

            if (line != null)
            {
                Console.WriteLine(line);
            } 
            else
            {
                Console.WriteLine();
            }
        
            Console.ResetColor();
        }

        /// <summary>
        /// Prints the FFMPEG warning.
        /// </summary>
        public static void PrintFFMPEGWarning()
        {
            WriteLine("      /!\\ Error: FFmpeg could not be found. Make sure FFmpeg is installed and available in PATH. (see: https://ffmpeg.org/download.html)", ConsoleColor.Red);
        }
    }
}
