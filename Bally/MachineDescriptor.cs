namespace Bally;

public class MachineDescriptor
{
    public required string Name { get; set; }
    public required Dictionary<WinCombo, int> WinAmounts { get; set; }
    public required ReelDescriptor[] Reels { get; set; }
}
public class ReelDescriptor
{
    public required Symbol[] SymbolMap { get; set; }
    public required IndexWheel IndexWheel { get; set; }
    public bool Multiline { get; set; }
    public string[]? ReelStripInfo { get; set; }
}
