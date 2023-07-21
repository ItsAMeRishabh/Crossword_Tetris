using UnityEngine;

public class GameManager : MonoBehaviour
{
    TileGrid Grid;
    

    void Awake()
    {
        Grid = GetComponent<TileGrid>();
        Grid.GMAwake();
        Grid.WordHandler.GMAwake();
        Grid.PowerUpManager.GMAwake();

    }
    void Start()
    {
        Grid.GMStart();
    }

}
