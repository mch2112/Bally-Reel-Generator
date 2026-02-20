using System.Text.Json.Serialization;

namespace Bally;

[JsonConverter(typeof(JsonStringEnumConverter))]
[Flags]
public enum WinCombo : int
{
    None        = 0,
    OneCherry   = 1 << 0,
    TwoCherries = 1 << 1,
    Oranges     = 1 << 2,
    Plums       = 1 << 3,
    Bells       = 1 << 4,
    Melons      = 1 << 5,
    Bars        = 1 << 6,
    AnyBars      = 1 << 7,
    SingleBars   = 1 << 8,
    DoubleBars   = 1 << 9,
    TripleBars   = 1 << 10,
    Sevens      = 1 << 11 /* keep this last */
}