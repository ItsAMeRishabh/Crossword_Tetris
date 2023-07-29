using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinScreenManager : MonoBehaviour
{

    public LevelHandler lvl;

    public TextMeshProUGUI Phrase;
    public TextMeshProUGUI Points;
    public TextMeshProUGUI Coins;
    public TextMeshProUGUI Gems;

    int gems = 0;
    int coins = 0;

    public Image Star1;
    public Image Star2;
    public Image Star3;

    public Color Active = Color.white;
    public Color InActive = Color.black;

    public void Claim()
    {
        lvl.SetGems(lvl.Gems+gems);
        lvl.SetCoins(lvl.Coins + coins);
        UnityEngine.SceneManagement.SceneManager.LoadScene("LevelSelector");
    }

    public void Win(string phrase, int stars, int points, int coins, int gems)
    {
        this.gems = gems;
        this.coins = coins;

        gameObject.SetActive(true);
        Phrase.text = phrase;
        Points.text = points.ToString();
        Coins.text = coins.ToString();
        Gems.text = gems.ToString();

        switch (stars)
        {
            case 0:
                Star1.color = InActive;
                Star2.color = InActive;
                Star3.color = InActive;
                break;
            case 1:
                Star1.color = Active;
                Star2.color = InActive;
                Star3.color = InActive;
                break;
            case 2:
                Star1.color = Active;
                Star2.color = Active;
                Star3.color = InActive;
                break;
            case 3:
                Star1.color = Active;
                Star2.color = Active;
                Star3.color = Active;
                break;
        }
    }
}
