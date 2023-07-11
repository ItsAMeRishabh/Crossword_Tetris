using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Level", menuName = "Config/Level")]

public class Level : ScriptableObject
{
    public SettingsData Settings;

    public string ObjectiveQuestion;
    public string ObjectivePhrase;

    public List<string> InitialWord;

    public int Width;
    public int Height;


    public int[] StarRequired;


    public int ChanceOf2X = 15;
    public int ChanceOfRandomCharacters = 70;
}
