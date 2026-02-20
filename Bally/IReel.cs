
//using System.Text.Json.Serialization;

//namespace Bally
//{
//    public interface IReel
//    {
//        Symbol this[int Index] { get; }
//        IndexWheel IndexWheel { get; }
//        int NumDistinctPatterns { get; }
//        bool Multiline { get; }
//        int NumStops { get; }
//        Symbol[] SymbolMap { get; }
//        Symbol[] Symbols { get; }
//        bool Validate();
//        string GetSymbolCountTable();
//        int GetSymbolMapDiffs(Reel Other);
//        string[] ReelStripInfo { get; } 
//        string ToString();
//        [JsonIgnore]
//        Lazy<int> MaxSymbolCount { get; }
//        [JsonIgnore]
//        Lazy<int> MinSymbolCount { get; }
//        [JsonIgnore]
//        Lazy<int> NumSoloSymbols { get; }
//        [JsonIgnore]
//        Lazy<int> NumDistinctSymbols { get; }
//        [JsonIgnore]
//        Lazy<float> SymbolVariance { get; }
//        [JsonIgnore]
//        Lazy<Dictionary<Symbol, int>> SymbolCounts { get; }
//    }
//}