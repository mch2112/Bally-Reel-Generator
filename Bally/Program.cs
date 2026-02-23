using Bally;

Console.WriteLine("Bally Asset Generation");
Console.WriteLine("----------------------");
Console.WriteLine();

var basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Bally");


go(new M952A(), "M952A", basePath, 3);
//go(new MyM952A(), "My_M952", basePath, 2);

//go(new M831ZZSH(), "M831ZZSH", basePath, 3);
//go(new MyM831ZZSH(), "My_M831", basePath, 2);

//go(new M1114(), "M1114", basePath, 2);
//go(new MyM1114(), "My_M1114", basePath, 2);


static void go(MachineBase m, string Slug, string BasePath, int ReelStripCopies = 1)
{
    m.ValidateOrThrow();
    Console.WriteLine(m.GetInfo());
    m.GenerateAssets(Path.Combine(BasePath, Slug), ReelStripCopies);
}

void generateInsert()
{
    GlassInsertRenderer.Generate("""
                                 ALL
                                 JACKPOTS
                                 PAID BY
                                 MACHINE
                                 EXCEPT
                                 SUPER
                                 JACKPOT
                                 PAID BY
                                 ATTENDANT
                                 """,
        1.6f,
        5,
        Path.Combine(basePath, "My_M952", "insert.svg"),
        Path.Combine(basePath, "My_M952", "insert.png"));
}
