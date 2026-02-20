using System.Text.Json.Serialization;
using System.Text.Json;

namespace Bally;

public static class Util
{
    // SymbolMap values are Symbols, describes which symbols are referenced in Baseline and Maxes
    // Returns array indexed by actual symbol values
    public static IEnumerable<int[]> GetDivisions(int[] Baseline, int[] Maxes, int TargetTotal, int[] SymbolMap)
    {
        var count = Baseline.Length;
        var arraySize = (int)Symbol.Seven + 1;
        foreach (var x in GetDivisions(Baseline, Maxes, TargetTotal))
        {
            var ret = new int[arraySize];
            for (int i = 0; i < count; i++)
                ret[SymbolMap[i]] = x[i];
            yield return ret;
        }
    }

    public static IEnumerable<int[]> GetDivisions(int[] Baseline, int[] Maxes, int TargetTotal)
    {
        var numBuckets = Baseline.Length;
        var numToAdd = TargetTotal - Baseline.Sum();

        if (numToAdd < 0)
            throw new Exception("Baseline counts exceed available symbol positions");

        var MaxesToAdd = Enumerable.Range(0, Baseline.Length)
                                   .Select(i => Math.Min(numToAdd, Maxes[i] - Baseline[i]))
                                   .ToArray();

        if (MaxesToAdd.Any(m => m < 0))
            throw new Exception($"{nameof(GetDivisions)}: Maxes must be less than or equal to Baseline.");

        switch (numBuckets)
        {
            case 1:
                yield return [Baseline[0] + numToAdd];
                break;
            case 2:
                {
                    for (int i = Math.Max(0, numToAdd - MaxesToAdd[0]); i <= MaxesToAdd[1]; i++)
                        yield return [Baseline[0] + numToAdd - i, Baseline[1] + i];
                    break;
                }
            case 3:
                {
                    for (int i = 0; i <= MaxesToAdd[2]; i++)
                        for (int j = Math.Max(i, numToAdd - MaxesToAdd[0]); j <= numToAdd && j - i <= MaxesToAdd[1]; j++)
                            yield return [Baseline[0] + numToAdd - j, Baseline[1] + j - i, Baseline[2] + i];
                    break;
                }
            case 4:
                {
                    for (int i = 0; i <= MaxesToAdd[3]; i++)
                        for (int j = i; j <= numToAdd && j - i <= MaxesToAdd[2]; j++)
                            for (int k = Math.Max(j, numToAdd - MaxesToAdd[0]); k <= numToAdd && k - j <= MaxesToAdd[1]; k++)
                                yield return [Baseline[0] + numToAdd - k, Baseline[1] + k - j, Baseline[2] + j - i, Baseline[3] + i];
                    break;
                }
            case 5:
                {
                    for (int i = 0; i <= MaxesToAdd[4]; i++)
                        for (int j = i; j <= numToAdd && j <= i + MaxesToAdd[3]; j++)
                            for (int k = j; k <= numToAdd && k <= j + MaxesToAdd[2]; k++)
                                for (int l = Math.Max(k, numToAdd - MaxesToAdd[0]); l <= numToAdd && l <= k + MaxesToAdd[1]; l++)
                                    yield return [Baseline[0] + numToAdd - l, Baseline[1] + l - k, Baseline[2] + k - j, Baseline[3] + j - i, Baseline[4] + i];
                    break;
                }
            case 6:
                {
                    for (int i = 0; i <= MaxesToAdd[5]; i++)
                        for (int j = i; j <= numToAdd && j <= i + MaxesToAdd[4]; j++)
                            for (int k = i; k <= numToAdd && k <= j + MaxesToAdd[3]; k++)
                                for (int l = k; l <= numToAdd && l <= k + MaxesToAdd[2]; l++)
                                    for (int m = Math.Max(l, numToAdd - MaxesToAdd[0]); m <= numToAdd && m <= l + MaxesToAdd[1]; m++)
                                        yield return [Baseline[0] + numToAdd - m, Baseline[1] + m - l, Baseline[2] + l - k, Baseline[3] + k - j, Baseline[4] + j - i, Baseline[5] + i];
                    break;
                }
            case 7:
                {
                    for (int i = 0; i <= MaxesToAdd[6]; i++)
                        for (int j = i; j <= numToAdd && j <= i + MaxesToAdd[5]; j++)
                            for (int k = j; k <= numToAdd && k <= j + MaxesToAdd[4]; k++)
                                for (int l = k; l <= numToAdd && l <= k + MaxesToAdd[3]; l++)
                                    for (int m = l; m <= numToAdd && m <= l + MaxesToAdd[2]; m++)
                                        for (int n = Math.Max(m, numToAdd - MaxesToAdd[0]); n <= numToAdd && n <= m + MaxesToAdd[1]; n++)
                                            yield return [Baseline[0] + numToAdd - n, Baseline[1] + n - m, Baseline[2] + m - l, Baseline[3] + l - k, Baseline[4] + k - j, Baseline[5] + j - i, Baseline[6] + i];
                    break;
                }
            case 8:
                {
                    for (int i = 0; i <= MaxesToAdd[7]; i++)
                        for (int j = i; j <= numToAdd && j <= i + MaxesToAdd[6]; j++)
                            for (int k = j; k <= numToAdd && k <= j + MaxesToAdd[5]; k++)
                                for (int l = k; l <= numToAdd && l <= k + MaxesToAdd[4]; l++)
                                    for (int m = l; m <= numToAdd && m <= l + MaxesToAdd[3]; m++)
                                        for (int n = m; n <= numToAdd && n <= m + MaxesToAdd[2]; n++)
                                            for (int o = Math.Max(n, numToAdd - MaxesToAdd[0]); o <= numToAdd && o <= n + MaxesToAdd[1]; o++)
                                                yield return [Baseline[0] + numToAdd - o, Baseline[1] + o - n, Baseline[2] + n - m, Baseline[3] + m - l, Baseline[4] + l - k, Baseline[5] + k - j, Baseline[6] + j - i, Baseline[7] + i];
                    break;
                }
            case 9:
                {
                    for (int i = 0; i <= MaxesToAdd[8]; i++)
                        for (int j = i; j <= numToAdd && j <= i + MaxesToAdd[7]; j++)
                            for (int k = j; k <= numToAdd && k <= j + MaxesToAdd[6]; k++)
                                for (int l = k; l <= numToAdd && l <= k + MaxesToAdd[5]; l++)
                                    for (int m = l; m <= numToAdd && m <= l + MaxesToAdd[4]; m++)
                                        for (int n = m; n <= numToAdd && n <= m + MaxesToAdd[3]; n++)
                                            for (int o = n; o <= numToAdd && o <= n + MaxesToAdd[2]; o++)
                                                for (int p = Math.Max(o, numToAdd - MaxesToAdd[0]); p <= numToAdd && p <= o + MaxesToAdd[1]; p++)
                                                    yield return [Baseline[0] + numToAdd - p, Baseline[1] + p - o, Baseline[2] + o - n, Baseline[3] + n - m, Baseline[4] + m - l, Baseline[5] + l - k, Baseline[6] + k - j, Baseline[7] + j - i, Baseline[8] + i];
                    break;
                }
            case 10:
                {
                    for (int i = 0; i <= MaxesToAdd[9]; i++)
                        for (int j = i; j <= numToAdd && j <= i + MaxesToAdd[8]; j++)
                            for (int k = j; k <= numToAdd && k <= j + MaxesToAdd[7]; k++)
                                for (int l = k; l <= numToAdd && l <= k + MaxesToAdd[6]; l++)
                                    for (int m = l; m <= numToAdd && m <= l + MaxesToAdd[5]; m++)
                                        for (int n = m; n <= numToAdd && n <= m + MaxesToAdd[4]; n++)
                                            for (int o = n; o <= numToAdd && o <= n + MaxesToAdd[3]; o++)
                                                for (int p = o; p <= numToAdd && p <= o + MaxesToAdd[2]; p++)
                                                    for (int q = Math.Max(p, numToAdd - MaxesToAdd[0]); q <= numToAdd && q <= p + MaxesToAdd[1]; q++)
                                                        yield return [Baseline[0] + numToAdd - q, Baseline[1] + q - p, Baseline[2] + p - o, Baseline[3] + o - n, Baseline[4] + n - m, Baseline[5] + m - l, Baseline[6] + l - k, Baseline[7] + k - j, Baseline[8] + j - i, Baseline[9] + i];
                    break;
                }
            case 11:
                {
                    for (int i = 0; i <= MaxesToAdd[10]; i++)
                        for (int j = i; j <= numToAdd && j <= i + MaxesToAdd[9]; j++)
                            for (int k = j; k <= numToAdd && k <= j + MaxesToAdd[8]; k++)
                                for (int l = k; l <= numToAdd && l <= k + MaxesToAdd[7]; l++)
                                    for (int m = l; m <= numToAdd && m <= l + MaxesToAdd[6]; m++)
                                        for (int n = m; n <= numToAdd && n <= m + MaxesToAdd[5]; n++)
                                            for (int o = n; o <= numToAdd && o <= n + MaxesToAdd[4]; o++)
                                                for (int p = o; p <= numToAdd && p <= o + MaxesToAdd[3]; p++)
                                                    for (int q = p; q <= numToAdd && q <= p + MaxesToAdd[2]; q++)
                                                        for (int r = Math.Max(q, numToAdd - MaxesToAdd[0]); r <= numToAdd && r <= q + MaxesToAdd[1]; r++)
                                                            yield return [Baseline[0] + numToAdd - r, Baseline[1] + r - q, Baseline[2] + q - p, Baseline[3] + p - o, Baseline[4] + o - n, Baseline[5] + n - m, Baseline[6] + m - l, Baseline[7] + l - k, Baseline[8] + k - j, Baseline[9] + j - i, Baseline[10] + i];
                    break;
                }
            default:
                throw new NotImplementedException();
        }
    }
    public static int[] SplitStringToNumbers(string Input, int Offset)
    {
        return Input.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                    .Select(i => int.Parse(i) + Offset)
                    .ToArray();
    }
 
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> Source)
    {
        ArgumentNullException.ThrowIfNull(Source);
        Random r = new();
        var buf = Source.ToList();
        var num = buf.Count;
        for (int i = 0; i < num; i++)
        {
            int j = r.Next(i, num);
            yield return buf[j];
            buf[j] = buf[i];
        }
    }

    public static void SerializeToFile<T>(string Path, T Object) => WriteText(Path, Serialize(Object));
    public static string Serialize<T>(T Object) => JsonSerializer.Serialize(Object, JsonOptions);
    public static T DeserializeFromFile<T>(string Path) => Deserialize<T>(File.ReadAllText(Path));
    public static T Deserialize<T>(string Json) => JsonSerializer.Deserialize<T>(Json, JsonOptions) ?? throw new Exception("Failed to deserialize json!");
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };
    public static void WriteText(string Path, string Text)
    {
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Path) ?? throw new DirectoryNotFoundException(Path));
        File.WriteAllText(Path, Text);
    }
    //public static int[] ExpandSymbolCounts(int[] Counts, Symbol[] Map)
    //{
    //    var ret = new int[(int)Map.Max() + 1];
    //    for (int i = 0; i < Counts.Length; i++)
    //        ret[(int)Map[i]] = Counts[i];
    //    return ret;
    //}
    public static Stream ToStream(this string Str)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(Str);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }
}
