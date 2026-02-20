using System.Text;
using SkiaSharp;
using Svg.Skia;

namespace Bally;

public static class GlassInsertRenderer
{
    public const float DPI = 600;
    public const string BACKGROUND_COLOR = "#FFFF00";
    public const float STRETCH_FACTOR = 2.35f;
    public static string Generate(string Text, float Width, float Height, string? SvgPath = null, string? PngPath = null)
    {
        var svg = $$"""
                    <svg width="{{Width * DPI}}" height="{{Height * DPI}}" xmlns="http://www.w3.org/2000/svg">
                    <style>
                    .font {
                      font: bold 100px sans-serif;
                      paint-order: stroke;
                      fill: #FF0000;
                      stroke: #000000;
                      stroke-width: 8px;
                      stroke-linecap: butt;
                      stroke-linejoin: miter;
                      font-weight: 800;
                    }
                    .background { fill: {{BACKGROUND_COLOR}} }
                    </style>
                    {{fillRect("background", 0, 0, Width * DPI, Height * DPI)}}
                    {{getText()}}
                    </svg>
                    """;

        if (SvgPath is not null)
        {
            Util.WriteText(SvgPath, svg);
            Console.WriteLine($"Wrote insert SVG image to {SvgPath}.");
        }

        if (PngPath is not null)
        {
            using var skSvg = new SKSvg();
            using var stream = svg.ToStream();
            if (skSvg.Load(stream) is not null && skSvg.Save(PngPath, SKColors.White, SKEncodedImageFormat.Png, 100))
                Console.WriteLine($"Wrote insert PNG image to {PngPath}.");
            else
                Console.WriteLine($"Failed to insert PNG image to {PngPath}.");
        }
        return svg;

        string getText()
        {
            StringBuilder sb = new();
            var lines = Text.Split(Environment.NewLine);
            float stride = Height / (lines.Length + 2) * DPI;
            float start = stride * 1.5f;
            return string.Join(Environment.NewLine,
                Enumerable.Range(0, lines.Length).Select(i => drawText(lines[i], Width / 2 * DPI, start + i * stride)));
        }


        string drawText(string text, float x, float y)
            => $"""<text transform="scale(1, {STRETCH_FACTOR})"  lengthAdjust="spacingAndGlyphs" x="{x}" y="{y / STRETCH_FACTOR}" class="font" text-anchor="middle" dominant-baseline="middle">{text}</text>""";
        string fillRect(string @class, float x0, float y0, float x1, float y1)
            => $"""<rect class="{@class}" x="{x0}" y="{y0}" width="{x1 - x0}" height="{y1 - y0}"/>""";

    }
}
