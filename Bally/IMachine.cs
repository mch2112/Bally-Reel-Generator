
    namespace Bally
    {
        public interface IMachine
        {
            int NumPossibleReelPositions { get; }
            Lazy<float> PayoutPercent { get; }
            Reel[] Reels { get; }
            string GetInfo(string? Caption = null);
            int GetPayout(int Stop1, int Stop2, int Stop3);
            string GetPayoutTable();
            string GetReelInfo();
            WinCombo GetResult(int Stop0, int Stop1, int Stop2);
            string GetRowSymbolsShort(int Stop0, int Stop1, int Stop2);
            string GetSymbolCountTable();
            string GetSymbolMapTable();
            bool Validate();
            MachineDescriptor ToDescriptor();
            string GetVisual(int Stop0, int Stop1, int Stop2);
        }
    }