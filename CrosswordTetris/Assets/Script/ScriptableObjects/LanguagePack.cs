using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class LetterMeta
{
    public char letter;
    public float value;
    public float weightage;
}

[CreateAssetMenu(fileName = "Language", menuName = "Config/LanguagePack")]
public class LanguagePack : ScriptableObject
{
    public TextAsset MainDictionary;
    public float vowelWeight = 0;
    public float consonantWeight = 0;
    public float totalWeight = 0;
    HashSet<string> Main = new();

    public LetterMeta[] LetterVowelData = new LetterMeta[] {
        new LetterMeta{ letter = 'A', value = 100, weightage = 8 },
        new LetterMeta{ letter = 'E', value = 100, weightage = 9.7f },
        new LetterMeta{ letter = 'I', value = 100, weightage = 9 },
        new LetterMeta{ letter = 'O', value = 100, weightage = 6 },
        new LetterMeta{ letter = 'U', value = 100, weightage = 3 },
    };

    public LetterMeta[] LetterConsonantData = new LetterMeta[] {
        new LetterMeta{ letter = 'B', value = 300, weightage = 2 },
        new LetterMeta{ letter = 'C', value = 300, weightage = 4 },
        new LetterMeta{ letter = 'D', value = 200, weightage = 4 },
        new LetterMeta{ letter = 'F', value = 400, weightage = 1.3f },
        new LetterMeta{ letter = 'G', value = 200, weightage = 3 },
        new LetterMeta{ letter = 'H', value = 400, weightage = 2 },
        new LetterMeta{ letter = 'J', value = 800, weightage = 1 },
        new LetterMeta{ letter = 'K', value = 500, weightage = 1 },
        new LetterMeta{ letter = 'L', value = 100, weightage = 5 },
        new LetterMeta{ letter = 'M', value = 300, weightage = 3 },
        new LetterMeta{ letter = 'N', value = 100, weightage = 7 },
        new LetterMeta{ letter = 'P', value = 300, weightage = 3 },
        new LetterMeta{ letter = 'Q', value =1000, weightage = 1 },
        new LetterMeta{ letter = 'R', value = 100, weightage = 7 },
        new LetterMeta{ letter = 'S', value = 100, weightage = 8 },
        new LetterMeta{ letter = 'T', value = 100, weightage = 6 },
        new LetterMeta{ letter = 'V', value = 400, weightage = 1 },
        new LetterMeta{ letter = 'W', value = 400, weightage = 1 },
        new LetterMeta{ letter = 'X', value = 800, weightage = 1 },
        new LetterMeta{ letter = 'Y', value = 400, weightage = 2 },
        new LetterMeta{ letter = 'Z', value =1000, weightage = 1 },
    };

    private void OnEnable()
    {
        Caliberate();

        Main.Clear();
        //Common.Clear();
        
        LoadDictionary(MainDictionary, ref Main);
        //LoadDictionary(CommonDictionary, ref Common);   
    }

    void LoadDictionary(TextAsset asset, ref HashSet<string> set)
    {
        string[] lines = asset.text.Split('\n');
        foreach (var line in lines)
        {
            string word = line.Trim().ToUpper();
            if (word.Length > 1)
            {
                set.Add(word);
            }
        }
    }

    void Caliberate()
    {
        consonantWeight = 0;
        foreach (var item in LetterConsonantData)
        {
            consonantWeight += item.weightage;
        }

        vowelWeight = 0;
        foreach (var item in LetterVowelData)
        {
            vowelWeight += item.weightage;
        }

        totalWeight = consonantWeight + vowelWeight;
    }
    //readonly char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

    public char GetRandomChar(bool vowel)
    {
        float current = 0;
        if (vowel)
        {
            float random = Random.Range(0, vowelWeight);
            foreach (var item in LetterVowelData)
            {
                current += item.weightage;
                if (current >= random)
                {
                    return item.letter;
                }
            }
            return LetterVowelData[Random.Range(0, LetterVowelData.Length)].letter;
        }
        else
        {
            float random = Random.Range(0, consonantWeight);
            foreach (var item in LetterConsonantData)
            {
                current += item.weightage;
                if (current >= random)
                {
                    return item.letter;
                }
            }
            return LetterConsonantData[Random.Range(0, LetterConsonantData.Length)].letter;

        }
    }
    public bool IsWord(string word)
    {
        return Main.Contains(word);
    }

    //public char[] GetOptimumCharacters(char Target, Dictionary<char, int> frequency, out string bestWord)
    //{
    //    bestWord = null;
    //    if (Target != ' ')
    //    {
    //        int minDifference = int.MaxValue;
    //        foreach (string word in Common)
    //        {
    //            if (word.Contains(Target))
    //            {
    //                frequencycopy = new(frequency);
    //                int difference = CalculateCharacterDifference(word);
    //                if (difference < minDifference && difference != 0)
    //                {
    //                    minDifference = difference;
    //                    bestWord = word;
    //                }
    //            }
    //        }
    //        Debug.Log(bestWord + " needed " + minDifference + " Changes");
    //        frequencycopy = new(frequency);
    //        return CalculateCharacterRequirement(bestWord);
    //    }
    //    return new char[] { GetRandomChar()};
    //}
    //private int CalculateCharacterDifference(string word)
    //{
    //    int difference = 0;
    //    foreach (char c in word)
    //    {
    //        if (frequencycopy.ContainsKey(c))
    //        {
    //            frequencycopy[c]--;
    //            if (frequencycopy[c] < 0)
    //            {
    //                difference++;
    //            }
    //        }
    //        else
    //        {
    //            difference++;
    //        }
    //    }
    //    return difference;
    //}
    //Dictionary<char, int> frequencycopy;
    //private char[] CalculateCharacterRequirement(string word)
    //{
    //    List<char> result = new ();
    //    foreach (char c in word)
    //    {
    //        if (frequencycopy.ContainsKey(c))
    //        {
    //            frequencycopy[c]--;
    //            if (frequencycopy[c] < 0)
    //            {
    //                result.Add(c);
    //            }
    //        }
    //        else
    //        {
    //            result.Add(c);
    //        }
    //    }
    //    return result.ToArray();
    //}

    public static bool IsAlpha(char v)
    {
        return (v >= 'a' && v <= 'z') || (v >= 'A' && v <= 'Z');
    }

}