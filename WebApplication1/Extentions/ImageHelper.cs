using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace WebApplication1.Extentions
{
    public static class ImageHelper
    {
        public static string ResizeImg(string inputPath, int? width, int? height)
        {
            using (var image = new Bitmap(System.Drawing.Image.FromFile(inputPath + "\\original.png")))
            {

                Bitmap resized = width.HasValue ? new Bitmap(width.Value, height.Value) : new Bitmap(image.Height, image.Width);

                var filePath = Path.Combine(inputPath + $"\\{resized.Height}.png");

                if (File.Exists(filePath))
                    return filePath;

                using (var graphics = Graphics.FromImage(resized))
                {
                    graphics.CompositingQuality = CompositingQuality.HighSpeed;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.DrawImage(image, 0, 0, resized.Width, resized.Height);

                    using (var output = File.Open(filePath, FileMode.Create))
                    {
                        var qualityParamId = System.Drawing.Imaging.Encoder.Quality;

                        var encoderParameters = new EncoderParameters(1);
                        encoderParameters.Param[0] = new EncoderParameter(qualityParamId, 100);

                        var codec = ImageCodecInfo.GetImageDecoders()
                            .FirstOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);

                        resized.Save(output, ImageFormat.Png);//, codec, encoderParameters);

                        return filePath;
                    }
                }
            }
        }
    }
}
