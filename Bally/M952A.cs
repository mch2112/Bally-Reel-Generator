namespace Bally;

public class M952A : MachineBase
{
    public M952A() : base("M952-A", StockWinAmounts, StockReels) { }
    public M952A(MachineDescriptor Descriptor) : base(Descriptor) { }
    protected M952A(string Name, Reel[] Reels) : base(Name, StockWinAmounts, Reels) { }

    public static SymbolSet Symbols { get; } = new SymbolSet(Symbol.Lemon,
                                                             Symbol.Cherry,
                                                             Symbol.Orange,
                                                             Symbol.Plum,
                                                             Symbol.Bell,
                                                             Symbol.SingleBar,
                                                             Symbol.DoubleBar,
                                                             Symbol.TripleBar);
    private static Dictionary<WinCombo, int> StockWinAmounts { get; }
    private static Reel[] StockReels { get; }
    protected override IndexWheelParams IndexWheelParams => IndexWheelParams.P484;
    static M952A()
    {
        StockWinAmounts = new()
        {
            [WinCombo.OneCherry] = 2,
            [WinCombo.TwoCherries] = 5,
            [WinCombo.Oranges] = 10,
            [WinCombo.Plums] = 14,
            [WinCombo.Bells] = 18,
            [WinCombo.AnyBars] = 20,
            [WinCombo.SingleBars] = 50,
            [WinCombo.DoubleBars] = 100,
            [WinCombo.TripleBars] = 800,
        };
    
        StockReels = [new Reel(IndexWheelNumber.P1233,
                               false,
                               SymbolSet.SplitStringToSymbols("CH OR PL BE 1B 2B 3B LE LE"),
                               ["EL - TBP - 1 - K", "BALLY MFG. CORP.", "7 - 18 , 1972", "M-220-1596"]),
                      new Reel(IndexWheelNumber.P1234,
                               false,
                               SymbolSet.SplitStringToSymbols("CH OR PL BE 1B 2B 3B LE LE"),
                               ["EL - TBP - 2 - K", "BALLY MFG. CORP.", "7 - 18 , 1972", "M-220-1597"]),
                      new Reel(IndexWheelNumber.P1235, 
                               false,
                               SymbolSet.SplitStringToSymbols("OR PL BE 1B 2B 3B LE LE LE"),
                               ["EL - TBP - 3 - K", "BALLY MFG. CORP.", "7 - 18 , 1972", "M-220-1598"])];
    }
    public override WinCombo GetResult(Symbol s1, Symbol s2, Symbol s3)
    {
        if (s1 == Symbol.Cherry)
        {
            return s2 == Symbol.Cherry ? WinCombo.TwoCherries : WinCombo.OneCherry;
        }
        else if (s1 == Symbol.Lemon)
        {
            return WinCombo.None;
        }
        else if (s1 == s2)
        {
            if (s1 == s3)
                return s1 switch
                {
                    Symbol.Orange => WinCombo.Oranges,
                    Symbol.Plum => WinCombo.Plums,
                    Symbol.Bell => WinCombo.Bells,
                    Symbol.SingleBar => WinCombo.SingleBars,
                    Symbol.DoubleBar => WinCombo.DoubleBars,
                    Symbol.TripleBar => WinCombo.TripleBars,
                    _ => throw new NotImplementedException()
                };
            else if (s3 == Symbol.SingleBar || s3 == Symbol.DoubleBar || s3 == Symbol.TripleBar)
                return s1 switch
                {
                    Symbol.Orange => WinCombo.Oranges,
                    Symbol.Plum => WinCombo.Plums,
                    Symbol.Bell => WinCombo.Bells,
                    Symbol.SingleBar => WinCombo.AnyBars,
                    Symbol.DoubleBar => WinCombo.AnyBars,
                    Symbol.TripleBar => WinCombo.AnyBars,
                    _ => throw new NotImplementedException()

                };
        }
        else if ((s1 == Symbol.SingleBar || s1 == Symbol.DoubleBar || s1 == Symbol.TripleBar) &&
                 (s2 == Symbol.SingleBar || s2 == Symbol.DoubleBar || s2 == Symbol.TripleBar) &&
                 (s3 == Symbol.SingleBar || s3 == Symbol.DoubleBar || s3 == Symbol.TripleBar))
        {
            return WinCombo.AnyBars;
        }
        return WinCombo.None;
    }
}