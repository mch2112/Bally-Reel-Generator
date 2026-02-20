using System.Text.Json.Serialization;

namespace Bally;
public class M831ZZSH : MultilineMachineBase, IMachine
{
    public M831ZZSH() : base(MODEL_NAME, StockWinAmounts, StockReels) { }
    [JsonConstructor]
    public M831ZZSH(string Name, params Reel[] Reels) : base(Name, StockWinAmounts, Reels) { }
    public M831ZZSH(MachineDescriptor Descriptor) : base(Descriptor) { }
    static M831ZZSH()
    {
        StockWinAmounts = new()
        {
            [WinCombo.OneCherry] = 2,
            [WinCombo.TwoCherries] = 5,
            [WinCombo.Oranges] = 10,
            [WinCombo.Plums] = 14,
            [WinCombo.Bells] = 18,
            [WinCombo.Melons] = 20,
            [WinCombo.Bars] = 100,
            [WinCombo.Sevens] = (1000 + 500 + 200) / 3
        };
        StockReels = [new Reel(IndexWheel.P168,
                               true,
                               SymbolSet.SplitStringToSymbols("SE OR OR OR CH BA PL ME BE ME"),
                               ["ZZ - 3LPF - 1", "BALLY MFG. CORP.", "11 , 25 , 1973", "M - 222 - 355 - K"]),
                      new Reel(IndexWheel.P168,
                               true,
                               SymbolSet.SplitStringToSymbols("SE ME ME ME CH BA BE PL OR PL"),
                               ["ZZ - 3LPF - 2", "BALLY MFG. CORP.", "11 , 25 , 1973", "M - 222 - 356 - K"]),
                      new Reel(IndexWheel.P169,
                               true,
                               SymbolSet.SplitStringToSymbols("PL ME BA ME OR BE BE BE BE SE"),
                               ["ZZ - 3LPF - 3", "BALLY MFG. CORP.", "11 , 25 , 1973", "M - 222 - 357 - K"])];
    }
    public const string MODEL_NAME = "Model 831-ZZSH";
    public static Dictionary<WinCombo, int> StockWinAmounts { get; }
    public static Reel[] StockReels { get; }
    public static SymbolSet Symbols { get; } = new SymbolSet(Symbol.Cherry,
                                                             Symbol.Orange,
                                                             Symbol.Plum,
                                                             Symbol.Bell,
                                                             Symbol.Melon,
                                                             Symbol.Bar,
                                                             Symbol.Seven);
    public override bool Validate()
    {
        if (!base.Validate())
            return false;

        bool ok = true;
        foreach (var w in PossibleMultiWins.Value)
            if (!validWinCombos.Contains(w))
            {
                ok = false;
            //    Console.WriteLine($"Invalid payout combo: {w}");
                return false;
            }
        
        return ok;
    }
    public override int GetMultipleLinePayout(WinCombo Win)
    {
        int ret = 0;
        foreach (var w in new WinCombo[] { WinCombo.OneCherry, WinCombo.TwoCherries, WinCombo.Oranges, WinCombo.Plums, WinCombo.Bells, WinCombo.Melons, WinCombo.Bars, WinCombo.Sevens})
            if ((Win & w) == w)
                ret += WinAmounts[w];
        return ret;
    }

    public override WinCombo GetResult(Symbol s1, Symbol s2, Symbol s3)
    {
        if (s1 == s2)
        {
            if (s1 == Symbol.Cherry)
            {
                return WinCombo.TwoCherries;
            }
            else if (s2 == s3)
            {
                return s1 switch
                {
                    Symbol.Orange => WinCombo.Oranges,
                    Symbol.Plum => WinCombo.Plums,
                    Symbol.Bell => WinCombo.Bells,
                    Symbol.Melon => WinCombo.Melons,
                    Symbol.Bar => WinCombo.Bars,
                    Symbol.Seven => WinCombo.Sevens,
                    _ => WinCombo.None
                };
            }
            else if (s3 == Symbol.Bar)
            {
                return s1 switch
                {
                    Symbol.Orange => WinCombo.Oranges,
                    Symbol.Plum => WinCombo.Plums,
                    Symbol.Bell => WinCombo.Bells,
                    _ => WinCombo.None
                };
            }
        }
        else if (s1 == Symbol.Cherry)
        {
            return WinCombo.OneCherry;
        }
        return WinCombo.None;
    }
    protected override IndexWheelParams IndexWheelParams => IndexWheelParams.P684;

    // The machine might work with other combos but these are in the stock machine
    private static readonly HashSet<WinCombo> validWinCombos =
            [ WinCombo.None,

              WinCombo.OneCherry,
              WinCombo.TwoCherries,
              WinCombo.Oranges,
              WinCombo.Plums,
              WinCombo.Bells,
              WinCombo.Melons,
              WinCombo.Bars,
              WinCombo.Sevens,

              WinCombo.OneCherry | WinCombo.Oranges,
              WinCombo.OneCherry | WinCombo.Plums,
              WinCombo.OneCherry | WinCombo.Bars,

              WinCombo.TwoCherries | WinCombo.Plums, /* needs test */
              WinCombo.TwoCherries | WinCombo.Bars,
            ];
}
