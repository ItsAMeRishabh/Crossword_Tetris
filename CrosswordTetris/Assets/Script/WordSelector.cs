using UnityEngine.Pool;
using UnityEngine;
using Random = UnityEngine.Random;
using System;
using System.Collections.Generic;
using System.IO;

[Serializable]
public class Word
{
    public string word = "";
    public float value = 0;
}

public class WordSelector : MonoBehaviour
{
    public TextAsset txt;
    HashSet<string> dictionary;

    public void GMAwake()
    {

        dictionary = HashSetPool<string>.Get();

        //TextAsset textAsset = Resources.Load<TextAsset>("Dictionary79.3k");
        string fileContent = txt.text;
        string[] lines = fileContent.Split('\n');

        foreach (string line in lines)
        {
            dictionary.Add(line.Trim().ToUpper());
        }
    }

    public bool IsWord(string word)
    {
        return dictionary.Contains(word);
    }

}
