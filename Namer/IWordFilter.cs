using Namer;

public interface IWordFilter
{
    bool filter(Word w);
}

class SysnoymFilter : IWordFilter
{
    private List<string> synonyms;


    public SysnoymFilter(List<string> synonyms)
    {
        this.synonyms = synonyms;
    }

    public bool filter(Word w)
    {
        return w.synonyms.Any(syn => synonyms.Any(syn.Contains));
    }
}