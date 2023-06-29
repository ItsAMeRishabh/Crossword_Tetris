using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    TileGrid Grid;
    

    void Awake()
    {
        Grid = GetComponent<TileGrid>();
        Grid.GMAwake();
        Grid.WordHandler.GMAwake();

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<TileLetter>().GMAwake();
        }
    }
    void Start()
    {
        Grid.GMStart();
    }

}
