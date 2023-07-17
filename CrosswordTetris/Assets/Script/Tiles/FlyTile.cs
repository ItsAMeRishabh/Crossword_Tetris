using UnityEngine;

public class FlyTile : Poolable
{
    public Tile target;
    public Transform particle;
    
    Vector3 control;
    float durationSeconds = 2;

    public float Deviation;
    public float Speed = 1;
    public float Accelaration = 1;

    float SpeedAcc = 1;
    float i;
    Vector3 InitPosition;
    Vector3 targetPos;

    
    public void Init()
    {

        InitPosition = transform.position;
        targetPos = new (target.transform.position.x, target.transform.position.y, 0);

        durationSeconds = (Vector3.Distance(InitPosition, targetPos) );//@ + Random.Range(0, Deviation)) / Speed;

        Vector3 D = (InitPosition - targetPos).normalized;
        Vector3 devi = new Vector3(-D.y,D.x,0);//@ * Random.Range(-Deviation,Deviation);
        control = Vector3.Lerp(transform.position, targetPos, 0.5f) + devi;

        SpeedAcc = Speed;
        i = 0;
    }

    private void Update() 
    {
        float t = i / durationSeconds;
        Vector3 p0 = Vector3.Lerp(Vector3.Lerp(InitPosition, control, t), Vector3.Lerp(control, targetPos, t), t);
        Vector3 vec = (p0 - transform.position).normalized;

        float angle = Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;
        particle.transform.rotation = Quaternion.AngleAxis(angle - 90,Vector3.forward);

        transform.position = p0;

        i += Time.deltaTime * SpeedAcc;
        SpeedAcc += Accelaration;
        if(i > durationSeconds)
        {
            target.Activate();
            ReleaseObject();
        }
    }

}
