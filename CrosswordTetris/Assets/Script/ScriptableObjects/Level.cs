using Array2DEditor;
using Unity.Collections;
using UnityEngine;
[CreateAssetMenu(fileName = "Level", menuName = "Config/Level")]

public class Level : ScriptableObject
{

    public string ObjectiveQuestion;
    public string ObjectivePhrase;

    [Tooltip("# : Frozen\n@ : Bubble")]
    public bool HoverForCheatsheet;
    public Array2DString InitalLetter;



    public int Moves;


    public int ChanceOf2X = 15;
    public int ChanceOfRandomCharacters = 70;
}
