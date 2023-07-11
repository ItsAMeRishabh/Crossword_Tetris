using UnityEngine;
[CreateAssetMenu(fileName = "Level", menuName = "Config/Level")]

public class LevelCreator : ScriptableObject
{
    public string ObjectivePhrase;
    public string[] InitialWord;

    public int Width;
    public int Height;


    public int[] StarRequired;


    public int ChanceOf2X = 15;
    public int ChanceOfRandomCharacters = 70;
}
