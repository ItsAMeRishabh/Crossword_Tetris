using System;
using UnityEngine;

public class FlyPather : Poolable
{
    [NonSerialized]
    public Tile target;

    public float Deviation;
    public float Speed = 1;
    public float Accelaration = 1;

    float i;
    float SpeedAcc = 1;
    float durationSeconds = 2;

    Vector3 control;
    Vector3 InitPosition;
    Vector3 targetPos;

    Vector3 p0;
        Vector3 vec;

    public void Init()
    {

        InitPosition = transform.position;
        targetPos = new (target.transform.position.x, target.transform.position.y, 0);

        durationSeconds = (Vector3.Distance(InitPosition, targetPos) );//@ + Random.Range(0, Deviation)) / Speed;

        Vector3 D = (InitPosition - targetPos).normalized;
        control = Vector3.Lerp(transform.position, targetPos, 0.5f) + new Vector3(-D.y, D.x, 0); 

        SpeedAcc = Speed;
        i = 0;
    }

    private void Update() 
    {
        float t = i / durationSeconds;
        p0 = Vector3.Lerp(Vector3.Lerp(InitPosition, control, t), Vector3.Lerp(control, targetPos, t), t);
        vec = (p0 - transform.position).normalized;

        //particle.transform.rotation = Quaternion.AngleAxis((Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg) - 90,Vector3.forward);
        transform.position = p0;

        SpeedAcc += Accelaration * Time.deltaTime * .5f;
        i += Time.deltaTime * SpeedAcc;
        SpeedAcc += Accelaration *Time.deltaTime *.5f;
        
        if(i > durationSeconds)
        {
            target.Activate();
            ReleaseObject();
        }
    }

}
