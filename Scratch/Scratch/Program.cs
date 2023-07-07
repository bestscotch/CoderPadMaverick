using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// I will upload this code to https://github.com/bestscotch/CoderPadMaverick.git
// Wayne Brown.

class Solution
{
    private static readonly string Example1 = "/home/coderpad/data/example1.txt";
    private static readonly string Example2 = "/home/coderpad/data/example2.txt";

    static void Main(string[] args)
    {
        RunTests();

        try
        {
            ProcessFile(Example1);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception processing input {ex.Message}");
        }
    }

    private static void ProcessFile(string filePath)
    {
        Action<string> logResult = (string s) => Console.WriteLine(s);
        using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
        using (BufferedStream bs = new BufferedStream(fs))
        using (StreamReader sr = new StreamReader(bs))
        {
            List<string> currentWordSet = new();
            int currentWordLength = 0;
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (currentWordLength == 0)
                {
                    currentWordLength = line.Length;
                }
                else
                {
                    if (line.Length > currentWordLength)
                    {
                        ProcessWordSet(currentWordSet, logResult);
                        currentWordSet = new List<string>();
                        currentWordLength = line.Length;
                    }
                }

                currentWordSet.Add(line);
            }

            ProcessWordSet(currentWordSet, logResult);
        }
    }

    private static void ProcessWordSet(List<string> wordSet, Action<string> logResult)
    {
        if (!wordSet?.Any() ?? true)
        {
            return;
        }

        List<List<string>> groups = new();

        foreach (var word in wordSet)
        {
            bool groupFound = false; ;

            foreach (var group in groups)
            {
                // First check for existing words
                if (group.Contains(word))
                {
                    groupFound = true;
                    continue;
                }

                if (AreAnagram(word, group.First()))
                {
                    group.Add(word);
                    groupFound = true;
                    continue;
                }
            }

            if (!groupFound)
            {
                groups.Add(new List<string> { word });
            };
        }

        ReportResults(groups, logResult);
    }

    private static void ReportResults(List<List<string>> groups, Action<string> logResult)
    {
        foreach (var group in groups)
        {
            logResult(string.Join(',', group) + Environment.NewLine);
        }
    }

    // Stole this from https://www.c-sharpcorner.com/blogs/determine-two-words-are-anagram
    private static bool AreAnagram(string firstString, string secondString)
    {
        if (firstString.Length != secondString.Length)
        {
            return false;
        }
        //Convert string to character array  
        char[] firstCharsArray = firstString.ToLower().ToCharArray();
        char[] secondCharsArray = secondString.ToLower().ToCharArray();
        //Sort array  
        Array.Sort(firstCharsArray);
        Array.Sort(secondCharsArray);
        //Check each character and position.  
        for (int i = 0; i < firstCharsArray.Length; i++)
        {
            if (firstCharsArray[i].ToString() != secondCharsArray[i].ToString())
            {
                return false;
            }
        }
        return true;
    }

    // Would ideally use NUnit or something similar
    // Given more time I would add tests which generate large sets of random data to check performance.
    private static void RunTests()
    {
        TestWordCount();
        EmptyInputDoesNotThrow();
        NullInputDoesNotThrow();
        AllSameWord();
    }

    // Note the input set actually has 7 words but "fun" is repeated.
    // The task instructions do NOT state that the output should show only distinct words
    // but the Example Output demonstrates that it should.
    private static void TestWordCount()
    {
        var testData = @"abc
fun
bac
fun
cba
unf
hello";

        List<string> result = new();
        ProcessWordSet(testData.Split(Environment.NewLine).ToList(), s => result.Add(s));

        var count = CountCommaSeparatedWords(result);

        if (count != 6)
        {
            // Would use NUnit Assert or Shouldly
            throw new Exception($"Expected word count 6 actual {count}");
        }
    }

    private static void AllSameWord()
    {
        var testData = @"abc
abc
abc
abc
abc
abc
abc";

        List<string> result = new();
        ProcessWordSet(testData.Split(Environment.NewLine).ToList(), s => result.Add(s));

        var count = CountCommaSeparatedWords(result);

        if (count != 1)
        {
            // Would use NUnit Assert or Shouldly
            throw new Exception($"Expected word count 1 actual {count}");
        }
    }


    private static void EmptyInputDoesNotThrow()
    {
        List<string> result = new();
        ProcessWordSet(new List<string>(), s => result.Add(s));
    }

    private static void NullInputDoesNotThrow()
    {
        List<string> result = new();
        ProcessWordSet(null, s => result.Add(s));
    }

    private static int CountCommaSeparatedWords(List<string> input) => input.Select((string line) => line.Split(',').Length).Sum();
}
