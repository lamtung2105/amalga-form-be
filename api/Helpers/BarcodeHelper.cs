using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using ZXing;
using ZXing.Common;
using ZXing.ImageSharp.Rendering;

namespace api.Helpers;

public static class BarcodeHelper
{
    public static string GenerateCode128A(string value)
    {
        var writer = new BarcodeWriter<Image<Rgba32>>
        {
            Format = BarcodeFormat.CODE_128,
            Options = new EncodingOptions
            {
                Width = 400,
                Height = 120,
                Margin = 0,
                PureBarcode = false
            },
            Renderer = new ImageSharpRenderer<Rgba32>()
        };

        using var image = writer.Write(value) ?? throw new Exception("ZXing failed to generate barcode image");
        using var ms = new MemoryStream();
        image.Save(ms, new PngEncoder());

        return Convert.ToBase64String(ms.ToArray());
    }
}
