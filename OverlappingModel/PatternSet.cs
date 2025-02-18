namespace OverlappingModel;

internal class PatternSet
{
    private List<Pattern> patterns;
    public PatternSet()
    {
        patterns = new();
    }

    public int TryAddPattern(Pattern pattern)
    {
        var index = patterns.FindIndex(p => p.Equals(pattern));
        if (index != -1)
        {
            patterns[index].Frequency++;
            return index;
        }
        patterns.Add(pattern);
        pattern.Id = patterns.Count - 1;
        foreach (var p in patterns)
        {
            p.MatchPattern(pattern);
            pattern.MatchPattern(p);
        }
        return pattern.Id;
    }

    public Pattern[] GetPatterns()
    {
        return patterns.ToArray();
    }

    public int[] GetFrequencies()
    {
        return patterns.Select(p => p.Frequency).ToArray();
    }
}
