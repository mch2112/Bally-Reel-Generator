using System.Web;

namespace Bally;

public class MyM831ZZSH : M831ZZSH
{
    public MyM831ZZSH(MachineDescriptor Descriptor) : base(Descriptor) { }
    public MyM831ZZSH() : base("Mod-831ZZSH", customReels) { }
    private MyM831ZZSH(string Name, Reel Reel1, Reel Reel2, Reel Reel3) : base(Name, [Reel1, Reel2, Reel3]) { }
    private static readonly Reel[] customReels =
                     [new Reel(IndexWheel.P168,
                               true,
                               SymbolSet.SplitStringToSymbols("ME BE PL BE BA PL CH OR SE OR"),
                               ["MOD-ZZ-3LPF-1", "BALLY MFG. CORP.", "9 , 10 , 2024", "M - 222 - 355 - Z"]),
                      new Reel(IndexWheel.P168,
                               true,
                               SymbolSet.SplitStringToSymbols("SE ME PL ME CH OR OR BE BA BE"),
                               ["MOD-ZZ-3LPF-2", "BALLY MFG. CORP.", "9 , 10 , 2024", "M - 222 - 356 - Z"]),
                      new Reel(IndexWheel.P169,
                               true,
                               SymbolSet.SplitStringToSymbols("PL ME BA ME OR BE PL BE SE BA"),
                               ["MOD-ZZ-3LPF-3", "BALLY MFG. CORP.", "9 , 10 , 2024", "M - 222 - 357 - Z"])];
    public static void Analyze(string OutputWritePath)
    {
        Symbol[][] ootbSymbolMaps = [SymbolSet.SplitStringToSymbols("SE OR OR OR CH BA PL ME BE ME"),
                                     SymbolSet.SplitStringToSymbols("SE ME ME ME CH BA BE PL OR PL"),
                                     SymbolSet.SplitStringToSymbols("PL ME BA ME OR BE BE BE BE SE")];

        List<MyM831ZZSH> candidates = [];
        var validMachinesJsonPath = Path.Combine(OutputWritePath, "machines.json");

        if (AskGenerateCandidateList(validMachinesJsonPath))
        {
            Console.WriteLine("Generating candidate machines...");

            if (true)
            {
                foreach (var z in analyze3())
                {
                    Console.WriteLine(z.GetInfo());
                    candidates.Add(z);
                }
                Console.WriteLine($"Found {candidates.Count:N0} candidates.");
                Util.SerializeToFile(validMachinesJsonPath, candidates
                    .MinBy(m => symbolMapDiffCount(ootbSymbolMaps, m.Reels[0].SymbolMap, m.Reels[1].SymbolMap, m.Reels[2].SymbolMap)));

                var m = candidates.First();
                Console.WriteLine(m.GetInfo());
                Console.WriteLine($"optimum requires rewiring {symbolMapDiffCount(ootbSymbolMaps, m.Reels[0].SymbolMap, m.Reels[1].SymbolMap, m.Reels[2].SymbolMap)} different paths.");

                return;
            }
            else
            {
                foreach (var m in analyze2())
                {
                    candidates.Add(m);
                    Console.WriteLine(m.GetInfo());
                }
                if (candidates.Count == 0)
                {
                    Console.WriteLine("No solutions found.");
                    return;
                }
                else
                {
                    Console.WriteLine($"Found {candidates.Count:N0} candidates.");
                    Util.SerializeToFile(validMachinesJsonPath, candidates.OrderBy(m => Math.Abs(1 - m.PayoutPercent.Value)).Select(m => m.ToDescriptor()));
                }
            }
        }
        else
        {
            Console.WriteLine("Loading candidates...");
            candidates = Util.DeserializeFromFile<List<MachineDescriptor>>(validMachinesJsonPath)
                             .Select(d => new MyM831ZZSH(d)).ToList();
        }

        var optimum = candidates.MinBy(m => symbolMapDiffCount(ootbSymbolMaps, m.Reels[0].SymbolMap, m.Reels[1].SymbolMap, m.Reels[2].SymbolMap));

        Console.WriteLine(optimum.GetInfo("OPTIMUM"));

        new ReelStripRenderer().GenerateReelStrips(Machine: optimum!,
                                                   NumCopies: 1,
                                                   SvgPath: Path.Combine(OutputWritePath, "reels-m831zzsh.svg"),
                                                   PngPath: Path.Combine(OutputWritePath, "reels-m831zzsh.png"));

        Console.WriteLine($"optimum requires rewiring {symbolMapDiffCount(ootbSymbolMaps, optimum.Reels[0].SymbolMap, optimum.Reels[1].SymbolMap, optimum.Reels[2].SymbolMap)} different paths.");

        IEnumerable<MyM831ZZSH> analyze2()
        {
            IndexWheel[] indexWheels = [IndexWheel.P168, IndexWheel.P168, IndexWheel.P169];
            Analyzer<MyM831ZZSH> a = new();

            var sc = a.FindCandidateSymbolCounts(22,
                                                     MinSymbolCounts:
                                                         [[3, 3, 3, 3, 1, 1, 1],
                                                          [3, 3, 3, 3, 1, 1, 1],
                                                          [0, 3, 3, 3, 1, 1, 1]],

                                                     MaxSymbolCounts:
                                                         [[3, 7, 7, 7, 7, 3, 1],
                                                          [3, 7, 7, 7, 7, 3, 1],
                                                          [0, 7, 7, 7, 7, 3, 1]])

                .Where(MyM831ZZSH.DistributionCheck)
                .ToArray();

            var symbols = M831ZZSH.Symbols;

            Console.WriteLine($"Found {sc.Length} candidate patterns.");

            for (int i = 0; i < sc.Length; i++)
            {
                Console.WriteLine($"Checking pattern {i:N0} of {sc.Length:N0}...");
                foreach (var m in a.FindPossibleMachines(indexWheels,
                                                         symbols,
                                                         sc[i],
                                                         0.92f,
                                                         1.001f,
                                                         (r1, r2, r3) => new MyM831ZZSH("831-Candidate", r1, r2, r3),
                                                         ReelFilter,
                                                         1,
                                                         100))
                    yield return m;
            }

        }

        IEnumerable<MyM831ZZSH> analyze3()
        {
            Analyzer<MyM831ZZSH> a = new();
            var r168Perms = GetPermutations(2, 3, 4, 5, 6, 7);
            var r169Perms = GetPermutations(3, 4, 5, 6);

            Random rnd = new();
            for (int z = 0; z < 50; z++)
            {
                //int[] mod168 = IndexWheel.P168.Stops.ToArray();
                //var aa = rnd.Next(0, r168Perms.Count);
                //for (int i = 0; i < 3; i++)
                //    for (int j = 0; j < 6; j++)
                //        mod168[i * 6 + j + 3] = r168Perms[aa][j];

                //int[] mod168b = IndexWheel.P168.Stops.ToArray();
                //var aab = rnd.Next(0, r168Perms.Count);
                //for (int i = 0; i < 3; i++)
                //    for (int j = 0; j < 6; j++)
                //        mod168b[i * 6 + j + 3] = r168Perms[aab][j];


                //int[] mod169 = IndexWheel.P169.Stops.ToArray();
                //var bb = rnd.Next(0, r169Perms.Count);
                //for (int i = 0; i < 4; i++)
                //    for (int j = 0; j < 4; j++)
                //        mod169[i * 4 + j + 5] = r169Perms[bb][j];


                //IndexWheel[] indexWheels = [new IndexWheel("p168_mod", mod168),
                //                            new IndexWheel("p168_mod_b", mod168b),
                //                            new IndexWheel("p169_mod", mod169)];

                IndexWheel[] indexWheels = [IndexWheel.P168, IndexWheel.P168, IndexWheel.P169];

                //IndexWheel[] indexWheels = [IndexWheel.P168, IndexWheel.P168, new IndexWheel("p169_mod", 9, 0, 8, 2, 7, 6, 3, 4, 5, 6, 3, 4, 5, 6, 3, 4, 5, 6, 3, 4, 5, 1)];  //20
                var symbols = M831ZZSH.Symbols;
                int[][] sc = [[3, 4, 6, 4, 1, 3, 1],
                              [3, 6, 3, 4, 4, 1, 1],
                              [0, 4, 5, 5, 5, 2, 1]];

                foreach (var m in a.FindPossibleMachines(indexWheels,
                                                         symbols,
                                                         sc,
                                                         0,
                                                         float.MaxValue,
                                                         (r1, r2, r3) => new MyM831ZZSH("831-Candidate", r1, r2, r3),
                                                         ReelFilter,
                                                         int.MaxValue,
                                                         int.MaxValue))
                {
                    var dc = symbolMapDiffCount(ootbSymbolMaps, m.Reels[0].SymbolMap, m.Reels[1].SymbolMap, m.Reels[2].SymbolMap);
                    if (dc < 20)
                    {
                        Console.WriteLine(m.GetInfo());
                        Console.WriteLine($"{dc} rewirings!");
                        yield return m;
                    }
                }
            }
            yield break;
        }
        static bool AskGenerateCandidateList(string validMachinesJsonPath)
        {
            if (!File.Exists(validMachinesJsonPath))
                return true;

            while (true)
            {
                Console.WriteLine();
                Console.Write("Regenerate candidate machines?");
                var key = Console.ReadKey().Key;
                Console.WriteLine();
                Console.WriteLine();
                switch (key)
                {
                    case ConsoleKey.Y:
                        return true;
                    case ConsoleKey.N:
                    case ConsoleKey.Escape:
                    case ConsoleKey.Enter:
                        return false;
                }
            }
        }
        static int symbolMapDiffCount(Symbol[][] maps1, params Symbol[][] maps2)
        {
            var m1 = maps1.ToArray();
            var m2 = maps2.ToArray();

            int diffCount = 0;
            for (int i = 0; i < m1.Length; i++)
            {
                for (int j = 0; j < m1[i].Length; j++)
                {
                    if (m1[i][j] != m2[i][j])
                        diffCount++;
                }
            }
            return diffCount;
        }
    }
    static bool ReelFilter(Reel r, int Num)
    {
        if (Num == 0)
            for (int i = 0; i < r.Symbols.Length; i++)
                if (r[i] == Symbol.Cherry)
                    if (r[i - 1] == Symbol.Bell || r[i - 2] == Symbol.Bell || r[i + 1] == Symbol.Bell || r[i + 2] == Symbol.Bell ||
                        r[i - 1] == Symbol.Melon || r[i - 2] == Symbol.Melon || r[i + 1] == Symbol.Melon || r[i + 2] == Symbol.Melon ||
                        r[i - 1] == Symbol.Seven || r[i - 2] == Symbol.Seven || r[i + 1] == Symbol.Seven || r[i + 2] == Symbol.Seven)
                        return false;
        return true;
    }
    static bool DistributionCheck(int[][] sc)
    {
        var cherries = sc[0][(int)Symbol.Cherry] * sc[1][(int)Symbol.Cherry];
        var oranges = sc[0][(int)Symbol.Orange] * sc[1][(int)Symbol.Orange] * (sc[2][(int)Symbol.Orange] + sc[2][(int)Symbol.Bar]);
        var plums = sc[0][(int)Symbol.Plum] * sc[1][(int)Symbol.Plum] * (sc[2][(int)Symbol.Plum] + sc[2][(int)Symbol.Bar]);
        var bells = sc[0][(int)Symbol.Bell] * sc[1][(int)Symbol.Bell] * (sc[2][(int)Symbol.Bell] + sc[2][(int)Symbol.Bar]);
        var melons = sc[0][(int)Symbol.Melon] * sc[1][(int)Symbol.Melon] * sc[2][(int)Symbol.Melon];
        var bars = sc[0][(int)Symbol.Bar] * sc[1][(int)Symbol.Bar] * sc[2][(int)Symbol.Bar];
        var sevens = sc[0][(int)Symbol.Seven] * sc[1][(int)Symbol.Seven] * sc[2][(int)Symbol.Seven];

        return cherries >= 8 && cherries <= 12 && oranges <= 150 && plums <= oranges && bells <= plums && melons <= bells && bars <= melons * 5 / 10 && melons >= 20 && bars >= 6 && sevens <= 2;
        //return cherries >= 8 && cherries <= 12 && oranges <= 150 && plums <= oranges * 15 / 10 && bells <= plums * 15 / 10 && melons <= bells * 15 / 10 && bars <= melons * 15 / 10 && sevens <= 2;
    }

    static List<List<int>> GetPermutations(params int[] nums)
    {
        var list = new List<List<int>>();
        return DoPermute(nums, 0, nums.Length - 1, list);
    }

    static List<List<int>> DoPermute(int[] nums, int start, int end, List<List<int>> list)
    {
        if (start == end)
        {
            // We have one of our possible n! solutions,
            // add it to the list.
            list.Add(new List<int>(nums));
        }
        else
        {
            for (var i = start; i <= end; i++)
            {
                swap(ref nums[start], ref nums[i]);
                DoPermute(nums, start + 1, end, list);
                swap(ref nums[start], ref nums[i]);
            }
        }

        return list;

        static void swap(ref int a, ref int b)
        {
            (a, b) = (b, a);
        }
    }
}