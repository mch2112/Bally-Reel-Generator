using static SkiaSharp.HarfBuzz.SKShaper;
using System.Collections.Generic;

namespace Bally;

public class Analyzer<T> where T : IMachine
{

    public IEnumerable<T> FindPossibleMachines(IndexWheel[] IndexWheels,
                                               SymbolSet Symbols,
                                               int[][] SymbolCounts,
                                               float MinPayoutPercent,
                                               float MaxPayoutPercent,
                                               Func<Reel, Reel, Reel, T> MachineCreator,
                                               Func<Reel, int, bool>? ReelFilter = null,
                                               int MaxCandidatesPerPattern = 20,
                                               int MaxTriesPerReel = 50)
    {
        Random rnd = new();

        ReelFilter ??= (_, _) => true;

        var reels = Enumerable.Range(0, IndexWheels.Length)
                              .Select(i => Reel.TryCreateReels(Symbols, SymbolCounts[i], IndexWheels[i], r => ReelFilter(r, i)).OrderBy(x => rnd.Next()).Take(MaxTriesPerReel))
                              .ToArray();

        bool first = true;
        foreach (var r1 in reels[0])
            foreach (var r2 in reels[1])
                foreach (var r3 in reels[2])
                {
                    var m = MachineCreator(r1, r2, r3);
                    if (first)
                    {
                        if (m.PayoutPercent.Value < MinPayoutPercent || m.PayoutPercent.Value > MaxPayoutPercent)
                            yield break;
                        first = false;
                    }
                    if (m.Validate())
                    {
                        yield return m;
                        if (--MaxCandidatesPerPattern < 0)
                            yield break;
                    }
                }
    }

    public IEnumerable<int[][]> FindCandidateSymbolCounts(int SymbolsPerReel,
                                                          int[][] MinSymbolCounts,
                                                          int[][] MaxSymbolCounts)
    {
        for (int i = 0; i < MinSymbolCounts.Length; i++)
        {
            Console.WriteLine($"Reel {i + 1}");
            Console.WriteLine("------");
            for (int j = 0; j < MinSymbolCounts[i].Length; j++)
                if (MinSymbolCounts[i][j] > 0)
                    Console.WriteLine($"{(Symbol)j}: {MinSymbolCounts[i][j]} - {MaxSymbolCounts[i][j]}");
            Console.WriteLine("");
        }
        
        var q = Enumerable.Range(0, MinSymbolCounts.Length)
                          .Select(i => Util.GetDivisions(MinSymbolCounts[i],
                                                         MaxSymbolCounts[i],
                                                         SymbolsPerReel)
                          .ToArray()).ToArray();

        if (q.Length != 3)
            throw new NotImplementedException();

        foreach (var a in q[0])
            foreach (var b in q[1])
                foreach (var c in q[2])
                    yield return [a, b, c];
    }
}
    