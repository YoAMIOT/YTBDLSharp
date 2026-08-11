using YTBDLSharp.Models;

namespace YTBDLSharp.Helpers
{
    /// <summary>
    /// A static helper class for parsing command line arguments and handling command line operations.
    /// </summary>
    public static class CommandLineHelper
    {
        /// <summary>
        /// Prints the usage information for the command line application.
        /// </summary>
        private static void PrintUsage()
        {
            ConsoleHelper.WriteLine("Usage:");
            ConsoleHelper.WriteLine("  ytbdlsharp <youtube-url>");
            ConsoleHelper.WriteLine("    (Current directory will be used as the output directory)");
            ConsoleHelper.WriteLine("  ytbdlsharp <youtube-url> [outputDirectory]");
        }

        /// <summary>
        /// Parses the command line arguments and returns a <see cref="CommandLineOptions"/> object if valid, or null if invalid.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns>The parsed command line options, or null if the arguments are invalid.</returns>
        public static CommandLineOptions? ParseCommandLineArguments(string[] args)
        {
            // If no arguments are provided, print the usage information and return null.
            if (args.Length == 0)
            {
                PrintUsage();
                return null;
            }
            
            // Get first argument as the YouTube URL.
            string url = args[0];

            // If the provided URL is not a valid absolute URI, print an error message and return null.
            if (!Uri.TryCreate(url, UriKind.Absolute, out _))
            {
                ConsoleHelper.WriteLine("      /!\\ Invalid URL.", ConsoleColor.Red);
                return null;
            }

            // Determine the output directory based on the second argument or use the current directory if not provided.
            string destination = args.Length >= 2 ? ResolveDestination(args[1]) : Directory.GetCurrentDirectory();

            return new CommandLineOptions
            {
                Url = url,
                OutputDirectory = destination
            };
        }

        /// <summary>
        /// Resolves the destination path, handling special cases like "~" for the user's home directory.
        /// </summary>
        /// <param name="path">The path to resolve.</param>
        /// <returns>The resolved path.</returns>
        private static string ResolveDestination(string path)
        {
            // Handle the special case where the path is "~" which represents the user's home directory.
            if (path == "~")
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            }
            
            // Handle the case where the path starts with "~/", which also represents a path relative to the user's home directory.
            if (path.StartsWith("~/" , StringComparison.Ordinal))
            {
                path = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    path[2..]
                );
            }

            return Path.GetFullPath(path);
        }
    }
}