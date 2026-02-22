namespace Bally;

public class MyM1114 : M1114
{
    public MyM1114() : base("Mod-1114", customReels)
    {
    }

    private static readonly Reel[] customReels =
                                 [new Reel(new IndexWheel("P684-Z997", 5, 3, 2, 5, 0, 3, 1, 2, 4, 6, 2, 3, 0, 2, 3, 2, 6, 4, 1, 3, 2, 3),
                                           false,
                                           Symbols,
                                           ["MOD DOL. 3 COIN MULT. 1", "BALLY MFG. CORP.", "1 , 25 , 1977", "M-239-z997"]),
                                  new Reel(new IndexWheel("P684-Z998", 5, 1, 4, 0, 6, 3, 4, 0, 6, 1, 4, 0, 3, 1, 4, 1, 6, 3, 4, 2, 4, 0),
                                           false,
                                           Symbols,
                                           ["MOD DOL. 3 COIN MULT. 2", "BALLY MFG. CORP.", "1 , 25 , 1977", "M-239-Z998"]),
                                  new Reel(new IndexWheel("P684-Z999", 5, 1, 2, 1, 2, 0, 6, 4, 2, 1, 6, 4, 2, 1, 2, 3, 5, 0, 2, 1, 2, 1),
                                           false,
                                           Symbols,
                                           ["MOD DOL. 3 COIN MULT. 3", "BALLY MFG. CORP.", "1 , 25 , 1977", "M-239-Z999"])];
}
