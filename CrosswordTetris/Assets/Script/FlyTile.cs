using UnityEngine;

public class FlyTile : Poolable
{
    public Tile Destination;
    public int time = 50;
    public int i;
    public Vector3 RandomVec;
    public float Speed;
    private void Start()
    {

        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);
        RandomVec = new(x, y, 0);
        RandomVec.Normalize();

        i = time;
    }


    private void FixedUpdate()
    {
        if (Destination != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, Destination.gameObject.transform.position, (1 - ((float)i / time)) * Speed);
            transform.position += (float)i / time * Speed * RandomVec;

            if (i > 0)
            {
                i--;
            }

            if (Vector3.Distance(transform.position, Destination.gameObject.transform.position) < 0.1f)
            {
                Destination.Activate();
                ReleaseObject();
            }
        }

    }
}