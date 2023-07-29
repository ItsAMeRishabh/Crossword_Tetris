using System;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(fileName = "ActiveLevel", menuName = "Config/ActiveLevel")]

public class LevelHandler : ScriptableObject
{
    public int CompletedLevels { get; private set; }
    public int Coins { get; private set; }
    public int Gems { get; private set; }


    public void SetCoins(int coins)
    {
        Coins = coins;
        PlayerPrefs.SetInt("Coins", coins);
    }

    public void SetGems(int gems)
    {
        Gems = gems;
        PlayerPrefs.SetInt("Gems", gems);
    }

    public void SetCompletedLevels(int completedLevels)
    {
        CompletedLevels = completedLevels;
        PlayerPrefs.SetInt("CompletedLevels", completedLevels);
    }

    void OnEnable()
    {
        CompletedLevels = PlayerPrefs.GetInt("CompletedLevels", 0);
        Coins = PlayerPrefs.GetInt("Coins", 0);
        Gems = PlayerPrefs.GetInt("Gems", 0);
    }

    public int Current;
    public Level[] Levels;
}
