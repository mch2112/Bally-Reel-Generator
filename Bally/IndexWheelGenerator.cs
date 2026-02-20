using System.Text;

namespace Bally;

public static class IndexWheelGenerator
{
    public static void Generate(int[] Codes, IndexWheelParams Params, string SvgWritePath)
    {
        if (Params.NumStops != Codes.Length)
            throw new Exception("Code list doesn't match number of stops in params.");

        double scale = 1;

        // The bounds of the canvas are defined by the tips of the spokes in the index disk
        Offset min = new Offset(Enumerable.Range(0, Codes.Length).Min(i => Math.Cos((i + 0.5) * Math.PI * 2 / Codes.Length) * Params.OuterRadius),
                                Enumerable.Range(0, Codes.Length).Min(i => Math.Sin((i + 0.5) * Math.PI * 2 / Codes.Length) * Params.OuterRadius))
                            * scale;


        Offset max = new Offset(Enumerable.Range(0, Codes.Length).Max(i => Math.Cos((i + 0.5) * Math.PI * 2 / Codes.Length) * Params.OuterRadius),
                                Enumerable.Range(0, Codes.Length).Max(i => Math.Sin((i + 0.5) * Math.PI * 2 / Codes.Length) * Params.OuterRadius))
                            * scale;

        var path = GetPath(Codes, Params)
                    .WithItems(new Hole(Point.Empty, Params.CenterHoleRadius, false),
                               new Hole(new(0, Params.FastenerHolePositionRadius), Params.FastenerHoleRadius, false),
                               new Hole(new(0, -Params.FastenerHolePositionRadius), Params.FastenerHoleRadius, false),
                               new Hole(new(Params.FastenerHolePositionRadius, 0), Params.FastenerHoleRadius, false),
                               new Hole(new(-Params.FastenerHolePositionRadius, 0), Params.FastenerHoleRadius, false),
                               new Hole(new(Math.Cos(Math.PI / 4) * Params.IndexHolePositionRadius, -Math.Sin(Math.PI / 4) * Params.IndexHolePositionRadius), Params.IndexHoleRadius, false))
                   .Scale(scale)
                   //  .HReflect()
                   .Translate(-min);

        var width = max.dX - min.dX;
        var height = max.dY - min.dY;

        StringBuilder sb = new();
        sb.AppendLine($"""
                       <svg width="{width}in"
                            height="{height}in"
                            viewBox="0 0 {width} {height}"
                            xmlns="http://www.w3.org/2000/svg"
                            xmlns:inkscape="http://www.inkscape.org/namespaces/inkscape"
                            xmlns:sodipodi="http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd"
                            xmlns:svg="http://www.w3.org/2000/svg">
                       <sodipodi:namedview
                            id="main"
                            inkscape:document-units="in"/>
                       
                       """);
        sb.AppendLine(path.Render("#b0b0b0"));

        sb.AppendLine("</svg>");

        Util.WriteText(SvgWritePath, sb.ToString());
        Console.WriteLine($"Wrote index wheel SVG image to {SvgWritePath}.");

    }

    private static SvgPath GetPath(int[] Codes, IndexWheelParams Params)
    {
        List<IPathItem> items = [];

        var angleIncrement = Math.PI * 2 / Codes.Length;
        var angleIncrementHalf = angleIncrement / 2;

        Point leadingPoint = SvgPath.Rotate(new Point(Params.OuterRadius, 0), Point.Empty, -angleIncrementHalf);
        items.Add(new Move(leadingPoint));

        Point slotArcOrigin = new(Params.SlotArcOriginX, Params.SlotArcOriginY);

        var slotStart = new Point(Params.MinSlotDepth, Params.StartSlotDeviation);

        // relative to the arc origin

        var minAngle = Math.Atan2(Params.StartSlotDeviation - slotArcOrigin.Y,
                                  Params.MinSlotDepth - slotArcOrigin.X);
        var maxAngle = Math.Atan2(Params.EndSlotDeviation - slotArcOrigin.Y,
                                  Params.MaxSlotDepth - slotArcOrigin.X);

        // Angle increment per code # change
        var dTheta = (maxAngle - minAngle) / (Params.NumCodes - 1);

        var slotRadius2d = new Offset(Params.SlotRadius, Params.SlotRadius);
        var arcRadius2d = new Offset(Params.SlotArcRadius, Params.SlotArcRadius);

        (Point start, Point end) = getSlotOpeningPoints(slotStart,
                                                        slotArcOrigin,
                                                        Params.SlotRadius);

        for (int i = 0; i < Codes.Length; i++)
        {
            SvgPath path = new();
            
            // P0
            if (i > 0)
                path = path.WithItem(new Line(leadingPoint));

            // P1
            path = path.WithItem(new Line(start));

            // P2
            var depth = Codes[i]; /* Codes are zero based */

            double angle = minAngle + depth * dTheta;
            Point p = SvgPath.ToCartesian(new PolarCoordinates(Params.SlotArcRadius - Params.SlotRadius, angle), slotArcOrigin);
            path = path.WithItem(new Arc(p, arcRadius2d, 0, true));

            // P3
            p = SvgPath.ToCartesian(new PolarCoordinates(Params.SlotArcRadius, angle), slotArcOrigin);
            var apex = SvgPath.ToCartesian(new PolarCoordinates(Params.SlotRadius, angle + Math.PI / 2), p);
            path = path.WithItem(new Arc(apex, slotRadius2d));
            
            // P4
            p = SvgPath.ToCartesian(new PolarCoordinates(Params.SlotArcRadius + Params.SlotRadius, angle), slotArcOrigin);
            path = path.WithItem(new Arc(p, slotRadius2d));

            // P5
            path = path.WithItem(new Arc(end, arcRadius2d, 0, false));

            items.AddRange(path.Rotate(i * angleIncrement, Point.Empty).Items);
        }
        items.Add(new Close());
        return new SvgPath(items);
    }
    static (Point start, Point end) getSlotOpeningPoints(Point slotStart, Point origin, double slotRadius)
    {
        // See https://mathworld.wolfram.com/Circle-LineIntersection.html for formulas

        var x1 = 0;
        var y1 = 0;
        var x2 = origin.X - slotStart.X;
        var y2 = origin.Y + slotStart.Y;

        var r = slotRadius;
        var dx = x2 - x1;
        var dy = y2 - y1;
        var dr = Math.Sqrt(dx * dx + dy * dy);
        var D = x1 * y2 - x2 * y1;

        Offset start = new((D * dy - Math.Sign(dy) * dx * Math.Sqrt(r * r * dr * dr - D * D)) / (dr * dr),
                           (-D * dx - Math.Abs(dy) * Math.Sqrt(r * r * dr * dr - D * D)) / (dr * dr));
        Offset end = new((D * dy + Math.Sign(dy) * dx * Math.Sqrt(r * r * dr * dr - D * D)) / (dr * dr),
                         (-D * dx + Math.Abs(dy) * Math.Sqrt(r * r * dr * dr - D * D)) / (dr * dr));

        return (slotStart + start, slotStart + end);
    }
}
interface IPathItem
{
    string Key { get; }
    string Render();
    IPathItem Translate(Offset O);
    IPathItem Scale(double Scale);
    IPathItem Rotate(double Theta, Point About);
    IPathItem HReflect();
}
record struct Move(Point P) : IPathItem
{
    public readonly string Key => "M";
    public readonly string Render() => $"{Key} {P.X} {P.Y}";
    public readonly Move Translate(Offset O) => new(P + O);
    public readonly Move Scale(double Scale) => new(P * Scale);
    public readonly Move Rotate(double dTheta, Point About) => new(SvgPath.Rotate(P, About, dTheta));
    public readonly Move HReflect() => new(new(P.X, -P.Y));
    readonly IPathItem IPathItem.Translate(Offset O) => Translate(O);
    readonly IPathItem IPathItem.Scale(double Scale) => this.Scale(Scale);
    readonly IPathItem IPathItem.Rotate(double ThetaDelta, Point About) => this.Rotate(ThetaDelta, About);
    readonly IPathItem IPathItem.HReflect() => this.HReflect();
}
record struct Line(Point P) : IPathItem
{
    public readonly string Key => "L";
    public readonly string Render() => $"{Key} {P.X} {P.Y}";
    public readonly Line Scale(double Scale) => new(P * Scale);
    public readonly Line Translate(Offset O) => new(P + O);
    public readonly Line Rotate(double dTheta, Point About)
    {
        var p = SvgPath.Rotate(P, About, dTheta);
        return new Line(p);
    }
    public readonly Line HReflect() => new(new(P.X, -P.Y));

    readonly IPathItem IPathItem.Translate(Offset O) => Translate(O);
    readonly IPathItem IPathItem.Scale(double Scale) => this.Scale(Scale);
    readonly IPathItem IPathItem.Rotate(double ThetaDelta, Point About) => this.Rotate(ThetaDelta, About);
    readonly IPathItem IPathItem.HReflect() => this.HReflect();
}
record struct Arc(Point P, Offset Radius, double Rotation = 0, bool Clockwise = false, bool LargeArc = false) : IPathItem
{
    public readonly string Key => "A";
    public readonly string Render() => $"{Key} {Radius.dX} {Radius.dY} {Rotation} {(LargeArc ? 1 : 0)} {(Clockwise ? 1 : 0)} {P.X} {P.Y} ";
    public readonly Arc Translate(Offset O) => new(P + O, Radius, Rotation, Clockwise, LargeArc);
    public readonly Arc Scale(double Scale) => new(P * Scale, Radius * Scale, Rotation, Clockwise, LargeArc);
    public readonly Arc Rotate(double dTheta, Point About)
    {
        var p = SvgPath.Rotate(P, About, dTheta);
        return new Arc(p, Radius, Rotation * 180 / Math.PI, Clockwise, LargeArc);
    }
    public readonly Arc HReflect() => new(new(P.X, -P.Y), Radius, Rotation, !Clockwise, LargeArc);
    readonly IPathItem IPathItem.Translate(Offset O) => Translate(O);
    readonly IPathItem IPathItem.Scale(double Scale) => this.Scale(Scale);
    readonly IPathItem IPathItem.Rotate(double ThetaDelta, Point About) => this.Rotate(ThetaDelta, About);
    readonly IPathItem IPathItem.HReflect() => this.HReflect();

}
record struct Hole(Point P, double Radius, bool Clockwise) : IPathItem
{
    public readonly string Key => "?";
    public readonly string Render()
    {
        var offset = Clockwise ? -Radius : Radius;
        var radius2d = new Offset(Radius, Radius);
        StringBuilder sb = new();
        sb.Append(new Move(P.OffsetX(- offset)).Render() + " ");
        sb.Append(new Arc(P.OffsetY(offset), radius2d, Clockwise: Clockwise).Render() + " ");
        sb.Append(new Arc(P.OffsetX(offset), radius2d, Clockwise: Clockwise).Render() + " ");
        sb.Append(new Arc(P.OffsetY(-offset), radius2d, Clockwise: Clockwise).Render() + " ");
        sb.Append(new Arc(P.OffsetX(-offset), radius2d, Clockwise: Clockwise).Render() + " ");
        sb.Append(new Close().Render());
        return sb.ToString();
    }
    public readonly Hole Scale(double Scale) => new(P * Scale, Radius * Scale, Clockwise);
    public readonly Hole Translate(Offset O) => new(P + O, Radius, Clockwise);
    public readonly Hole Rotate(double dTheta, Point About)
    {
        var p = SvgPath.Rotate(P, About, dTheta);
        return new Hole(p, Radius, Clockwise);
    }
    public readonly Hole HReflect() => new(new(P.X, -P.Y), Radius, !Clockwise);
    readonly IPathItem IPathItem.Scale(double Scale) => this.Scale(Scale);
    readonly IPathItem IPathItem.Translate(Offset O) => Translate(O);
    readonly IPathItem IPathItem.Rotate(double dTheta, Point About) => this.Rotate(dTheta, About);
    readonly IPathItem IPathItem.HReflect() => this.HReflect();
}
record struct Close() : IPathItem
{
    public readonly string Key => "Z";
    public readonly string Render() => Key;
    public readonly Close Translate(Offset _) => this;
    public readonly Close Scale(double _) => this;
    public readonly Close Rotate(double _, Point __) => this;
    public readonly Close HReflect() => this;
    readonly IPathItem IPathItem.Scale(double Scale) => this.Scale(Scale);
    readonly IPathItem IPathItem.Translate(Offset O) => this;
    readonly IPathItem IPathItem.Rotate(double dTheta, Point About) => this.Rotate(dTheta, About);
    readonly IPathItem IPathItem.HReflect() => this.HReflect();
} 
class SvgPath
{
    public SvgPath() { }
    public SvgPath(IEnumerable<IPathItem> Items) => this.items = [.. Items];
    private readonly List<IPathItem> items = [];
    public IEnumerable<IPathItem> Items => items;
    public string Render(string? Fill)
    {
        StringBuilder sb = new();
        sb.Append("<path ");
        if (Fill is not null)
            sb.Append($""" fill="{Fill}" """);
        sb.Append(" d=\"");
        sb.Append(string.Join(' ', items.Select(i => i.Render())));
        sb.Append("\"/>");
        return sb.ToString();
    }
    public SvgPath HReflect() => new(items.Select(i => i.HReflect()));
    public SvgPath Scale(double Scale) => new(items.Select(i => i.Scale(Scale)));
    public  SvgPath Rotate(double Theta, Point About) => new(items.Select(i => i.Rotate(Theta, About)));
    public SvgPath Translate(Offset O) => new(items.Select(i => i.Translate(O)));
    public SvgPath WithItem(IPathItem Item) => new(items.Append(Item));
    public SvgPath WithItems(params IPathItem[] Items) => new(items.Concat(Items));
    internal static Point Rotate(Point P, Point About, double dTheta)
    {
        var pc = ToVector(P, About);
        return ToCartesian(new PolarCoordinates(pc.Radius, pc.Theta + dTheta), About);
    }
    internal static PolarCoordinates ToVector(Point P, Point About)
    {
        var r = P.DistanceTo(About);
        var theta = Math.Atan2(P.Y - About.Y, P.X - About.X);
        return new(r, theta);
    }
    internal static Point ToCartesian(PolarCoordinates Pc, Point About) => new(About.X + Pc.Radius * Math.Cos(Pc.Theta),
                                                                               About.Y + Pc.Radius * Math.Sin(Pc.Theta));
}

internal record struct Point(double X, double Y)
{
    public static Point Empty { get; } = new(0, 0);
    public static Point operator +(Point P, Offset O) => new(P.X + O.dX, P.Y + O.dY);
    public static Point operator -(Point P, Offset O) => new(P.X - O.dX, P.Y - O.dY);
    public static Offset operator -(Point P1, Point P2) => new(P1.X - P2.X, P1.Y - P2.Y);
    public static Point operator *(Point P, double Factor) => new(P.X * Factor, P.Y * Factor);
    public static Point operator -(Point P) => new(-P.X, -P.Y);
    public readonly Point OffsetX(double Offset) => new(X + Offset, Y);
    public readonly Point OffsetY(double Offset) => new(X, Y + Offset);
    public readonly double DistanceTo(Point Other) => Math.Sqrt((Other.X - X) * (Other.X - X) + (Other.Y - Y) * (Other.Y - Y));
}
internal record struct Offset(double dX, double dY)
{
    public static Offset operator *(Offset P, double Factor) => new(P.dX * Factor, P.dY * Factor);
    public static Offset operator -(Offset O) => new(-O.dX, -O.dY);
}
internal record struct PolarCoordinates(double Radius, double Theta);
