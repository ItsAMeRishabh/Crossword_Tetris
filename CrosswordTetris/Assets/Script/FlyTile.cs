using UnityEditor.UIElements;
using UnityEngine;

public class FlyTile : Poolable
{
    public Tile target;
    public float CurveSeconds = 2;
    public float i;
    public Vector3 RandomVec;
    public float Speed;
    float R = 10f;
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        float x = Random.Range(-R, R);
        float y = Random.Range(-R, R);
        RandomVec = new(x, y, 0);

        i = CurveSeconds;
    }


    private void Update()
    {
        if (target != null)
        {

            if (i > 0)
                i-= Time.deltaTime;
            else
                i+= 0;


            float t = i / CurveSeconds;
            transform.position = Vector3.MoveTowards(transform.position, RandomVec, Speed * t * Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Speed * R * (1-t) * Time.deltaTime);

            if (Vector3.Distance(transform.position, target.gameObject.transform.position) < 0.1f)
            {
                target.Activate();
                ReleaseObject();
                Init();
            }
        }
    }
}