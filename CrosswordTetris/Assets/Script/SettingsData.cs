using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SettingsData", menuName = "Config/SettingsData")]
public class SettingsData : ScriptableObject
{
    public Sprite inactive;

    public Sprite active;
    public Color activeFontColor;

    public Sprite displayInactive;

    public Sprite display;
    public Color displayFontColor;

    public Sprite active2X;
    public Color active2XFontColor;

    public Sprite selected;
    public Color selectedFontColor;

    public Sprite golden;
    public Color goldenFontColor;


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