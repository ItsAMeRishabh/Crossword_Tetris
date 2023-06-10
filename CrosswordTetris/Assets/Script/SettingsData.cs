using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SettingsData", menuName = "Config/SettingsData")]
public class SettingsData : ScriptableObject
{
    public Color inactiveColor;
    public Color activeColor;
    public Color activeFontColor;

    public Color selectedColor;
    public Color selectedFontColor;


    public List<Word> LetterPoints = new();
    readonly char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

    public char[] GetAlphabet()
    {
        return alphabet;
    }

    public SettingsData()
    {
        for (int i = 0; i < alphabet.Length; i++)
        {
            Word w = new()
            {
                word = alphabet[i] + ""
            };
            LetterPoints.Add(w);
        }
    }
}