using UnityEngine;

public class FlyTile : Poolable
{
    public Tile Destination;
    public float CurveSeconds = 50;
    public float WaitSeconds = 0;
    float i;
    public Vector3 RandomVec;
    public float Speed;
    private void Start()
    {

        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);
        RandomVec = new(x, y, 0);
        RandomVec.Normalize();

        i = CurveSeconds;
    }


    private void Update()
    {
        if (WaitSeconds < 0)
        {
            if (Destination != null)
            {
                transform.position = Vector3.MoveTowards(transform.position, Destination.gameObject.transform.position, (1 - ((float)i / CurveSeconds)) * Speed * Time.deltaTime);
                transform.position += i / CurveSeconds * Speed * RandomVec * Time.deltaTime;

                if (i > 0)
                {
                    i -= Time.deltaTime;
                }

                if (Vector3.Distance(transform.position, Destination.gameObject.transform.position) < 0.1f)
                {
                    Destination.Activate();
                    ReleaseObject();
                }
            }
        }
               else
        {
            WaitSeconds -= Time.deltaTime;
        }   
    }
}