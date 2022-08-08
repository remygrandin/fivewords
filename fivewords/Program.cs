using System.Diagnostics;
using fivewords;

Stopwatch sw = Stopwatch.StartNew();

IEnumerable<string> words = File.ReadAllLines("wordleWords.txt");
//IEnumerable<string> words = File.ReadAllLines("words_alpha.txt");
Console.WriteLine($"Precleanup word count : {words.Count()}");
words = words.Where(w => w.Length == 5);
Console.WriteLine($"length 5 count : {words.Count()}");

List<Tuple<string, Int32>> words5 = new List<Tuple<string, Int32>>();

foreach (string word in words)
{
    Int32 wordFP = 0;
    bool dupplicateFound = false;
    foreach (char c in word)
    {
        byte charPos = (byte)(c - 61);
        var bit = (wordFP & (1 << charPos - 1)) != 0;

        if (bit)
        {
            //Console.WriteLine($"{word} has a duplicate letter");
            dupplicateFound = true;
            break;

        }
        wordFP = wordFP | (1 << charPos - 1);
    }

    if (!dupplicateFound)
    {
        words5.Add(new Tuple<string, int>(word, wordFP));
    }
}

int words5Count = words5.Count();

int i = 0;

Console.WriteLine($"length 5 dedup count : {words5Count}");

List<Result> results = new List<Result>();

//for (int a = 0; a < words5Count; a++)
Parallel.For(0, words5Count, a =>
{
    var itemA = words5[a];

    List<Tuple<string, Int32>> validBWords = new List<Tuple<string, Int32>>(words5Count);

    Console.WriteLine($"{i++}");

    Int32 associative = itemA.Item2;

    for (int b = a + 1; b < words5Count; b++)
    {
        var itemB = words5[b];
        if ((associative & itemB.Item2) == 0)
        {
            validBWords.Add(itemB);
        }
    }

    var validBWordsCount = validBWords.Count;

    foreach (var validBWord in validBWords)
    {
        Int32 associativeB = associative | validBWord.Item2;

        List<Tuple<string, Int32>> validCWords = new List<Tuple<string, Int32>>(validBWordsCount);

        foreach (var cWord in validBWords)
        {
            if ((associativeB & cWord.Item2) == 0)
            {
                validCWords.Add(cWord);
            }
        }

        var validCWordsCount = validCWords.Count;

        foreach (var validCWord in validCWords)
        {
            Int32 associativeC = associativeB | validCWord.Item2;

            List<Tuple<string, Int32>> validDWords = new List<Tuple<string, Int32>>(validCWordsCount);

            foreach (var dWord in validCWords)
            {
                if ((associativeC & dWord.Item2) == 0)
                {
                    validDWords.Add(dWord);
                }
            }

            var validDWordsCount = validDWords.Count;


            foreach (var validDWord in validDWords)
            {
                Int32 associativeD = associativeC | validDWord.Item2;

                foreach (var eWord in validDWords)
                {
                    if ((associativeD & eWord.Item2) == 0)
                    {
                        //Console.WriteLine($"Found {itemA.Item2}/{validBWord}/{validCWord}/{validDWord}/{eWord}");
                        //Console.WriteLine($"Found {itemA.Item1}/{words5.First(item => item.Item2 == validBWord).Item1}/{words5.First(item => item.Item2 == validCWord).Item1}/{words5.First(item => item.Item2 == validDWord).Item1}/{words5.First(item => item.Item2 == eWord).Item1}");
                        Result res = new Result();

                        res.SetWords(itemA, validBWord, validCWord, validDWord, eWord);
                        results.Add(res);
                    }
                }
            }

        }

    }

});

Console.WriteLine($"Found : {results.Count}");

results = results.GroupBy(p => p.ToString()).Select(g => g.First()).ToList();


Console.WriteLine($"Found after dedup : {results.Count}");

foreach (Result res in results)
{
    Console.WriteLine(res);
}

sw.Stop();

Console.WriteLine($"Took {sw.ElapsedMilliseconds}ms");



