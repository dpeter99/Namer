// See https://aka.ms/new-console-template for more information
using System.Linq;

Console.WriteLine("Hello, World!");

var prov = new SimpleCSVProvider(new FileInfo("./Database/Dictionary.txt"));

await prov.Load();

string target = "GLaDOS";


target = target.ToLower();

var filtered = prov.GetWords().AsParallel()
    .Where(w => w.ToLower().ContainsAny(target))
    .Select(w=>w.ToLower())
    .SelectMany((w,h) =>
    {
        var res = new List<Result>();
        for (int i = 0; i < target.Length; i++)
        {
            for (int j = i; j < target.Length; j++)
            {
                var substring = target.Substring(i, j - i + 1);

                int minIndex = w.IndexOf(substring);
                while (minIndex != -1)
                {

                    res.Add(new Result()
                    {
                        word = w,
                        score = 1000-minIndex,
                        targetLetters = new Range(i, j + 1),
                        sourceLetters = new Range(minIndex, minIndex + substring.Length)
                    });

                    minIndex = w.IndexOf(substring, minIndex + substring.Length);
                }
            }
        }

        return res;
    });


var sorted = filtered
    .OrderByDescending(r => r.score)
    .GroupBy(r => r.targetLetters)
    .OrderBy(r=>r.Key.Start.Value)
    .Select(g =>
    {
        return new Substitution()
        {
            target = g.Key,
            options = g.ToList()
        };
    })
    .ToList();


List<Permutation> something = new();

func(0, new List<Substitution>());


List<Substitution> func(int progress, List<Substitution> list)
{
    if (progress == target.Length)
    {
        var targets = new List<Range>();
        foreach (var result in list)
        {
            targets.Add(result.target);
            //Console.Write(target[result.target] + " ");
        }
        
        var a = new Permutation()
        {
            targetRanges = targets,
            substitutions = list
        };
            
        something.Add(a);
        
        //Console.WriteLine();

        return list;
    }
    
    foreach (var part in sorted)
    {
        if (part.target.Start.Value == progress)
        {
            var a = list.Append(part).ToList();
            
            var f = func(part.target.End.Value, a);

            
        }
    }

    return list;
}


foreach (var option in something)
{
    foreach (var range in option.targetRanges)
    {
        Console.Write(target[range] + " ");    
    }
    Console.WriteLine();
    
    foreach (var substitution in option.substitutions)
    {
        Console.WriteLine(target[substitution.target]);
        foreach (var opt in substitution.options)
        {
            Console.Write($"{opt.score} \t {opt.word[..opt.sourceLetters.Start]}");
        
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write($"{opt.word[opt.sourceLetters]}");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        
            Console.Write($"{opt.word[opt.sourceLetters.End..]} \n");    
        }
        
    }
}


/*
foreach(var w in sorted){
    //Console.WriteLine(target.SubstringFromTo(w.Key.Start.Value,w.Key.End.Value));
    Console.WriteLine(target[w.Key]);
    foreach (var word in w)
    {
        Console.Write($"{word.score} \t {word.word[..word.sourceLetters.Start]}");
        
        Console.BackgroundColor = ConsoleColor.Red;
        Console.ForegroundColor = ConsoleColor.Black;
        Console.Write($"{word.word[word.sourceLetters]}");
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.White;
        
        Console.Write($"{word.word[word.sourceLetters.End..]} \n");
        
    }
    
}
*/

class Permutation
{
    public List<Range> targetRanges;
    public List<Substitution> substitutions;
}

class Substitution
{
    public Range target;
    public List<Result> options;
}

struct Result
{
    public Range targetLetters;
    public Range sourceLetters;
    public string word;
    public float score;
}


interface IWordProvider
{
    IEnumerable<string> GetWords();
}

class SimpleCSVProvider : IWordProvider
{
    private readonly FileInfo _csvFile;

    private List<string> words = new ();

    public SimpleCSVProvider(FileInfo csvFile)
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

static class StringExtensions
{
    public static bool ContainsAny(this string str, string chars)
    {
        return str.ContainsAny(chars.ToCharArray());
    }
    
    public static bool ContainsAny(this string str, char[] chars)
    {
        return str.IndexOfAny(chars) != -1;
    }

    public static string SubstringFromTo(this string str, int from, int to)
    {
        return str.Substring(from, to - from + 1);
    }
    
    public static string[] Substrings(this string str)
    {
        var stringList = new List<string>();
        for (int i=0; i <str.Length; i++)
        for (int j=i; j <str.Length; j++)
            stringList.Add(str.Substring(i,j-i+1));

        return stringList.ToArray();
    }
    
}