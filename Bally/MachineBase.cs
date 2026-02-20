using System.Text;
using System.Text.Json.Serialization;

namespace Bally;

public abstract class MachineBase : IMachine
{
    protected const string div = "===============================";
    protected const string div2 = "-------------------------------";

    public string Name { get; }
    public Dictionary<WinCombo, int> WinAmounts { get; }
    public Reel[] Reels { get; }
    public MachineBase(string Name, Dictionary<WinCombo, int> WinAmounts, Reel[] Reels)
    {
        this.WinTypeCounts = new(GetWinTypeCounts);
        this.Payout = new(GetTotalPayout);
        this.PayoutPercent = new(() => this.Payout.Value / (float)NumPossibleReelPositions);
        this.Name = Name;
        this.WinAmounts = WinAmounts;
        this.WinAmounts[WinCombo.None] = 0;
        this.Reels = Reels;
        NumPossibleReelPositions = 1;
        foreach (var r in Reels)
            NumPossibleReelPositions *= r.NumStops;
    }
    public MachineBase(MachineDescriptor Descriptor) : this(Descriptor.Name, Descriptor.WinAmounts, Descriptor.Reels.Select(r => new Reel(r)).ToArray())
    {
    }
    public WinCombo GetResult(int Stop0, int Stop1, int Stop2)
        => GetResult(Reels[0][Stop0], Reels[1][Stop1], Reels[2][Stop2]);
    public string GetRowSymbolsShort(int Stop0, int Stop1, int Stop2)
        => $"{SymbolSet.ToShortSymbolString(Reels[0][Stop0])} {SymbolSet.ToShortSymbolString(Reels[1][Stop1])} {SymbolSet.ToShortSymbolString(Reels[2][Stop2])}";
    public string GetVisual(int Stop0, int Stop1, int Stop2)
        => string.Join(Environment.NewLine,
                       Enumerable.Range(-1, Reels.Length)
                                 .Select(i => GetRowSymbolsShort(Stop0 + i, Stop1 + i, Stop2 + i)));
    public abstract WinCombo GetResult(Symbol s1, Symbol s2, Symbol s3);
    public int GetPayout(int Stop1, int Stop2, int Stop3)
        => WinAmounts[GetResult(Stop1, Stop2, Stop3)];
    public string GetPayoutTable()
    {
        var data = WinTypeCounts.Value
                                .Select(kvp => new
                                {
                                    kvp.Key,
                                    PerWin = WinAmounts[kvp.Key],
                                    Count = kvp.Value,
                                    TypeValue = kvp.Value * WinAmounts[kvp.Key]
                                })
                                .Where(x => x.TypeValue > 0)
                                .OrderByDescending(x => x.PerWin)
                                .ToArray();

        return string.Join(Environment.NewLine,
                           data.Select(win => $"{win.Key,-12} {win.PerWin,3} {win.Count,6:N0} {win.TypeValue,7:N0}"))
               + Environment.NewLine
               + $"TOTAL            {data.Sum(x => x.Count),6:N0} {data.Sum(x => x.TypeValue),7:N0}";
    }
    public string GetReelInfo()
    {
        return "    R1 R2 R3  R1 R2 R3" + Environment.NewLine +
               "    SYMBOLS    CODES  " + Environment.NewLine +
               string.Join(Environment.NewLine, Enumerable.Range(0, Reels[0].NumStops)
                         .Select(i => $"{i + 1:00}: {GetRowSymbolsShort(i, i, i)}  {Reels[0].IndexWheel.Stops[i] + 1:00} {Reels[1].IndexWheel.Stops[i] + 1:00} {Reels[2].IndexWheel.Stops[i] + 1:00}"));
    }
    public string GetSymbolCountTable()
        => GetSymbolCountTable(Reels[0].SymbolCounts.Value,
                               Reels[1].SymbolCounts.Value,
                               Reels[2].SymbolCounts.Value);
    public static string GetSymbolCountTable(Dictionary<Symbol, int[]> SymbolCounts)
        => string.Join(Environment.NewLine,
                       SymbolCounts.Where(kvp => kvp.Value.Sum() > 0)
                                   .Select(kvp => kvp.Key)
                                   .OrderDescending()
                                   .Select(s => SymbolSet.ToShortSymbolString(s) + " " + string.Join(' ', SymbolCounts[s].Select(ss => $"{ss,2}"))));
    public static string GetSymbolCountTable(Dictionary<Symbol, int> Reel1, Dictionary<Symbol, int> Reel2, Dictionary<Symbol, int> Reel3)
        => string.Join(Environment.NewLine,
                       Reel1.Keys
                            .Concat(Reel2.Keys.Concat(Reel3.Keys))
                            .Distinct()
                            .OrderDescending()
                            .Where(s => Reel1[s] > 0 || Reel2[s] > 0 || Reel3[s] > 0)
                            .Select(s => $"{SymbolSet.ToShortSymbolString(s)} {Reel1[s],2} {Reel2[s],2} {Reel3[s],2}"));
    public string GetSymbolMapTable()
        => string.Join(Environment.NewLine,
                       Enumerable.Range(0, Reels[0].IndexWheel.NumCodes)
                                 .Select(i => $"{i + 1,2}  " + string.Join("  ", Reels.Select(r => SymbolSet.ToShortSymbolString(r.SymbolMap[i])))));
    public virtual void GenerateAssets(string AssetsPath, int NumStripCopies = 3)
    {
        Util.WriteText(Path.Combine(AssetsPath, "info.txt"), this.GetInfo());
        for (int i = 0; i < Reels.Length; i++)
        {
            IndexWheelGenerator.Generate(Reels[i].IndexWheel.Stops,
                                         IndexWheelParams,
                                         Path.Combine(AssetsPath, Reels[i].IndexWheel.Name + "Z.svg"));
        }
        ReelStripRenderer.GenerateReelStrips(this,
                                             NumCopies: NumStripCopies,
                                             SvgPath: Path.Combine(AssetsPath, "reelstrips.svg"),
                                             PngPath: Path.Combine(AssetsPath, "reelstrips.png"));

    }
    public Lazy<Dictionary<WinCombo, int>> WinTypeCounts { get; }
    protected virtual IndexWheelParams IndexWheelParams => throw new NotImplementedException();

    [JsonIgnore]
    public int NumPossibleReelPositions { get; }
    [JsonIgnore]
    protected Lazy<int> Payout { get; }
    //[JsonIgnore]
    public Lazy<float> PayoutPercent { get; }
    
    public void ThrowIfInvalid()
    {
        if (!this.Validate())
            throw new Exception("Machine is invalid!");
    }
    public virtual bool Validate() => this.Reels.All(r => r.Validate());
    public MachineBase ValidateOrThrow()
    {
        if (!this.Validate())
            throw new Exception($"Machine '{Name}' Not Valid!");
        return this;
    }
    protected Dictionary<WinCombo, int> GetWinTypeCounts()
    {
        Dictionary<WinCombo, int> ret = [];
        for (int i = 0; i < Reels[0].NumStops; i++)
            for (int j = 0; j < Reels[1].NumStops; j++)
                for (int k = 0; k < Reels[2].NumStops; k++)
                {
                    var res = GetResult(i, j, k);
                    if (!ret.TryAdd(res, 1))
                        ret[res]++;
                }
        return ret;
    }
    protected int GetTotalPayout()
    {
        int totalCoins = 0;
        for (int i = 0; i < Reels[0].NumStops; i++)
            for (int j = 0; j < Reels[1].NumStops; j++)
                for (int k = 0; k < Reels[2].NumStops; k++)
                    totalCoins += GetPayout(i, j, k);
        return totalCoins;
    }
    public MachineDescriptor ToDescriptor() => new()
    {
        Name = this.Name,
        WinAmounts = this.WinAmounts,
        Reels = this.Reels.Select(r => new ReelDescriptor() { IndexWheel = r.IndexWheel, SymbolMap = r.SymbolMap, Multiline = r.Multiline, ReelStripInfo = r.ReelStripInfo }).ToArray()
    };
    public virtual string GetInfo(string? Caption = null)
    {
        StringBuilder sb = new();

        sb.AppendLine(div);
        if (Caption is not null)
        {
            sb.AppendLine(Caption);
            sb.AppendLine(div);
        }
        if (!Validate())
        {
            sb.AppendLine("INVALID MACHINE CONFIGURATION!");
            sb.AppendLine(div2);
        }
        sb.AppendLine();
        sb.AppendLine($"Payout: {this.Payout.Value:N0}");
        sb.AppendLine($"Payout percent: {this.PayoutPercent.Value * 100:N8}%");
        sb.AppendLine($"Combinations: {this.NumPossibleReelPositions}");

        sb.AppendLine("Symbol Counts");
        sb.AppendLine(this.GetSymbolCountTable());
        sb.AppendLine();
        sb.AppendLine("Payout");
        sb.AppendLine(this.GetPayoutTable());
        sb.AppendLine();
        sb.AppendLine("Reel Info");
        sb.AppendLine(this.GetReelInfo());
        sb.AppendLine();
        sb.AppendLine("Symbol Map");
        sb.AppendLine(this.GetSymbolMapTable());
        sb.AppendLine(div);

        return sb.ToString();
    }
}
