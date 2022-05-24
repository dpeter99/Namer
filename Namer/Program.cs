// See https://aka.ms/new-console-template for more information
using System.Linq;
using System.Text;
using Alba.CsConsoleFormat;
using Alba.CsConsoleFormat.Fluent;

public class Program{

    static LineThickness headerThickness = new LineThickness(LineWidth.Double, LineWidth.Single);
    
    public static async Task Main()
    {
        

        //Load the dictionary
        var prov = new SimpleCsvProvider(new FileInfo("./Database/Dictionary.txt"));
        await prov.Load();
     
        string target = "GLaDOS";
        target = target.ToLower();
        Console.WriteLine($"Target: {target}");

        var substitutions = getSubstitutions(prov, target);

        var perms = getPermutations(substitutions, target);

        
        
        foreach (var perm in perms)
        {
            Display(perm);
        }
    }

    public static List<Substitution> getSubstitutions(IWordProvider prov, string target)
    {
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
                                score = 1 - (minIndex / (float)w.Length),
                                targetLetters = new Range(i, j + 1),
                                sourceLetters = new Range(minIndex, minIndex + substring.Length)
                            });

                            minIndex = w.IndexOf(substring, minIndex + substring.Length);
                        }
                    }
                }

                return res;
            })
            .Where(r => !(r.word.Length <= 4 && r.sourceLetters.Start.Value > 0))
            .Where(r=> r.score > 0.5);


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
        
        
        
        return sorted; 
    }

    static List<Permutation> getPermutations(List<Substitution> substitutions,string target)
    {
        List<Permutation> perms = new();

        Stack<Substitution> stack = new();
        foreach (var root in substitutions.Where(s => s.target.Start.Value == 0))
        {
            traverse(root);
        }

        void traverse(Substitution current)
        {
            stack.Push(current);
            
            if (current.target.End.Value == target.Length)
            {
                var a = new Permutation()
                {
                    //targetRanges = targets,
                    substitutions = stack.Reverse().ToList()
                };
                perms.Add(a);
                    
            }
            
            var neighbours = substitutions
                .Where(s => s.target.Start.Value == current.target.End.Value);

            foreach (var neighbour in neighbours)
            {
                traverse(neighbour);
            }

            stack.Pop();
        }

        return perms;
    }
    
    static void Display(Permutation perm)
    {
        var height = perm.substitutions.Max(s => s.options.Count);
    
        Element[,] display = new Element[perm.substitutions.Count, height];

        for (int i = 0; i < perm.substitutions.Count; i++)
        {
            var sub = perm.substitutions[i];
        
            for (int j = 0; j < sub.options.Count; j++)
            {
                var opt = sub.options[j];

                Div collection = new(null);

                collection.Children.Add( new Span($"{opt.score:P1} \t {opt.word[..opt.sourceLetters.Start]}"));
            
                collection.Children.Add($"{opt.word[opt.sourceLetters]}".Red());

                collection.Children.Add( new Span($"{opt.word[opt.sourceLetters.End..]} \n"));

                display[i, j] = collection;

            }
        }

        List<List<Element>> a = new();
        for (int y = 0; y < display.GetLength(1); y++)
        {
            List<Element> b = new();
            for (int x = 0; x < display.GetLength(0); x++)
            {
                b.Add(display[x,y]);
            }
            a.Add(b);
        }
    
        var doc = new Document(
            new Span("Order #") { Color = ConsoleColor.Yellow }, "asd", "\n",
            new Span("Customer: ") { Color = ConsoleColor.Yellow }, "asd",
            new Grid {
                Color = ConsoleColor.Gray,
                Columns = {Enumerable.Range(0,perm.substitutions.Count).Select(i=>GridLength.Star(1))},
                Children = {
                    a.Select(item => item.Select(i => new Cell(i)))
                }
            }
        );

        
        ConsoleRenderer.RenderDocument(doc);
    }
}











/*
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
    
    Display(option);
}
*/
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