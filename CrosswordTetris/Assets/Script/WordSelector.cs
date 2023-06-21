using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Word
{
    public string word = "";
    public float value = 0;
}

public class WordSelector : MonoBehaviour
{
    [SerializeField]
    string filePath;
    HashSet<string> dictionary;

    private void Awake()
    {
        dictionary = new HashSet<string>();

        string[] lines = File.ReadAllLines(filePath);

        foreach (string line in lines)
        {
            dictionary.Add(line.Trim());
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
        using StreamReader reader = new(filePath);
        for (int i = 0; i < lineIndex; i++)
        {
            reader.ReadLine();
        }
        return reader.ReadLine();
    }
}
