using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public ActiveLevel ActiveLevel;
    public void Open(Level lvl)
    {
        ActiveLevel.Level = lvl;
        UnityEngine.SceneManagement.SceneManager.LoadScene("ILevel");
    }
}
