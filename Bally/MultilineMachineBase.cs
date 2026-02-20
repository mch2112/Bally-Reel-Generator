using System.Text;

namespace Bally;

public abstract class MultilineMachineBase : MachineBase
{
    public MultilineMachineBase(string Name, Dictionary<WinCombo, int> WinAmounts, params Reel[] Reels) : base(Name, WinAmounts, Reels)
    {
        this.FractionOfWinsWithThreeCoins = new(() =>
        {
            int wins = 0;
            for (int i = 0; i < Reels[0].NumStops; i++)
                for (int j = 0; j < Reels[1].NumStops; j++)
                    for (int k = 0; k <= Reels[2].NumStops; k++)
                        if (GetThreeLineResult(i, j, k) != WinCombo.None)
                            wins++;
            return (float)wins / NumPossibleReelPositions;
        });
        this.PossibleMultiWins = new(() =>
        {
            HashSet<WinCombo> ret = [];
            for (int i = 0; i < Reels[0].NumStops; i++)
                for (int j = 0; j < Reels[1].NumStops; j++)
                    for (int k = 0; k <= Reels[2].NumStops; k++)
                        ret.Add(GetThreeLineResult(i, j, k));
            return ret;
        });
    }
    public MultilineMachineBase(MachineDescriptor Descriptor) : this(Descriptor.Name, Descriptor.WinAmounts, [.. Descriptor.Reels.Select(r => new Reel(r))]) { }
    public Lazy<float> FractionOfWinsWithThreeCoins { get; }
    public Lazy<HashSet<WinCombo>> PossibleMultiWins { get; }
    public WinCombo GetThreeLineResult(int Stop1, int Stop2, int Stop3)
    {
        return GetResult(Stop1 - 1, Stop2 - 1, Stop3 - 1) |
               GetResult(Stop1, Stop2, Stop3) |
               GetResult(Stop1 + 1, Stop2 + 1, Stop3 + 1);
    }
    public abstract int GetMultipleLinePayout(WinCombo Win);
    public override string GetInfo(string? Caption = null)
    {
        var sb = new StringBuilder();
        sb.AppendLine(base.GetInfo(Caption ?? Name));
        sb.AppendLine("Three Line Combinations");
        HashSet<WinCombo> mw = new(this.PossibleMultiWins.Value.OrderByDescending(GetMultipleLinePayout));
        foreach (var w in mw)
            sb.AppendLine($"{w}: {GetMultipleLinePayout(w)}");
        sb.AppendLine(div);
        return sb.ToString();
    }

    public bool WinCombosValid(HashSet<WinCombo> ValidWinCombos)
        => !this.PossibleMultiWins.Value.Except(ValidWinCombos).Any();
}
