namespace Bally;

/// <summary>
/// A different center reel changes Double-Bar into a Plum, so odds go from 
/// 86% to 81%.
/// </summary>
public class M952Av2 : M952A
{
    public M952Av2() : base("M952-A-v2", StockReels) { }
    private static Reel[] StockReels { get; }
    static M952Av2()
    {
        var baseM = new M952A();
        StockReels = [baseM.Reels[0],
                      new Reel(IndexWheelNumber.P1234,
                               false,
                               SymbolSet.SplitStringToSymbols("CH OR PL BE 1B 2B 3B LE LE"),
                               ["EL - TBP - 2A - K", "BALLY MFG. CORP.", "7 - 18 , 1972", "M-220-1599"]),
                      baseM.Reels[2]];
    }
}