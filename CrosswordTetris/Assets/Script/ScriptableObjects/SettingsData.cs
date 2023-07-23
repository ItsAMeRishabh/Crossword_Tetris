using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SettingsData", menuName = "Config/SettingsData")]
public class SettingsData : ScriptableObject
{
    [Space(20)]
    public Sprite displayInactive;
    public Sprite display;
    public Color displayFontColor;


    [Space(20)]
    public Sprite inactive;

    
    [Space(20)]
    public Sprite normal;
    public Color normalFontColor;

    public Sprite frozen;
    public Color frozenFontColor;

    public Sprite bubble;
    public Color bubbleFontColor;

    [Space(25)]
    public Sprite fly;
    public Color flyFontColor;


    [Space(25)]
    public Sprite active2X;
    public Color active2XFontColor;


    [Space(25)]
    public Sprite selected;
    public Color selectedFontColor;

    [Space(25)]
    public Sprite golden;
    public Color goldenFontColor;


    public List<Word> LetterPoints = new();
    readonly char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

    public char GetRandomChar()
    {
        return alphabet[Random.Range(0,alphabet.Length-1)];
    }

    public SettingsData()
    {
        for (int i = 0; i < alphabet.Length; i++)
        {
            LetterPoints.Add(new()
            {
                word = alphabet[i] + ""
            });
        }
    }

    public static bool IsAlphaNumeric(char v)
    {
        return (v >= 'a' && v <= 'z') || (v >= 'A' && v <= 'Z') || (v >= '0' && v <= '9');
    }

}