using YTBDLSharp.Helpers;
using YTBDLSharp.Models;
using YTBDLSharp.Services;

// Set the console output encoding to UTF-8 to support special characters.
Console.OutputEncoding = System.Text.Encoding.UTF8;

// Print the application banner.
ConsoleHelper.PrintBanner();

// Parse command line arguments.
CommandLineOptions? options = CommandLineHelper.ParseCommandLineArguments(args);

// If the options are null, it means the arguments were invalid or help was requested, so we exit the program.
if (options == null)
{
    return;
}

// Check if FFMPEG is available in the system. If not, print a warning and exit.
if (FfmpegHelper.IsFfmpegAvailable() == false)
{
    ConsoleHelper.PrintFFMPEGWarning();
    return;
}

Console.WriteLine($"  URL:              {options.Url}");
Console.WriteLine($"  Output directory: {options.OutputDirectory}");

// Parse the YouTube URL to determine the resource type (video, playlist, etc.). If the URL is unsupported, print an error message and exit.
if (!YoutubeUrlParser.TryParse(options.Url, out YoutubeResourceType resourceType))
{
    ConsoleHelper.WriteLine("      /!\\ Unsupported YouTube URL.", ConsoleColor.Red);
    return;
}

Console.WriteLine($"  Resource type:    {resourceType}");

// Create an instance of the YoutubeService to handle the download process.
YoutubeService ytb = new YoutubeService();

try
{
    // Attempt to download the resource based on the parsed options and resource type.
    await ytb.DownloadResource(options, resourceType);
}
catch (Exception e)
{
    ConsoleHelper.WriteLine($"      /!\\ Error: {e.Message}", ConsoleColor.Red);
}