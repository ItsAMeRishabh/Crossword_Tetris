using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WordSelector : MonoBehaviour
{
    [SerializeField]
    string filePath;
    private HashSet<string> dictionary;

    private void Start()
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
