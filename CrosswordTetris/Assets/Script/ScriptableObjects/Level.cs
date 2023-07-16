using Array2DEditor;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Level", menuName = "Config/Level")]

public class Level : ScriptableObject
{

    public string ObjectiveQuestion;
    public string ObjectivePhrase;

    //public List<string> InitialWord;

    public Array2DString InitalLetter;

    //public int Width;
    //public int Height;


    public int Moves;


    public int ChanceOf2X = 15;
    public int ChanceOfRandomCharacters = 70;
}
