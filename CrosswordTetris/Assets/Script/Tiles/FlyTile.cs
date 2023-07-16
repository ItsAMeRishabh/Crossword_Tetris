using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlyTile : Poolable
{
    public Tile target;
    public Vector3 control;
    public float durationSeconds = 2;
    public float deviation;
    public float speed = .1f;
    float i;
    Vector3 InitPosition;
    Vector3 pA;
    Vector3 pB;
    private void Start()
    {
        Init();
    }
    
    public void Init()
    {

        InitPosition = transform.position;
        Vector3 dev = new Vector3(Random.Range(-deviation, deviation), Random.Range(-deviation, deviation), 0);
        control = Vector3.Lerp(transform.position, target.transform.position, 0.5f) + dev;

        i = 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, control);
        Gizmos.DrawLine(control, target.transform.position);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(pA,pB);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(pA, .1f);
        Gizmos.DrawSphere(pB, .1f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(target.transform.position, .1f);


    }

    private void Update() 
    {
        float t = i / durationSeconds;
        Debug.Log(t);

        pA = Vector3.Lerp(InitPosition, control, t);

        pB = Vector3.Lerp(control, target.transform.position,t);

        Vector3 p = Vector3.Lerp(pA, pB, t);


        transform.position = p;

        i += Time.deltaTime * speed;
        if(i > durationSeconds)
        //if (Vector3.Distance(transform.position, target.gameObject.transform.position) < 0.1f)
        {
            Init();
            target.Activate();
            ReleaseObject();
        }
    }

}






















class d: Poolable
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