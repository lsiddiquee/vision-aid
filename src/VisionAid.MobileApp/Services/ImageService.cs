using Microsoft.Maui.ApplicationModel;
using SkiaSharp;

namespace VisionAid.MobileApp.Services;

internal class ImageService
{
    private Queue<Stream> _imageQueue = new Queue<Stream>();
    private object _lock = new object();

    //public Stream ResizeImage(Stream imageStream, int maxWidth = Configuration.MaxWidth)
    //{
    //    using SKBitmap image = SKBitmap.Decode(imageStream);

    //    using var newImage = Resize(image, maxWidth);

    //    var originalImageStream = new MemoryStream();
    //    newImage.Encode(SKEncodedImageFormat.Png, 40).SaveTo(originalImageStream);
    //    return originalImageStream;
    //}

    //public Stream DrawZones(Stream imageStream)
    //{
    //    using var image = SKBitmap.Decode(imageStream);
    //    // Create a new bitmap to draw on
    //    using var newImage = DrawZones(image);

    //    var newImageStream = new MemoryStream();
    //    newImage.Encode(SKEncodedImageFormat.Png, 40).SaveTo(newImageStream);

    //    return newImageStream;
    //}

    public Stream ResizeAndDrawZones(Stream imageStream, int maxWidth = Configuration.MaxWidth)
    {
        using var image = SKBitmap.Decode(imageStream);

        var rotated = new SKBitmap(image.Height, image.Width);

        using (var surface = new SKCanvas(rotated))
        {
            surface.Translate(rotated.Width, 0);
            surface.RotateDegrees(90);
            surface.DrawBitmap(image, 0, 0);
        }

        using var resizedImage = Resize(rotated, maxWidth);

        using var withZones = DrawZones(resizedImage);

        var newImageStream = new MemoryStream();
        resizedImage.Encode(SKEncodedImageFormat.Png, 40).SaveTo(newImageStream);
        return newImageStream;
    }

    private SKBitmap DrawZones(SKBitmap image)
    {
        var canvas = new SKCanvas(image);

        // Define paint for drawing lines and text
        var linePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Red,
            StrokeWidth = 3
        };

        var textPaint = new SKPaint
        {
            Color = SKColors.White,
            TextSize = 10,
            IsAntialias = true
        };

        var rectanglePaint = new SKPaint
        {
            Color = SKColors.Gray,
            Style = SKPaintStyle.Fill,
        };

        // Get the thirds of the image width
        float thirdWidth = image.Width / 3;
        float halfHeight = image.Height / 2;

        // Draw vertical lines to create columns
        canvas.DrawLine(thirdWidth, 0, thirdWidth, image.Height, linePaint);
        canvas.DrawLine(2 * thirdWidth, 0, 2 * thirdWidth, image.Height, linePaint);

        // Draw horizontal line to create rows
        canvas.DrawLine(0, halfHeight, image.Width, halfHeight, linePaint);

        // Draw labels for each section
        float y = halfHeight - 20; // halfHeight / 2
        float yForBox = y - 12;
        canvas.DrawRect(thirdWidth / 2 - 60, yForBox, 40, 15, rectanglePaint);
        canvas.DrawRect(thirdWidth + thirdWidth / 2 - 60, yForBox, 50, 15, rectanglePaint);
        canvas.DrawRect(2 * thirdWidth + thirdWidth / 2 - 60, yForBox, 45, 15, rectanglePaint);

        canvas.DrawText("Top Left", thirdWidth / 2 - 60, y, textPaint);
        canvas.DrawText("Top Middle", thirdWidth + thirdWidth / 2 - 60, y, textPaint);
        canvas.DrawText("Top Right", 2 * thirdWidth + thirdWidth / 2 - 60, y, textPaint);

        y = image.Height - 20; // halfHeight + halfHeight / 2
        yForBox = y - 12;
        canvas.DrawRect(thirdWidth / 2 - 60, yForBox, 55, 15, rectanglePaint);
        canvas.DrawRect(thirdWidth + thirdWidth / 2 - 60, yForBox, 70, 15, rectanglePaint);
        canvas.DrawRect(2 * thirdWidth + thirdWidth / 2 - 60, yForBox, 60, 15, rectanglePaint);

        canvas.DrawText("Bottom Left", thirdWidth / 2 - 60, y, textPaint);
        canvas.DrawText("Bottom Middle", thirdWidth + thirdWidth / 2 - 60, y, textPaint);
        canvas.DrawText("Bottom Right", 2 * thirdWidth + thirdWidth / 2 - 60, y, textPaint);

        return image;
    }

    private static SKBitmap Resize(SKBitmap image, int maxWidth)
    {
        if (image.Width > maxWidth)
        {
            float newHeight = ((float)image.Height / image.Width) * maxWidth;
            return image.Resize(new SKImageInfo(maxWidth, (int)newHeight), SKFilterQuality.Medium);
        }

        return image;
    }
}
