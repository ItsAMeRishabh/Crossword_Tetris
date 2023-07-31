using UnityEngine;

public class Cloud : MonoBehaviour
{
    public SpriteRenderer sr;

    float Scale;
    public Vector2 ScaleDev;

    float Speed;
    public Vector2 SpeedDev;

    public Vector3 Min;
    public Vector3 Max;


    private void Update()
    {
        transform.position += Speed * Time.deltaTime * Vector3.right;

        if (transform.position.x > Max.x || transform.position.x < Min.x)
        {
            Debug.Log("Set");
                Set();
        }
    }

    public void Set()
    {
        bool b = Random.Range(0, 2) == 0;
        transform.position = new Vector3(b ? Min.x : Max.x, Random.Range(Min.y, Max.y), 0);

        Scale = Random.Range(ScaleDev.x, ScaleDev.y);
        Speed = Random.Range(SpeedDev.x, SpeedDev.y) * (b?1:-1) ;

        transform.localScale = Vector3.one * Scale;

        
    }
}
