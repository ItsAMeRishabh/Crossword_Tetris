using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    TileGrid Grid;
    

    void Awake()
    {
        Grid = GetComponent<TileGrid>();
        Grid.GMAwake();
        Grid.wordHandler.GMAwake();
        FindObjectOfType<UIManager>().DebugText.text = "Yo";

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<LetterTile>().GMAwake();
        }
    }
    void Start()
    {
        Grid.GMStart();
    }

}
