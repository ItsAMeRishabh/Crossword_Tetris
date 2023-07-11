using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SettingsData", menuName = "Config/SettingsData")]
public class SettingsData : ScriptableObject
{
    [Space(10)]
    public Sprite displayInactive;
    public Sprite display;
    public Color displayFontColor;


    [Space(10)]
    public Sprite inactive;
    public Sprite active;
    public Color activeFontColor;

    [Space(5)]
    public Sprite fly;
    public Color flyFontColor;


    [Space(5)]
    public Sprite active2X;
    public Color active2XFontColor;


    [Space(5)]
    public Sprite selected;
    public Color selectedFontColor;

    [Space(5)]
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