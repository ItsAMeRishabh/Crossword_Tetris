using System;
using UnityEngine;
[CreateAssetMenu(fileName = "ActiveLevel", menuName = "Config/ActiveLevel")]

public class LevelHandler : ScriptableObject
{
    public int Current;
    public int CompletedLevels;
    public Level[] Levels;
}
