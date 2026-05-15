using System;
using System.Drawing;
using System.IO;
using Sitecore.Resources.Media;

namespace Foundation.Sitecore.WebP.Upload.Media
{
    public class WebPImageMedia : ImageMedia
    {
        public override global::Sitecore.Resources.Media.Media Clone() => new WebPImageMedia();

        public override Image GetImage()
        {
            var mediaStream = GetStream();
            if (mediaStream == null)
                return null;

            using (var stream = mediaStream.Stream)
            {
                var (width, height) = ReadWebPDimensions(stream);
                if (width <= 0 || height <= 0)
                    return null;

                return new Bitmap(width, height);
            }
        }

        // Parses width/height from the RIFF/WEBP binary header — no external library required.
        // Spec: https://developers.google.com/speed/webp/docs/riff_container
        private static (int width, int height) ReadWebPDimensions(Stream stream)
        {
            try
            {
                var header = new byte[30];
                if (stream.Read(header, 0, header.Length) < 30)
                    return (0, 0);

                // Validate RIFF....WEBP signature
                if (header[0] != 'R' || header[1] != 'I' || header[2] != 'F' || header[3] != 'F')
                    return (0, 0);
                if (header[8] != 'W' || header[9] != 'E' || header[10] != 'B' || header[11] != 'P')
                    return (0, 0);

                var chunkType = System.Text.Encoding.ASCII.GetString(header, 12, 4);

                if (chunkType == "VP8 ")
                {
                    // Lossy: width/height are at bytes 26-29 (14-bit values, minus 1)
                    int w = (header[26] | (header[27] << 8)) & 0x3FFF;
                    int h = (header[28] | (header[29] << 8)) & 0x3FFF;
                    return (w + 1, h + 1);
                }

                if (chunkType == "VP8L")
                {
                    // Lossless: packed bits at bytes 21-24
                    // Byte 20 is 0x2F signature; skip it
                    uint bits = BitConverter.ToUInt32(header, 21);
                    int w = (int)(bits & 0x3FFF) + 1;
                    int h = (int)((bits >> 14) & 0x3FFF) + 1;
                    return (w, h);
                }

                if (chunkType == "VP8X")
                {
                    // Extended: canvas width/height at bytes 24-29 (24-bit LE, minus 1)
                    int w = (header[24] | (header[25] << 8) | (header[26] << 16)) + 1;
                    int h = (header[27] | (header[28] << 8) | (header[29] << 16)) + 1;
                    return (w, h);
                }

                return (0, 0);
            }
            catch
            {
                return (0, 0);
            }
        }
    }
}
