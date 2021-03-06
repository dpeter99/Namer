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
    
    public static bool IsAllUpper(this string input)
    {
        for (int i = 0; i < input.Length; i++)
        {
            if (Char.IsLetter(input[i]) && !Char.IsUpper(input[i]))
                return false;
        }
        return true;
    }
    
    public static bool IsAllLower(this string input)
    {
        for (int i = 0; i < input.Length; i++)
        {
            if (Char.IsLetter(input[i]) && !Char.IsLower(input[i]))
                return false;
        }
        return true;
    }
    
}