using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class LetterMeta
{
    public char letter;
    public float value ;
    public float weightage;
    [NonSerialized]
    public float chance;
}

[CreateAssetMenu(fileName = "SettingsData", menuName = "Config/SettingsData")]
public class SettingsData : ScriptableObject
{
    [Space(20)]
    public Sprite displayInactive;
    public Sprite display;
    public Color displayFontColor;


    [Space(20)]
    public Sprite inactive;
    public Sprite normal;
    public Sprite frozen;
    public Sprite bubble;
    public Sprite fly;
    public Sprite active2X;
    public Sprite selected;
    public Sprite golden;
    public Sprite debris;


    [Space(20)]

    public Sprite gem;
    public Sprite coin;

    [Space(20)]
    public Color normalFontColor;
    public Color frozenFontColor;
    public Color bubbleFontColor;
    public Color flyFontColor;
    public Color active2XFontColor;
    public Color selectedFontColor;
    public Color goldenFontColor;

    [Space(20)]

    public float scaler = 2f;
    public List<LetterMeta> LetterMetaData = new();

    public void Caliberate()
    {
        totalWeight = 0;
        float high = 0;
        foreach (var item in LetterMetaData)
        {
            if (item.weightage > high)
                high = item.weightage;
        }
        foreach (var item in LetterMetaData)
        { 
            item.chance = (item.weightage/scaler) + (high - (high/scaler));
            totalWeight += item.chance;
        }
    }
    //readonly char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

    public float totalWeight = 0;
    public char GetRandomChar()
    {

        float random = Random.Range(0, totalWeight);
        float currentWeight = 0;
        foreach (var item in LetterMetaData)
        {
            currentWeight += item.chance;
            if (currentWeight >= random)
            {
                return item.letter;
            }
        }

        return LetterMetaData[Random.Range(0, LetterMetaData.Count - 1)].letter;
    }


    public static bool IsAlphaNumeric(char v)
    {
        return (v >= 'a' && v <= 'z') || (v >= 'A' && v <= 'Z') || (v >= '0' && v <= '9');
    }

}