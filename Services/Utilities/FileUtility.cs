using Microsoft.AspNetCore.Http;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using System.Security.Policy;
using UnidecodeSharpFork;

public static class FileUtility
{
// #if DEBUG
//     const string serverRoot = "..\\ComicShelf\\wwwroot";
// #else
//     const string serverRoot = "/volume1/web/publish/wwwroot";
// #endif

    private const string serverRoot = "wwwroot";

    const string imageDir = "images";

    //public static string RestoreCover(string seriesName, int volumeNumber, VolumeCover cover)
    //{
    //    var escapedSeriesName = seriesName.Unidecode().Replace(Path.GetInvalidFileNameChars(), string.Empty);

    //    var localDirectory = Path.Combine(serverRoot, imageDir, "Series", escapedSeriesName);
    //    var ext = cover.Extention;
    //    if (ext == string.Empty)
    //    {
    //        ext = ".jpg";
    //    }
    //    var filename = $"{escapedSeriesName} {volumeNumber}{ext}";
    //    var localPath = Path.Combine(localDirectory, filename);

    //    if (!Directory.Exists(localDirectory))
    //        Directory.CreateDirectory(localDirectory);

    //    File.WriteAllBytes(localPath, cover.Cover);

    //    return Path.Combine(imageDir, "Series", escapedSeriesName, filename);
    //}

    internal static string DownloadFileFromWeb(string url, string seriesName, int volumeNumber, out byte[] image, out string extention)
    {
        extention = new FileInfo(url).Extension;
        var escapedSeriesName = seriesName.Unidecode().Replace(Path.GetInvalidFileNameChars(), string.Empty);
        var destiantionFolder = Path.Combine(imageDir, escapedSeriesName);

        var filename = $"{escapedSeriesName} {volumeNumber}{extention}";
        var urlPath = Path.Combine(destiantionFolder, filename);

        try
        {
            using (var client = new HttpClient())
            {
                using (var response = client.GetAsync(url))
                {
                    byte[] imageBytes =
                        response.Result.Content.ReadAsByteArrayAsync().Result;
                    image = imageBytes;

                    var localDirectory = Path.Combine(serverRoot, destiantionFolder);
                    var localPath = Path.Combine(localDirectory, filename);

                    if (!Directory.Exists(localDirectory))
                        Directory.CreateDirectory(localDirectory);

                    System.IO.File.WriteAllBytes(localPath, imageBytes);

                }
            }

            return urlPath;
        }
        catch (Exception)
        {
            throw;
        }
    }

    internal static string SaveOnServer(IFormFile coverFile, string seriesName, int volumeNumber)
    {
        var extention = new FileInfo(coverFile.FileName).Extension;
        var escapedSeriesName = seriesName.Unidecode().Replace(Path.GetInvalidFileNameChars(), string.Empty);
        var destiantionFolder = Path.Combine(imageDir, "Series", escapedSeriesName);
        var filename = $"{escapedSeriesName} {volumeNumber} {coverFile.GetHashCode()}{extention}";

        try
        {
            var localDirectory = Path.Combine(serverRoot, destiantionFolder);
            var localPath = Path.Combine(localDirectory, filename);

            if (!Directory.Exists(localDirectory))
                Directory.CreateDirectory(localDirectory);

            using (var fileStream = new FileStream(localPath, FileMode.Create))
            {
                coverFile.CopyTo(fileStream);
            }

            var urlPath = Path.Combine(destiantionFolder, filename);
            return urlPath;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public static void SaveFlagFromCDN(string countryCode, out string png, out string svg)
    {
        var urls = new List<string> {
            $"https://flagcdn.com/{countryCode}.svg",
            $"https://flagcdn.com/40x30/{countryCode}.png" };

        var destiantionFolder = Path.Combine(imageDir, "countries");
        var localDirectory = Path.Combine(serverRoot, destiantionFolder);


        foreach (var url in urls)
        {
            var extention = Path.GetExtension(url);
            var filename = $"{countryCode}{extention}";

            using (var client = new HttpClient())
            {
                using (var response = client.GetAsync(url))
                {
                    byte[] imageBytes =
                        response.Result.Content.ReadAsByteArrayAsync().Result;

                    var localPath = Path.Combine(localDirectory, filename);

                    if (!Directory.Exists(localDirectory))
                        Directory.CreateDirectory(localDirectory);

                    File.WriteAllBytes(localPath, imageBytes);
                }
            }
        }

        png = Path.Combine(destiantionFolder, $"{countryCode}.png");
        svg = Path.Combine(destiantionFolder, $"{countryCode}.svg");

    }

    public static string FindUrl(string coverUrl)
    {
        var localPath = Path.Combine(serverRoot, coverUrl);
        if (File.Exists(localPath))
        {
            return coverUrl;
        }
        else
        {
            var dir = Path.GetDirectoryName(localPath);
            var patten = $"{Path.GetFileNameWithoutExtension(localPath)} *{Path.GetExtension(localPath)}";
            var files = Directory.GetFiles(dir, patten);

            if (files.Length == 1)
            {
                return Path.Combine(Path.GetDirectoryName(coverUrl), Path.GetFileName(files[0]));
            }

        }
        return coverUrl;
    }

    internal static string? DownloadFileFromWeb(string url, string seriesName, int volumeNumber)
    {
        var extention = new FileInfo(url).Extension;
        var escapedSeriesName = seriesName.Unidecode().Replace(Path.GetInvalidFileNameChars(), string.Empty);
        var destiantionFolder = Path.Combine(imageDir, "Series", escapedSeriesName);
        var filename = $"{escapedSeriesName} {volumeNumber} {url.GetHashCode()}{extention}";

        try
        {
            using (var client = new HttpClient())
            {
                using (var response = client.GetAsync(url))
                {
                    byte[] imageBytes =
                        response.Result.Content.ReadAsByteArrayAsync().Result;

                    var localDirectory = Path.Combine(serverRoot, destiantionFolder);
                    var localPath = Path.Combine(localDirectory, filename);

                    if (!Directory.Exists(localDirectory))
                        Directory.CreateDirectory(localDirectory);

                    System.IO.File.WriteAllBytes(localPath, imageBytes);

                }
            }

            var urlPath = Path.Combine(destiantionFolder, filename);
            return urlPath;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
