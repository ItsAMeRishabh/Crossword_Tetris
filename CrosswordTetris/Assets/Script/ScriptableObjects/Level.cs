using Array2DEditor;
using UnityEngine;
[CreateAssetMenu(fileName = "Level", menuName = "Config/Level")]

public class Level : ScriptableObject
{

    public string ObjectiveQuestion;
    public string ObjectivePhrase;

    [Tooltip("# : Frozen\n@ : Bubble\n% : Debris\n$ : Coins\n^ : Gems")]
    public bool HoverForCheatsheet;

    public Array2DString InitalLetter;


    public int[] StarsThreshHolds = new int[] { 500, 900, 1200 };
    public int Moves;


    public float ChanceOfCoin = 15;
    public float ChanceOfGem = 1;


    //public float CompletionForSmart = 70;
    //public float RequiredNoReveals = 2;
    public float ChanceForRandom = 75;

    public float InitialSuggestTime = 7;
    public float LoopSuggestTime = 4;


}
