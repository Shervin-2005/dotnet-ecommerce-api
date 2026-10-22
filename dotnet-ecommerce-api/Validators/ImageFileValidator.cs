using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace dotnet_ecommerce_api.Validators;

public static class ImageFileValidator
{
    public const long MaxFileSizeBytes = 5 * 1024 * 1024;

    private static readonly HashSet<string> AllowedContentTypes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg",
            "image/png",
            "image/webp"
        };

    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };

    private const int MaxImageDimension = 10_000;
    private const long MaxPixelCount = 40_000_000;

    public static bool IsValid(IFormFile? file)
    {
        if (file is null || file.Length <= 0 || file.Length > MaxFileSizeBytes)
            return false;
        
        if (!AllowedContentTypes.Contains(file.ContentType))
            return false;

        var extension = Path.GetExtension(file.FileName);

        if (!AllowedExtensions.Contains(extension))
            return false;

        try
        {
            using var stream = file.OpenReadStream();
            
            var format = Image.DetectFormat(stream);

            if (format is null || !IsAllowedFormat(format))
                return false;

            // Check that the file's claimed MIME type matches its actual format.
            if (!string.Equals(format.DefaultMimeType, file.ContentType, StringComparison.OrdinalIgnoreCase))
                return false;
            
            stream.Position = 0;

            // Inspect dimensions before fully decoding the image.
            var info = Image.Identify(stream);

            if (info is null ||
                info.Width <= 0 ||
                info.Height <= 0 ||
                info.Width > MaxImageDimension ||
                info.Height > MaxImageDimension ||
                (long)info.Width * info.Height > MaxPixelCount)
                return false;
            
            stream.Position = 0;

            // Fully decode the image to reject corrupted or invalid files.
            using var image = Image.Load(stream);

            return image.Width == info.Width && image.Height == info.Height;
        }
        catch (UnknownImageFormatException)
        {
            return false;
        }
        catch (InvalidImageContentException)
        {
            return false;
        }
        catch (NotSupportedException)
        {
            return false;
        }
        catch (IOException)
        {
            return false;
        }
    }

    private static bool IsAllowedFormat(IImageFormat format)
    {
        return string.Equals(
                   format.Name,
                   "JPEG",
                   StringComparison.OrdinalIgnoreCase)
               || string.Equals(
                   format.Name,
                   "PNG",
                   StringComparison.OrdinalIgnoreCase)
               || string.Equals(
                   format.Name,
                   "WEBP",
                   StringComparison.OrdinalIgnoreCase);
    }
}