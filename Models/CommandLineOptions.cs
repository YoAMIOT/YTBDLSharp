namespace YTBDLSharp.Models
{
    /// <summary>
    /// Represents the command line options for the application.
    /// </summary>
    public class CommandLineOptions
    {
        /// <summary>
        /// Gets or sets the youtube URL of the video / playlist to download.
        /// </summary>
        public required string Url { get; init; }
        
        /// <summary>
        /// Gets or sets the output directory path where the downloaded files will be saved.
        /// </summary>
        public required string OutputDirectory { get; init; }
    }
}