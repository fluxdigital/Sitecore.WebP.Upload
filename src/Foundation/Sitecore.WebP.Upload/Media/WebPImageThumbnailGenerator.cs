using System.IO;
using SkiaSharp;
using Sitecore.Resources.Media;

namespace Foundation.Sitecore.WebP.Upload.Media
{
    public class WebPImageThumbnailGenerator : MediaThumbnailGenerator
    {
        public override MediaStream GetStream(MediaData mediaData, TransformationOptions options)
        {
            var input = mediaData.GetStream();
            if (input == null)
                return null;

            using (var inputStream = input.Stream)
            using (var skData = SKData.Create(inputStream))
            using (var original = SKBitmap.Decode(skData))
            {
                if (original == null)
                    return null;

                int maxWidth = options.MaxSize.Width > 0 ? options.MaxSize.Width : 150;
                int maxHeight = options.MaxSize.Height > 0 ? options.MaxSize.Height : 150;

                var (thumbWidth, thumbHeight) = FitDimensions(original.Width, original.Height, maxWidth, maxHeight);

                using (var resized = original.Resize(new SKImageInfo(thumbWidth, thumbHeight), SKFilterQuality.Medium))
                using (var image = SKImage.FromBitmap(resized))
                using (var encoded = image.Encode(SKEncodedImageFormat.Png, 90))
                {
                    var output = new MemoryStream();
                    encoded.SaveTo(output);
                    output.Position = 0;
                    return new MediaStream(output, "png", null);
                }
            }
        }

        private static (int w, int h) FitDimensions(int srcW, int srcH, int maxW, int maxH)
        {
            if (srcW <= maxW && srcH <= maxH)
                return (srcW, srcH);

            double ratio = System.Math.Min((double)maxW / srcW, (double)maxH / srcH);
            return ((int)(srcW * ratio), (int)(srcH * ratio));
        }
    }
}
