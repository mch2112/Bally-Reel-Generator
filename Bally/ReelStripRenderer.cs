using SkiaSharp;
using Svg.Skia;
using System.Text;
using System.Text.RegularExpressions;

namespace Bally;

public record struct ReelStripMetrics(double DPI,
                                      double CanvasHeightIn,
                                      double MarginXIn,
                                      double StripHeightIn,
                                      double StripWidthIn,
                                      double SymbolSizeIn,
                                      double SymbolSizeUnits,
                                      double YTrailingHeightIn,
                                      double FontHeightIn,
                                      double TextYStrideIn,
                                      double GuideThicknessPix)
{
    public readonly double CanvasHeightPix => CanvasHeightIn * DPI;
    public readonly double MarginXPix => MarginXIn * DPI;
    public readonly double MarginYIn => (CanvasHeightIn - StripHeightIn) / 2;
    public readonly double MarginYPix => MarginYIn * DPI;
    public readonly double StripHeightPix => StripHeightIn * DPI;
    public readonly double StripWidthPix => StripWidthIn * DPI;
    public readonly double StripHeightCoreIn => StripHeightIn - YTrailingHeightIn;
    public readonly double StripHeightCorePix => StripHeightCoreIn * DPI;
    public readonly double SymbolScale => SymbolSizeIn / SymbolSizeUnits;
    public readonly double SymbolSizePix => SymbolSizeIn * DPI;
    public readonly double SymbolOriginXPix => (StripWidthPix - SymbolSizePix) / 2;
    public readonly double YTrailingHeightPix => YTrailingHeightIn * DPI;
    public readonly double FontHeightPix => FontHeightIn * DPI;
    public readonly double TextStartYPix => MarginYPix + StripHeightPix - (0.6 * YTrailingHeightPix);
    public readonly double TextYStridePix => TextYStrideIn * DPI;
}

public enum ReelStripStyle { Standard, Wide }

public partial class ReelStripRenderer
{
    ReelStripMetrics Metrics { get; }
    public ReelStripRenderer(ReelStripStyle Style =  ReelStripStyle.Standard,
                             double CanvasHeightIn = 28.16,
                             double MarginXIn = 0.08,
                             double Dpi = 600,
                             double GuideThicknessPix = 3)
    {
        Metrics = Style switch
        {
            ReelStripStyle.Standard => new(Dpi,
                                           CanvasHeightIn,
                                           MarginXIn,
                                           27.75,
                                           2.0 + 11.0 / 16.0,
                                           0.05 + 1.125 + 1.0 / 32.0,
                                           1000,
                                           1.5,
                                           0.13,
                                           0.225,
                                           GuideThicknessPix),
            ReelStripStyle.Wide     => new(Dpi,
                                           CanvasHeightIn,
                                           MarginXIn,
                                           27.75,
                                           2.0 + 11.0 / 16.0,
                                           0.05 + 1.125 + 1.0 / 32.0,
                                           1000,
                                           1.5,
                                           0.13,
                                           0.225,
                                           GuideThicknessPix),
            _ => throw new NotImplementedException()
        };
    }
    
    public string GenerateReelStrips(IMachine Machine, int NumCopies, string SvgPath, string PngPath) =>
        GenerateReelStrips(Machine.Reels.Select(r => r.Symbols), Machine.Reels.Select(r => r.ReelStripInfo).ToArray(), NumCopies, SvgPath, PngPath);
    public string GenerateReelStrips(string[] Symbols, string[][] ReelStripInfo, int NumCopies, string SvgPath, string PngPath) =>
        GenerateReelStrips(Symbols.Select(SymbolSet.SplitStringToSymbols), ReelStripInfo, NumCopies, SvgPath, PngPath);
    public string GenerateReelStrips(IEnumerable<IEnumerable<Symbol>> Symbols, string[][]? ReelStripInfo, int NumCopies, string SvgPath, string PngPath)
    {
        var symbols = Symbols.Select(s => s.ToArray()).ToArray();
        int numReels = symbols.Length;

        double canvasWidthPix = Metrics.StripWidthPix * NumCopies * numReels + Metrics.MarginXPix * 2 + Metrics.GuideThicknessPix;

        StringBuilder sb = new(GetSymbols());
        sb.AppendLine();

        // Paint the background color
        fillRect(sb, "background", 0, 0, canvasWidthPix, Metrics.CanvasHeightPix);

        // Draw cutting guides
        drawHGuide(sb, Metrics.MarginXPix, Metrics.MarginYPix, Metrics.StripWidthPix * numReels * NumCopies);
        drawHGuide(sb, Metrics.MarginXPix, Metrics.MarginYPix + Metrics.StripHeightPix - Metrics.GuideThicknessPix, Metrics.StripWidthPix * numReels * NumCopies);
        for (double x = 0; x <= canvasWidthPix; x += Metrics.StripWidthPix)
            drawVGuide(sb, Metrics.MarginXPix + x - Metrics.GuideThicknessPix / 2, Metrics.MarginYPix, Metrics.StripHeightPix);

        for (int copyNum = 0; copyNum < NumCopies; copyNum++)
            for (int reelNum = 0; reelNum < numReels; reelNum++)
            {
                var numStops = symbols[reelNum].Length;
                double yStridePix = Metrics.StripHeightCorePix / numStops;
                
                // Symbols
                for (int stopNum = 0; stopNum < numStops; stopNum++)
                {
                    string id = symbols[reelNum][stopNum] switch
                    {
                        Symbol.Lemon => SYMBOL_NAME_LEMON,
                        Symbol.Cherry => SYMBOL_NAME_CHERRY,
                        Symbol.Orange => SYMBOL_NAME_ORANGE,
                        Symbol.Plum => SYMBOL_NAME_PLUM,
                        Symbol.Bell => SYMBOL_NAME_BELL,
                        Symbol.Melon => SYMBOL_NAME_MELON,
                        Symbol.Bar => SYMBOL_NAME_BAR,
                        Symbol.SingleBar => SYMBOL_NAME_ONE_BAR,
                        Symbol.DoubleBar => SYMBOL_NAME_TWO_BARS,
                        Symbol.TripleBar => SYMBOL_NAME_THREE_BARS,
                        Symbol.Seven => SYMBOL_NAME_SEVEN,
                        _ => throw new NotImplementedException()
                    };

                    var x = Metrics.MarginXPix + Metrics.SymbolOriginXPix + reelNum * Metrics.StripWidthPix + copyNum * Metrics.StripWidthPix * numReels;
                    var y = Metrics.MarginYPix + stopNum * yStridePix;

                    string transformSlug = $"""transform="translate({x}, {y}) scale({Metrics.DPI * Metrics.SymbolScale})" """;

                    drawSymbol(sb, id, 0, 0, transformSlug);
                }
                if (ReelStripInfo is not null)
                {
                    // Text at strip bottom
                    var x = Metrics.MarginXPix + Metrics.StripWidthPix * (0.5 + reelNum + numReels * copyNum);
                    for (int infoNum = 0; infoNum < ReelStripInfo[reelNum].Length; infoNum++)
                    {
                        var y = Metrics.TextStartYPix + infoNum * Metrics.TextYStridePix;
                        drawText(sb, x, y, ReelStripInfo[reelNum][infoNum]);
                    }
                }
            }

        string svg;
        svg = GetSvg(canvasWidthPix, sb);
        if (SvgPath is not null)
        {
            Util.WriteText(SvgPath, svg);
            Console.WriteLine($"Wrote reel strip SVG image to {SvgPath}.");
        }

        if (PngPath is not null)
            SavePng(PngPath, svg, out SKSvg skSvg);

        return svg;

        void drawHGuide(StringBuilder sb, double x, double y, double length, string transformSlug = "")
            => sb.AppendLine($"""<rect class="guide" x="{x}" y="{y}" width="{length}" height="{Metrics.GuideThicknessPix}" {transformSlug}/>""");
        void drawVGuide(StringBuilder sb, double x, double y, double length, string transformSlug = "")
            => sb.AppendLine($"""<rect class="guide" x="{x}" y="{y}" width="{Metrics.GuideThicknessPix}" height="{length}" {transformSlug}/>""");
        void fillRect(StringBuilder sb, string @class, double x0, double y0, double x1, double y1, string transformSlug = "")
           => sb.AppendLine($"""<rect class="{@class}" x="{x0}" y="{y0}" width="{x1 - x0}" height="{y1 - y0}" {transformSlug}/>""");
        void drawSymbol(StringBuilder sb, string symbolId, double x, double y, string transformSlug = "")
            => sb.AppendLine($"""<use href="#{symbolId}" x="{x}" y="{y}" width="1000" height="1000" {transformSlug}/>""");
        void drawText(StringBuilder sb, double x, double y, string text, string transformSlug = "")
            => sb.AppendLine($"""<text x="{x}" y="{y}" class="reelStripFont" text-anchor="middle" {transformSlug}>{text}</text>""");
    }


    private static void SavePng(string PngPath, string svg, out SKSvg skSvg)
    {
        skSvg = new SKSvg();
        Stream stream = svg.ToStream();
        if (skSvg.Load(stream) is not null &&
            skSvg.Save(PngPath, SKColors.White, SKEncodedImageFormat.Png, 100, (float)1, (float)1))
            Console.WriteLine($"Wrote reel strip PNG image to {PngPath}.");
        else
            Console.WriteLine($"Failed to save reel strip PNG image to {PngPath}.");
    }

    private static string GetSymbols()
    {
        return $"""
{LEMON}
{CHERRY}
{ORANGE}
{PLUM}
{BELL}
{MELON}
{BAR}
{ONE_BAR}
{TWO_BARS}
{THREE_BARS}
{SEVEN}
""";
    }
    private static string Transform(string Input, double X, double Y)
    {
    foreach (Match m in r.Matches(Input).OrderByDescending(m => m.Index))
        {
            var g = m.Groups[1];
            Input = Input[..g.Index] + TransformXY(Input[g.Index..(g.Index + g.Length)], X, Y) + Input[(g.Index + g.Length)..];
        }
        return Input;
    }

    private static string TransformXY(string input, double X, double Y)
    {
        return string.Join(' ',
                           input.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                                .SelectMany(xff));
        IEnumerable<string> xff(string? input)
        {
            if (input is null)
                yield break;
            else if (input.Length == 1 && (input[0] >= 'A' && input[0] <= 'Z') || (input[0] >= 'a' && input[0] <= 'z'))
                yield return input;
            else if (input.Contains(','))
            {
                var s = input.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (s.Length == 2 && double.TryParse(s[0], out var x) && double.TryParse(s[1], out var y))
                {
                    yield return $"{X + x},{Y + y}";
                }
                else
                    throw new Exception();
            }
            else
                throw new ArgumentException("Invalid Transformation");
        }
    }
    private static readonly Regex r = MyRegex();

    [GeneratedRegex("[^\\w]d=\"(.*)\"")]
    private static partial Regex MyRegex();
}
