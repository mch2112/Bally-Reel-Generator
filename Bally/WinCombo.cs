using System.Text.Json.Serialization;

namespace Bally;

[JsonConverter(typeof(JsonStringEnumConverter))]
[Flags]
public enum WinCombo : int
{
    None = 0,
    OneCherry = 1,
    TwoCherries = OneCherry << 1,
    ThreeCherries = TwoCherries << 1,
    Oranges = ThreeCherries << 1,
    Plums = Oranges << 1,
    Bells = Plums << 1,
    Melons = Bells << 1,
    Bars = Melons << 1,
    AnyBars = Bars << 1,
    SingleBars = AnyBars << 1,
    DoubleBars = SingleBars << 1,
    TripleBars = DoubleBars << 1,
    OrangesNatural = TripleBars << 1,
    PlumsNatural = OrangesNatural << 1,
    BellsNatural = PlumsNatural << 1,
    Sevens = BellsNatural << 1 /* keep this last */
}