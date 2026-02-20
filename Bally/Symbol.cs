using System.Text.Json.Serialization;

namespace Bally;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Symbol
{
    Lemon,
    Cherry,
    Orange,
    Plum,
    Bell,
    Melon,
    Bar,
    SingleBar,
    DoubleBar,
    TripleBar,
    Seven
}
public class SymbolSet(params Symbol[] Symbols)
{
    public SymbolSet(IEnumerable<Symbol> Symbols) : this(Symbols.ToArray()) { }
    private Symbol[] Symbols { get; } = Symbols;
    public int Count => Symbols.Length;
    public Symbol this[int Index] => Symbols[Index];
    public IEnumerator<Symbol> GetEnumerator() => (IEnumerator<Symbol>)Symbols.GetEnumerator();
    public static Symbol[] SplitStringToSymbols(string Input)
    {
        return Input.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                    .Select(ToSymbol)
                    .ToArray();
    }
    public static string ToShortSymbolString(Symbol Symbol)
    {
        return Symbol switch
        {
            Symbol.Seven => "SE",
            Symbol.Bar => "BA",
            Symbol.SingleBar => "1B",
            Symbol.DoubleBar => "2B",
            Symbol.TripleBar => "3B",
            Symbol.Melon => "ME",
            Symbol.Bell => "BE",
            Symbol.Plum => "PL",
            Symbol.Orange => "OR",
            Symbol.Cherry => "CH",
            Symbol.Lemon => "LE",
            _ => throw new Exception()
        };
    }
    public static string ToShortSymbolString(IEnumerable<Symbol> Symbols) => string.Join(' ', Symbols.Select(ToShortSymbolString));
    public static Symbol ToSymbol(string ShortSymbolString)
    {
        return ShortSymbolString switch
        {
            "SE" => Symbol.Seven,
            "3B" => Symbol.TripleBar,
            "2B" => Symbol.DoubleBar,
            "1B" => Symbol.SingleBar,
            "BA" => Symbol.Bar,
            "ME" => Symbol.Melon,
            "BE" => Symbol.Bell,
            "PL" => Symbol.Plum,
            "OR" => Symbol.Orange,
            "CH" => Symbol.Cherry,
            "LE" => Symbol.Lemon,
            _ => throw new Exception($"{ShortSymbolString} is not a valid symbol short string.")

        };
    }
    public static Symbol[] ToSymbols(string[] ShortSymbolStrings) => ShortSymbolStrings.Select(ToSymbol).ToArray();
    public Symbol[] ToSymbols(int[] Indexes) => Indexes.Select(i => Symbols[i]).ToArray();

}