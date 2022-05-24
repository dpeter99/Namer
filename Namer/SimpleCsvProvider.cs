class SimpleCsvProvider : IWordProvider
{
    private readonly FileInfo _csvFile;

    private List<string> words = new ();

    public SimpleCsvProvider(FileInfo csvFile)
    {
        _csvFile = csvFile;
    }

    public async Task Load()
    {
        StreamReader reader = new StreamReader(_csvFile.FullName);

        while (!reader.EndOfStream)
        {
            var a = await reader.ReadLineAsync();
            if(a is not null)
                words.Add(a);
        }
        
    }
    
    public IEnumerable<string> GetWords()
    {
        return words;
    }
}