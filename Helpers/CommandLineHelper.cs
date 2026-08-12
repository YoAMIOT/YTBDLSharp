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
            Console.WriteLine("Usage:");
            ConsoleHelper.WriteLine("  ytbdlsharp --help", ConsoleColor.Green);
            Console.WriteLine("    (Prints this help message.)");
            ConsoleHelper.WriteLine("  ytbdlsharp -h", ConsoleColor.Green);
            Console.WriteLine("    (Prints this help message.)");
            ConsoleHelper.WriteLine("  ytbdlsharp <youtube-url>", ConsoleColor.Green);
            Console.WriteLine("    (Current directory will be used as the output directory.)");
            ConsoleHelper.WriteLine("  ytbdlsharp <youtube-url> [outputDirectory]", ConsoleColor.Green);
            Console.WriteLine("    (Specifies the output directory for the downloaded files.)");
        }

        /// <summary>
        /// Parses the command line arguments and returns a <see cref="CommandLineOptions"/> object if valid, or null if invalid.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns>The parsed command line options, or null if the arguments are invalid.</returns>
        public static CommandLineOptions? ParseCommandLineArguments(string[] args)
        {
            // If first argument is "--help" or "-h", print the usage information and return null.
            if (args.Length > 0 && (args[0] == "--help" || args[0] == "-h"))
            {
                PrintUsage();
                return null;
            }

            // If no arguments are provided, asks for URL and output directory.
            if (args.Length == 0)
            {
                var promptArgs = PrintArgumentPrompt();

                ConsoleHelper.PrintSeparator();

                // If the prompt returns null, it means the user did not provide valid input, so we return null.
                if (promptArgs == null)
                {
                    return null;
                }

                args = promptArgs;
            }
            
            // Get first argument as the YouTube URL.
            string url = args[0];

            // If the provided URL is not a valid absolute URI, print an error message and return null.
            if (YoutubeUrlParser.ValidateURL(url) == false)
            {
                ConsoleHelper.WriteLine("      /!\\ Unsupported YouTube URL.", ConsoleColor.Red);
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

        /// <summary>
        /// Prints a prompt to the console asking the user for required arguments (YouTube URL and output directory) and reads the input from the console.
        /// </summary>
        /// <returns>The array of input strings.</returns>
        private static string[]? PrintArgumentPrompt()
        {
            Console.WriteLine("Please provide the required arguments:");
            ConsoleHelper.WriteLine("  1. YouTube URL (required)", ConsoleColor.Green);
            Console.WriteLine("    Exemple: https://www.youtube.com/watch?v=Aq5WXmQQooo");

            // Read the input from the console for the YouTube URL.
            string? urlInput = Console.ReadLine();

            // If the provided URL is not a valid absolute URI, print an error message and return null.
            if (string.IsNullOrEmpty(urlInput) || YoutubeUrlParser.ValidateURL(urlInput) == false)
            {
                ConsoleHelper.WriteLine("      /!\\ Unsupported YouTube URL.", ConsoleColor.Red);
                return null;
            }

            ConsoleHelper.WriteLine("  2. Output directory (optional, defaults to current directory)", ConsoleColor.Green);
            Console.WriteLine("    Exemple: ~/Downloads/YTBDLSharp");

            // Read the input from the console for the output directory.
            string? directoryInput = Console.ReadLine();

            if (string.IsNullOrEmpty(directoryInput) == false )
            {
                return new string[] { urlInput, directoryInput };
            } 
            else
            {
                return new string[] { urlInput};
            }
        }
    }
}