using YTBDLSharp.Models;

namespace YTBDLSharp.Helpers
{
    /// <summary>
    /// A static helper class for parsing YouTube URLs and determining their resource types.
    /// </summary>
    public static class YoutubeUrlParser
    {
        /// <summary>
        /// A set of recognized YouTube hostnames for URL validation.
        /// </summary>
        private static readonly HashSet<string> YouTubeHosts = new(StringComparer.OrdinalIgnoreCase)
        {
            "youtube.com",
            "www.youtube.com",
            "m.youtube.com",
            "music.youtube.com"
        };

        /// <summary>
        /// Tries to parse the given YouTube URL and determine its resource type (video or playlist).
        /// </summary>
        /// <param name="url">The URL to parse.</param>
        /// <param name="resourceType">The detected resource type.</param>
        /// <returns><c>true</c> if the URL is a supported YouTube URL; otherwise, <c>false</c>.</returns>
        public static bool TryParse(string url, out YoutubeResourceType resourceType)
        {
            // Default to Video if not determined otherwise.
            resourceType = YoutubeResourceType.Video;

            if (ValidateURL(url) == false)
            {
                return false;
            }

            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
            {
                return false;
            }

            // Check the path and query parameters to determine if it's a video URL.
            if (uri.AbsolutePath.Equals("/watch", StringComparison.OrdinalIgnoreCase))
            {
                // Check for the presence of the "v" query parameter for video.
                if (GetQueryParameter(uri, "v") is not null)
                {
                    resourceType = YoutubeResourceType.Video;
                    return true;
                }

                // Check for the presence of the "list" query parameter for playlist.
                if (GetQueryParameter(uri, "list") is not null)
                {
                    resourceType = YoutubeResourceType.Playlist;
                    return true;
                }
            }

            // Check the path and query parameters to determine if it's a playlist URL.
            if (uri.AbsolutePath.Equals("/playlist", StringComparison.OrdinalIgnoreCase))
            {
                // Check for the presence of the "list" query parameter for playlist.
                if (GetQueryParameter(uri, "list") is not null)
                {
                    resourceType = YoutubeResourceType.Playlist;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if the given host is a recognized YouTube host.
        /// </summary>
        /// <param name="host">The host to check.</param>
        /// <returns><c>true</c> if the host is a recognized YouTube host; otherwise, <c>false</c>.</returns>
        private static bool IsYouTubeHost(string host)
        {
            return YouTubeHosts.Contains(host);
        }

        /// <summary>
        /// Retrieves the value of a specific query parameter from the given URI.
        /// </summary>
        /// <param name="uri">The URI to parse.</param>
        /// <param name="parameter">The query parameter to retrieve.</param>
        /// <returns>The value of the query parameter, or <c>null</c> if not found.</returns>
        private static string? GetQueryParameter(Uri uri, string parameter)
        {
            // Split the query string into individual parameters and search for the specified parameter.
            string[] parameters = uri.Query
                .TrimStart('?')
                .Split('&', StringSplitOptions.RemoveEmptyEntries);

            // Iterate through the parameters to find the specified one and return its value if found.
            foreach (string item in parameters)
            {
                // Split the parameter into key-value pairs.
                string[] parts = item.Split('=', 2);

                // Check if the parameter matches the specified one and return its value if found.
                if (parts.Length == 2 && parts[0].Equals(parameter, StringComparison.OrdinalIgnoreCase))
                {
                    return Uri.UnescapeDataString(parts[1]);
                }
            }

            return null;
        }

        /// <summary>
        /// Validates whether the provided URL is a valid YouTube URL.
        /// </summary>
        /// <param name="url">The URL to validate.</param>
        /// <returns><c>true</c> if the URL is valid; otherwise, <c>false</c>.</returns>
        public static bool ValidateURL(string url)
        {
            // Validate the URL format.
            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
            {
                return false;
            }

            // Check if the host is a recognized YouTube host.
            if (!IsYouTubeHost(uri.Host))
            {
                return false;
            }

            return true;
        }
    }
}