using Namer;
using WordNetSharp;

class WordNetProvider : IWordProvider
{
    WordNetEngine engine;
    
    public WordNetProvider()
    {
        
        engine = new WordNetEngine("./wn3.1/dict", true);
    }
    
    public IEnumerable<Word> GetWords()
    {
        //engine.GetSynSets()
        
        return engine.AllWords.AsParallel()
            .SelectMany(pos => pos.Value.ToList())
            .Select(s =>
            {
                return new Word()
                {
                    word = s.Replace('_', ' '),
                    synonyms = engine.GetSynSets(s).Select(sy => sy.Gloss).ToList()
                };
            });
    }
}