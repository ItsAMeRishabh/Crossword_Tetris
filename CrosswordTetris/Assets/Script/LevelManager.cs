using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public ActiveLevel ActiveLevel;
    public GameObject Levels;

    public Sprite Current;
    public Sprite Completed;
    public Sprite Locked;

    private void Start()
    {

        for (int i = 0; i < Levels.transform.childCount; i++)
        {
            
            int levelIndex = i; // Create a local copy of 'i'
            Transform child = Levels.transform.GetChild(i);

            if (i == ActiveLevel.CompletedLevels)
                child.GetComponent<UnityEngine.UI.Image>().sprite = Current;
            else if (i < ActiveLevel.CompletedLevels)
                child.GetComponent<UnityEngine.UI.Image>().sprite = Completed;
            else
            {
                child.GetComponent<UnityEngine.UI.Image>().sprite = Locked;
                continue;
            }
            child.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => Open(levelIndex));
            child.GetComponentInChildren<TextMeshProUGUI>().text = (1 + levelIndex).ToString();
        }
    }

    public void Open(int lvl)
    {
        ActiveLevel.Current = lvl;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Levels");
    }
}
