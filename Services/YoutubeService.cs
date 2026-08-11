using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Videos.Streams;
using YTBDLSharp.Models;
using YTBDLSharp.Helpers;

namespace YTBDLSharp.Services
{
    /// <summary>
    /// Service class responsible for handling Youtube downloads.
    /// </summary>
    public class YoutubeService
    {
        /// <summary>
        /// The Youtube client instance used to make requests to Youtube.
        /// </summary>
        private readonly YoutubeClient _ytbClient;

        /// <summary>
        /// The audio converter instance used to convert downloaded audio streams to MP3 format.
        /// </summary>
        private readonly AudioConverter _audioConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="YoutubeService"/> class.
        /// </summary>
        public YoutubeService()
        {
            _ytbClient = new YoutubeClient();
            _audioConverter = new AudioConverter();
        }

        /// <summary>
        /// Downloads the resource (video or playlist) based on the provided command line options and resource type.
        /// </summary>
        /// <param name="options">The command line options.</param>
        /// <param name="type">The resource type.</param>
        /// <returns>A task.</returns>
        public async Task DownloadResource(CommandLineOptions options, YoutubeResourceType type)
        {
            // Checks arguments for null or empty values and throws exceptions if any are found.
            ArgumentNullException.ThrowIfNull(options);
            ArgumentNullException.ThrowIfNullOrEmpty(options.Url);
            ArgumentNullException.ThrowIfNullOrEmpty(options.OutputDirectory);

            // Check if the output directory exists. If not, prompt the user to create it or cancel the operation.
            if (Directory.Exists(options.OutputDirectory) == false)
            {
                ConsoleHelper.PrintSeparator();
                Console.WriteLine($"  Directory '{options.OutputDirectory}' does not exist.");
                Console.Write("  Do you want to create it? (Y)es or (N)o: ");

                var answer = Console.ReadLine();

                if (answer?.Equals("Y", StringComparison.OrdinalIgnoreCase) == true)
                {
                    Directory.CreateDirectory(options.OutputDirectory);
                    Console.WriteLine("  Directory created.");
                }
                else
                {
                    Console.WriteLine("  Operation cancelled.");
                    ConsoleHelper.PrintSeparator();
                    return;
                }
            }

            // Depending on the resource type, call the appropriate method to download the audio.
            if (type == YoutubeResourceType.Video)
            {
                ConsoleHelper.PrintSeparator();
                await DownloadAudioAsync(options.Url, options.OutputDirectory);
            }
            else if (type == YoutubeResourceType.Playlist)
            {
                await DownloadPlayListAudioAsync(options);
            }
        }

        /// <summary>
        /// Downloads the audio streams of all videos in a playlist based on the provided command line options.
        /// </summary>
        /// <param name="options">The command line options.</param>
        /// <returns>A task.</returns>
        /// <exception cref="DirectoryNotFoundException">When the output directory does not exist.</exception>
        /// <exception cref="Exception">When an error occurs while downloading the playlist.</exception>
        public async Task DownloadPlayListAudioAsync(CommandLineOptions options)
        {
            // Checks arguments for null or empty values and throws exceptions if any are found.
            ArgumentNullException.ThrowIfNull(options);
            ArgumentNullException.ThrowIfNullOrEmpty(options.Url);
            ArgumentNullException.ThrowIfNullOrEmpty(options.OutputDirectory);

            ConsoleHelper.PrintSeparator();

            // Check if the output directory exists. If not, throw a DirectoryNotFoundException.
            if (Directory.Exists(options.OutputDirectory) == false)
            {
                throw new DirectoryNotFoundException($"Output directory does not exist {options.OutputDirectory}");
            }

            ConsoleHelper.WriteLine("    * Getting playlist information...");

            // Get the videos in the playlist using the Youtube client.
            var videos = await _ytbClient.Playlists.GetVideosAsync(options.Url);

            // If no videos are found, throw an exception indicating that the playlist could not be found.
            if (videos == null)
            {
                throw new Exception($"Playlist could not be found for URL: {options.Url}");
            }

            var videoList = videos.ToList();
            ConsoleHelper.WriteLine($"    * Found {videoList.Count} video(s) in playlist.");

            int successCount = 0;
            List<string> failed = new List<string>();

            // Loop through each video in the playlist and attempt to download its audio stream.
            for (int i = 0; i < videoList.Count; i++)
            {
                var video = videoList[i];
                ConsoleHelper.PrintSeparator();
                ConsoleHelper.WriteLine($"  [{i + 1}/{videoList.Count}] {video.Title}");
                try
                {
                    await DownloadAudioAsync(
                        video.Url,
                        options.OutputDirectory
                    );
                    successCount ++;
                }
                catch (Exception e)
                {
                    ConsoleHelper.WriteLine($"      /!\\ Failed to download: {e.Message}", ConsoleColor.Red);
                    failed.Add(video.Title);
                }
            }

            ConsoleHelper.PrintPlaylistDownloaded(successCount);
            ConsoleHelper.PrintPlaylistFailedDownloads(failed);
        }

        /// <summary>
        /// Downloads the audio stream of a single video based on the provided URL and output directory.
        /// </summary>
        /// <param name="url">The URL of the Youtube video to download the audio from.</param>
        /// <param name="outputDirectory">The directory where to download the audio from the video to.</param>
        /// <returns>A Task.</returns>
        /// <exception cref="DirectoryNotFoundException">When the output directory does not exist.</exception>
        /// <exception cref="Exception">When an error occurs while downloading the audio.</exception>
        public async Task DownloadAudioAsync(string url, string outputDirectory)
        {
            // Check if the output directory exists. If not, throw a DirectoryNotFoundException.
            if (Directory.Exists(outputDirectory) == false)
            {
                throw new DirectoryNotFoundException($"Output directory does not exist! ({outputDirectory})");
            }

            // Get the manifest of the video to retrieve available streams.
            var manifest = await _ytbClient.Videos.Streams.GetManifestAsync(url);

            // If the manifest is null, throw an exception indicating that the manifest could not be retrieved.
            if (manifest == null)
            {
                throw new Exception($"Manifest is null for URL: {url}.");
            }

            // Get the audio stream with the highest bitrate from the manifest.
            var audioStream = manifest
                .GetAudioOnlyStreams()
                .GetWithHighestBitrate();

            // If no audio stream is found, throw an exception indicating that no audio stream could be found for the video.
            if (audioStream == null)
            {
                throw new Exception($"There was no audio stream found for URL: {url}");
            }

            ConsoleHelper.WriteLine("    * Getting video datas...");

            // Get the video information using the Youtube client.
            var video = await _ytbClient.Videos.GetAsync(url);

            // If no title is set use a timestamp.
            string fileTitle = video != null && string.IsNullOrEmpty(video.Title) == false ? video.Title : DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

            // Get the invalid characters for file names and replace them with underscores to create a safe file name.
            var invalidChars = Path.GetInvalidFileNameChars();
            // Replace invalid characters.
            string safeTitle = string.Concat(
                fileTitle.Select(c => invalidChars.Contains(c) ? '_' : c)
            );

            // Create a temporary file name and path for the downloaded audio stream.
            string temporaryFileName = $"{safeTitle}.{audioStream.Container.Name}";
            string temporaryFilePath = Path.Combine(outputDirectory, temporaryFileName);

            if(video == null || string.IsNullOrEmpty(video.Title) == true)
            {
                ConsoleHelper.WriteLine($"    * Output file name will be: {safeTitle} since the video title was empty");
            }

            // Download the audio stream to the temporary file path while displaying a progress bar in the console.
            await _ytbClient.Videos.Streams.DownloadAsync(audioStream, temporaryFilePath, ConsoleHelper.GetProgressBar());
            ConsoleHelper.WriteLine();

            // Create the final file name and path for the converted MP3 file.
            string finalFileName = $"{safeTitle}.mp3";
            string finalFilePath = Path.Combine(outputDirectory, finalFileName);

            ConsoleHelper.WriteLine($"    * Converting file to MP3...");
            // Convert the downloaded audio stream to MP3 format and save it to the final file path.
            await _audioConverter.ConvertToMp3Async(temporaryFilePath, finalFilePath);

            // Delete the temporary file after conversion and print a message indicating that the file has been downloaded.
            File.Delete(temporaryFilePath);
            ConsoleHelper.PrintFileDownloaded();
        }
    }
}