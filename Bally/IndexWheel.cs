using System.Text.Json.Serialization;

namespace Bally;

public class IndexWheel(string Name, params int[] Stops)
{
    public static IndexWheel[] BuiltInWheels =
        [
            new("P168",  0, 8, 1, 7, 2, 6, 4, 5, 3, 7, 2, 6, 4, 5, 3, 7, 2, 6, 4, 5, 3, 9),
            new("P169",  9, 0, 8, 2, 7, 3, 6, 4, 5, 3, 6, 4, 5, 3, 6, 4, 5, 3, 6, 4, 5, 1),
            new("P305",  5, 3, 2, 3, 0, 3, 1, 3, 4, 3, 2, 3, 0, 2, 3, 2, 3, 4, 1, 3, 2, 3),
            new("P306",  5, 1, 4, 0, 4, 3, 4, 0, 4, 1, 4, 0, 4, 0, 4, 1, 4, 3, 4, 2, 4, 0),
            new("P307",  5, 1, 2, 1, 2, 0, 2, 1, 2, 1, 2, 4, 2, 1, 2, 3, 2, 0, 2, 1, 2, 1),
            new("P347",  5, 1, 4, 0, 4, 3, 4, 0, 4, 1, 4, 0, 3, 0, 4, 1, 4, 3, 4, 2, 4, 0),
            new("P1233", 1, 4, 1, 0, 1, 4, 1, 4, 1, 6, 1, 2, 4, 0, 1, 4, 1, 5, 1, 3),
            new("P1234", 2, 6, 2, 0, 2, 0, 2, 5, 2, 1, 2, 0, 5, 0, 2, 4, 2, 3, 2, 0),
            new("P1235", 2, 5, 2, 0, 2, 0, 2, 4, 2, 0, 2, 1, 2, 0, 2, 3, 2, 0, 2, 0),
            new("P1236", 2, 5, 2, 0, 2, 0, 2, 6, 2, 1, 2, 0, 2, 0, 2, 4, 2, 3, 2, 0)
        ];
    public static IndexWheel GetWheel(IndexWheelNumber Number)
        => BuiltInWheels.First(w => w.Name == Number.ToString());
    public string Name { get; } = Name;
    public int[] Stops { get; } = Stops;
    [JsonIgnore]
    public int NumStops { get; } = Stops.Length;
    [JsonIgnore]
    public int NumCodes { get; } = Stops.Distinct().Count();
}
