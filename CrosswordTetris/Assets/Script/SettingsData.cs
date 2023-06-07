using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SettingsData", menuName = "Config/SettingsData")]
public class SettingsData : ScriptableObject
{
    public List<Word> LetterPoints = new();
    char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

    public SettingsData()
    {
        for (int i = 0; i < alphabet.Length; i++)
        {
            Word w = new();
            w.points = 1;
            w.word = alphabet[i]+"";
            LetterPoints.Add(w);
        }
    }
}