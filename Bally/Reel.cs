using System.Text.Json.Serialization;

namespace Bally;

public class Reel
{
    public int NumStops { get; }
    public Symbol[] SymbolMap { get; }
    public Symbol[] Symbols { get; }
    [JsonIgnore]
    public IndexWheel IndexWheel { get; }
    public bool Multiline { get; }
    public string[] ReelStripInfo { get; }
    private static readonly string[] DefaultReelStripInfo = ["Bally"];
    public Reel(IndexWheel IndexWheel, bool Multiline, Symbol[] SymbolMap, string[]? ReelStripInfo = null)
    {
        this.IndexWheel = IndexWheel;
        this.Multiline = Multiline;
        this.ReelStripInfo = ReelStripInfo ?? DefaultReelStripInfo;
        this.SymbolMap = [..SymbolMap];
        this.Symbols = this.IndexWheel.Stops.Select(i => SymbolMap[i]).ToArray();
        this.NumStops = this.IndexWheel.Stops.Length;

        this.SymbolCounts = new(() => Enumerable.Range(0, (int)Symbol.Seven + 1)
                                                .ToDictionary(i => (Symbol)i, i => this.Symbols.Count(s => (int)s == i)));

        this.NumSoloSymbols = new(() => SymbolCounts.Value.Values.Count(x => x == 1));
        this.NumDistinctSymbols = new(() => SymbolCounts.Value.Values.Count(x => x > 0));
        this.MinSymbolCount = new(() => SymbolCounts.Value.Values.Where(v => v > 0).Min());
        this.MaxSymbolCount = new(SymbolCounts.Value.Values.Max);

        this.SymbolVariance = new(() =>
        {
            var vals = SymbolCounts.Value.Values;
            var mean = NumStops / vals.Count;
            return vals.Sum(x => (x - mean) * (x - mean));
        });
    }
    public Reel(IndexWheelNumber IndexDiscNumber, bool Multiline, Symbol[] SymbolMap, string[]? ReelStripInfo = null)
        : this(IndexWheel.GetWheel(IndexDiscNumber), Multiline, SymbolMap, ReelStripInfo) { }
    public Reel(ReelDescriptor Descriptor) : this(Descriptor.IndexWheel, Descriptor.Multiline, Descriptor.SymbolMap, Descriptor.ReelStripInfo) { }
    public Symbol this[int Index] => this.Symbols[(Index + NumStops) % NumStops];
    private int GetPattern(int i) => ((int)this[i - 1] << 8) | ((int)this[i] << 4) | (int)this[i + 1];
    public string GetSymbolCountTable()
        => string.Join(Environment.NewLine,
                       SymbolCounts.Value
                                   .OrderByDescending(s => s.Key)
                                   .Select(kvp => $"{SymbolSet.ToShortSymbolString(kvp.Key)} {kvp.Value,2}"));
    public bool Validate()
    {
        // NO CONSECUTIVE SYMBOLS
        for (int i = 1; i < NumStops; i++)
            if (this.Symbols[i] == this.Symbols[i - 1])
                return false;

        if (this.Symbols[0] == this.Symbols[NumStops - 1])
            return false;

        if (this.Multiline)
        {
            // EACH STOP NUMBER MUST HAVE THE SAME PATTERN
            if (Enumerable.Range(0, NumStops)
                          .GroupBy(i => IndexWheel.Stops[i], GetPattern)
                          .Any(x => x.Distinct().Count() > 1))
                return false;
        }
        return true;
    }
    public static List<Reel> TryCreateReels(SymbolSet Symbols, int[] SymbolCounts, IndexWheel IndexWheel, Func<Reel, bool> Filter)
    {
        var codeCounts = new int[IndexWheel.NumCodes];
        foreach (var x in IndexWheel.Stops)
            codeCounts[x]++;

        List<Reel> result = [];

        var symbolMapLength = IndexWheel.NumCodes;
        var symbolMap = new int[symbolMapLength];
        int[] symbolCounts = [.. SymbolCounts];
        var numSymbols = symbolCounts.Length;
        go(0);

        return result;

        void go(int idx)
        {
            if (idx >= symbolMapLength)
            {
                var r = new Reel(IndexWheel, true, Symbols.ToSymbols(symbolMap));
                for (int i = 0; i < SymbolCounts.Length; i++)
                    if (r.SymbolCounts.Value[Symbols[i]] != SymbolCounts[i])
                        Console.WriteLine("????");
                if (Filter(r) && r.Validate())  
                    result.Add(r);
                return;
            }
            for (var s = 0; s < numSymbols; s++)
            {
                if (symbolCounts[s] >= codeCounts[idx])
                {
                    symbolMap[idx] = s;
                    symbolCounts[s] -= codeCounts[idx];
                    go(idx + 1);
                    symbolCounts[s] += codeCounts[idx];
                }
            }
        }
    }
    public int GetSymbolMapDiffs(Reel Other)
    {
        var numToCompare = Math.Min(this.SymbolMap.Length, Other.SymbolMap.Length);
        var excess = Math.Abs(this.SymbolMap.Length - Other.SymbolMap.Length);

        return Enumerable.Range(0, numToCompare).Count(i => this.SymbolMap[i] != Other.SymbolMap[i]) + excess;
    }
    [JsonIgnore]
    public int NumDistinctPatterns
    {
        get
        {
            List<int> patterns = [];

            for (int i = 0; i < NumStops; i++)
                patterns.Add(GetPattern(i));

            return patterns.Distinct()
                           .Count();
        }
    }
    [JsonIgnore]
    public Lazy<int> NumDistinctSymbols { get; }
    [JsonIgnore]
    public Lazy<int> MaxSymbolCount { get; }
    [JsonIgnore]
    public Lazy<int> MinSymbolCount { get; }
    [JsonIgnore]
    public Lazy<int> NumSoloSymbols { get; }
    [JsonIgnore]
    public Lazy<float> SymbolVariance { get; }
    [JsonIgnore]
    public Lazy<Dictionary<Symbol, int>> SymbolCounts { get; }
    public override string ToString() => string.Join(' ', this.Symbols.Select(SymbolSet.ToShortSymbolString));
}
