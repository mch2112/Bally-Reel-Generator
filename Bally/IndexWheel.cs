using System.Text.Json.Serialization;

namespace Bally;
public class IndexWheel(string Name, params int[] Stops)
{
    // IMPORTANT: These codes are zero-based. Subtract 1 from the values in the bally technical sheets for the Index Wheel
    public static IndexWheel P168  { get; } = new(nameof(P168), 0, 8, 1, 7, 2, 6, 4, 5, 3, 7, 2, 6, 4, 5, 3, 7, 2, 6, 4, 5, 3, 9);
    public static IndexWheel P169  { get; } = new(nameof(P169), 9, 0, 8, 2, 7, 3, 6, 4, 5, 3, 6, 4, 5, 3, 6, 4, 5, 3, 6, 4, 5, 1);
    public static IndexWheel P1233 { get; } = new(nameof(P1233), 1, 4, 1, 0, 1, 4, 1, 4, 1, 6, 1, 2, 4, 0, 1, 4, 1, 5, 1, 3);
    public static IndexWheel P1234 { get; } = new(nameof(P1234), 2, 6, 2, 0, 2, 0, 2, 5, 2, 1, 2, 0, 5, 0, 2, 4, 2, 3, 2, 0);
    public static IndexWheel P1235 { get; } = new(nameof(P1235), 3, 6, 3, 1, 3, 1, 3, 5, 3, 1, 3, 2, 3, 1, 3, 4, 3, 1, 3, 1);
    public static IndexWheel P1236 { get; } = new(nameof(P1236), 2, 5, 2, 0, 2, 0, 2, 6, 2, 1, 2, 0, 2, 0, 2, 4, 2, 3, 2, 0);

    public static IndexWheel GetIndexDisc(IndexWheelNumber Number) => Number switch
    {
        IndexWheelNumber.P168 => P168,
        IndexWheelNumber.P169 => P169,
        IndexWheelNumber.P1233 => P1233,
        IndexWheelNumber.P1234 => P1234,
        IndexWheelNumber.P1235 => P1235,
        IndexWheelNumber.P1236 => P1236,
        _ => throw new NotImplementedException()
    };

    public string Name { get; } = Name;
    public int[] Stops { get; } = Stops;
    [JsonIgnore]
    public int NumStops { get; } = Stops.Length;
    [JsonIgnore]
    public int NumCodes { get; } = Stops.Distinct().Count();
}
