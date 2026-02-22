namespace Bally;
public class M1114 : MachineBase
{
    public M1114(bool HighPay = true) : base("M1114", StockWinAmounts, HighPay ? StockReelsHigh : StockReelsLow) { }
    public M1114(MachineDescriptor Descriptor) : base(Descriptor) { }
    protected M1114(string Name, Reel[] Reels) : base(Name, StockWinAmounts, Reels) { }
    protected static Symbol[] Symbols { get; set; } = SymbolSet.SplitStringToSymbols("CH OR PL BE BA SE LE");
    private static Dictionary<WinCombo, int> StockWinAmounts { get; }
    private static Reel[] StockReelsHigh { get; }
    private static Reel[] StockReelsLow { get; }
    protected override IndexWheelParams IndexWheelParams => IndexWheelParams.P684;
    
    static M1114()
    {
        StockWinAmounts = new()
        {
            [WinCombo.OneCherry] = 2,
            [WinCombo.TwoCherries] = 5,
            [WinCombo.Oranges] = 10,
            [WinCombo.Plums] = 14,
            [WinCombo.Bells] = 18,
            [WinCombo.ThreeCherries] = 20,
            [WinCombo.OrangesNatural] = 20,
            [WinCombo.PlumsNatural] = 20,
            [WinCombo.BellsNatural] = 20,
            [WinCombo.Bars] = 50,
            [WinCombo.Sevens] = 100
        };

        StockReelsHigh = [new Reel(IndexWheelNumber.P305, false, Symbols, ["DOL. 3 COIN MULT. 1", "BALLY MFG. CORP.", "1 , 25 , 1977", "M-239-4    K"]),
                          new Reel(IndexWheelNumber.P347, false, Symbols, ["DOL. 3 COIN MULT. 2", "BALLY MFG. CORP.", "1 , 25 , 1977", "M-239-5    K"]),
                          new Reel(IndexWheelNumber.P307, false, Symbols, ["DOL. 3 COIN MULT. 3", "BALLY MFG. CORP.", "1 , 25 , 1977", "M-239-6    K"])];
    
        StockReelsLow =  [new Reel(IndexWheelNumber.P305, false, Symbols, ["DOL. 3 COIN MULT. 1", "BALLY MFG. CORP.", "1 , 25 , 1977", "M-239-13    K"]),
                          new Reel(IndexWheelNumber.P306, false, Symbols, ["DOL. 3 COIN MULT. 2", "BALLY MFG. CORP.", "1 , 25 , 1977", "M-239-14    K"]),
                          new Reel(IndexWheelNumber.P307, false, Symbols, ["DOL. 3 COIN MULT. 3", "BALLY MFG. CORP.", "1 , 25 , 1977", "M-239-15    K"])];
    }
    public override WinCombo GetResult(Symbol s1, Symbol s2, Symbol s3)
    {
        // Cherries

        if (s1 == Symbol.Cherry)
        {
            if (s2 == Symbol.Cherry)
            {
                if (s3 == Symbol.Cherry)
                    return WinCombo.ThreeCherries;
                else
                    return WinCombo.TwoCherries;
            }
            else if (s3 == Symbol.Cherry)
            {
                return WinCombo.TwoCherries;
            }
            else
            {
                return WinCombo.OneCherry;
            }
        }
        else if (s2 == Symbol.Cherry)
        {
            if (s3 == Symbol.Cherry)
                return WinCombo.TwoCherries;
            else
                return WinCombo.None; /* Middle cherry alone doesn't pay */
        }
        else if (s3 == Symbol.Cherry)
        {
            return WinCombo.OneCherry;
        }

        // Oranges

        if (s1 == Symbol.Orange && s2 == Symbol.Orange)
        {
            if (s3 == Symbol.Orange)
                return WinCombo.OrangesNatural;
            else if (s3 == Symbol.Bar)
                return WinCombo.Oranges;
            else
                return WinCombo.None;
        }
        else if (s2 == Symbol.Orange && s3 == Symbol.Orange)
        {
            if (s1 == Symbol.Orange)
                return WinCombo.OrangesNatural;
            else if (s1 == Symbol.Bar)
                return WinCombo.Oranges;
            else
                return WinCombo.None;
        }

        // Plums

        if (s1 == Symbol.Plum && s2 == Symbol.Plum)
        {
            if (s3 == Symbol.Plum)
                return WinCombo.PlumsNatural;
            else if (s3 == Symbol.Bar)
                return WinCombo.Plums;
            else
                return WinCombo.None;
        }
        else if (s2 == Symbol.Plum && s3 == Symbol.Plum)
        {
            if (s1 == Symbol.Plum)
                return WinCombo.PlumsNatural;
            else if (s1 == Symbol.Bar)
                return WinCombo.Plums;
            else
                return WinCombo.None;
        }

        // Bells

        if (s1 == Symbol.Bell && s2 == Symbol.Bell)
        {
            if (s3 == Symbol.Bell)
                return WinCombo.BellsNatural;
            else if (s3 == Symbol.Bar)
                return WinCombo.Bells;
            else
                return WinCombo.None;
        }
        else if (s2 == Symbol.Bell && s3 == Symbol.Bell)
        {
            if (s1 == Symbol.Bell)
                return WinCombo.BellsNatural;
            else if (s1 == Symbol.Bar)
                return WinCombo.Bells;
            else
                return WinCombo.None;
        }

        // Bars

        if (s1 == Symbol.Bar && s2 == Symbol.Bar && s3 == Symbol.Bar)
        {
            return WinCombo.Bars;
        }

        // Sevens

        if (s1 == Symbol.Seven && s2 == Symbol.Seven && s3 == Symbol.Seven)
        {
            return WinCombo.Sevens;
        }

        return WinCombo.None;
    }
}
