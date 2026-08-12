# YTBDLSharp

A simple command-line YouTube audio downloader written in **C# / .NET 10**.

YTBDLSharp uses [YoutubeExplode](https://github.com/Tyrrrz/YoutubeExplode) to retrieve YouTube video and audio streams.

## Features

* Download audio from YouTube URLs
* Command-line interface
* Custom output directory
* Automatically create missing output directories
* Audio stream selection through YoutubeExplode
* Native .NET 10 application
* Designed for Linux

## Requirements

### Runtime

The published Linux version is self-contained, so **.NET does not need to be installed** on the target machine.

### FFmpeg

FFmpeg is required for audio conversion.

On Arch Linux:

```bash
sudo pacman -S ffmpeg
```

Verify the installation:

```bash
ffmpeg -version
```

## Download

[Download the latest release](../../releases/latest)

## Building

Clone the repository:

```bash
git clone <repository-url>
cd ytbdlsharp
```

Restore dependencies:

```bash
dotnet restore
```

Build the project:

```bash
dotnet build
```

Run it directly with:

```bash
dotnet run
```

## Usage

```text
YTBDLSharp <YouTube URL> [output directory]
```

Example:

```bash
./YTBDLSharp "https://www.youtube.com/watch?v=..."
```

Specify an output directory:

```bash
./YTBDLSharp "https://www.youtube.com/watch?v=..." ~/Music
```

If the specified directory does not exist, YTBDLSharp will ask whether it should be created.

## Dependencies

* [.NET 10](https://dotnet.microsoft.com/)
* [YoutubeExplode](https://github.com/Tyrrrz/YoutubeExplode)
* [FFmpeg](https://ffmpeg.org/)

## Legal Notice

This project is intended for downloading content that you have the right or permission to download.

Users are responsible for complying with YouTube's Terms of Service, copyright law, and any other applicable laws.

## License

This project is licensed under the MIT License.
