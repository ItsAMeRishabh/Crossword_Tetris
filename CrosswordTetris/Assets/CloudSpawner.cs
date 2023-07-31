using System.Collections;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    public int Size;
    public GameObject CloudPrefab;
    public Camera mainCamera;



    private void Start()
    {
        StartCoroutine(Spawn());
    }
    IEnumerator Spawn()
    {
        Vector3 offset = new Vector3(3, 0, 0);
        Vector3 max = mainCamera.ViewportToWorldPoint(new Vector3(1f, 1f, mainCamera.nearClipPlane)) + offset;
        Vector3 min = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0f, mainCamera.nearClipPlane)) - offset;

        Debug.Log(min);
        Debug.Log(max);

        for (int i = 0; i < Size; i++)
        {
            Cloud obj = Instantiate(CloudPrefab, transform).GetComponent<Cloud>();
            obj.Min = min;
            obj.Max = max;
            obj.Set();
            //if (i % 2 == 0)
                yield return new WaitForSeconds(Random.Range(0,2.5f));
        }

    }
}
