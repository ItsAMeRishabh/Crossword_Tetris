using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Word
{
    public string word = "";
    public int points = 0;
}

public class WordSelector : MonoBehaviour
{
    [SerializeField]
    string filePath;
    HashSet<string> dictionary;

    public List<Word> ObjectiveWords;
    public List<Word> ActiveWords;


    private void Start()
    {
        dictionary = new HashSet<string>();

        string[] lines = File.ReadAllLines(filePath);

        foreach (string line in lines)
        {
            dictionary.Add(line.Trim());
        }


        foreach (var item in ObjectiveWords)
        {
            item.word = item.word.ToUpper();
        }
    }

    public bool IsWord(string word)
    {
        return dictionary.Contains(word);
    }

    public string GetRandomWord()
    {
        return ReadLineFromFile(filePath, Random.Range(0, dictionary.Count)).Trim();
    }


    private string ReadLineFromFile(string filePath, int lineIndex)
    {
        using (StreamReader reader = new StreamReader(filePath))
        {
            for (int i = 0; i < lineIndex; i++)
            {
                reader.ReadLine();
            }
            return reader.ReadLine();
        }
    }
}
