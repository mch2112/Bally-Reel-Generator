namespace Bally;

public class MyM952A : M952A
{
    public MyM952A(MachineDescriptor Descriptor) : base(Descriptor) { }
    public MyM952A() : base("Mod-952-A", customReels) { }
    private static readonly Reel[] customReels =
                                 [new Reel(new IndexWheel("P486-Z997", 1, 4, 1, 0, 1, 4, 2, 7, 1, 6, 1, 2, 4, 0, 3, 4, 1, 5, 1, 3),
                                           false,
                                           SymbolSet.SplitStringToSymbols("CH OR PL BE 1B 2B 3B LE LE"),
                                           ["MOD-EL-TBP-1-Z", "BALLY MFG. CORP.", "7 - 18 , 1972", "M-220-Z997"]),
                                  new Reel(new IndexWheel("P486-Z998", 2, 6, 2, 1, 2, 0, 2, 5, 3, 1, 2, 0, 5, 0, 3, 4, 2, 3, 2, 0),
                                           false,
                                           SymbolSet.SplitStringToSymbols("CH OR PL BE 1B 2B 3B LE LE"),
                                           ["MOD-EL-TBP-2-Z", "BALLY MFG. CORP.", "7 - 18 , 1972", "M-220-Z998"]),
                                  new Reel(new IndexWheel("P486-Z999", 2, 5, 2, 0, 2, 0, 2, 4, 1, 0, 2, 1, 2, 0, 6, 3, 2, 0, 1, 6),
                                           false,
                                           SymbolSet.SplitStringToSymbols("OR PL BE 1B 2B 3B LE LE LE"),
                                           ["MOD-EL-TBP-3-Z", "BALLY MFG. CORP.", "7 - 18 , 1972", "M-220-Z999"])];

}
