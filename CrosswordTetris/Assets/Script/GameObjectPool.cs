using UnityEngine;
using UnityEngine.Pool;

public class GameObjectPool : MonoBehaviour
{
    public Poolable poolablePrefab;

    public int Default;
    public int Size;

    public IObjectPool<GameObject> Pool { get; private set; }

    private void Awake()
    {
        Pool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool,
          OnDestroyPoolObject, true, Default, Size);
    }

    private GameObject CreatePooledItem()
    {
        GameObject obj = Instantiate(poolablePrefab.gameObject);
        obj.GetComponent<Poolable>().Pool = Pool;

        return obj;
    }

    private void OnTakeFromPool(GameObject obj) => obj.SetActive(true);

    private void OnReturnedToPool(GameObject obj) => obj.SetActive(false);

    private void OnDestroyPoolObject(GameObject obj) => Destroy(obj);
}