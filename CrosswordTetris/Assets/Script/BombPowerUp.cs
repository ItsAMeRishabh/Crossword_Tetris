using UnityEngine;

public class BombPowerUp : PowerUp
{

    public int Radius = 2;

    const float c = 15.004f;


    private void Update()
    {
        if (IsActive&&CanDo)
        {
            if (Input.GetMouseButtonUp(0))
            {
                int I = -1;
                float Distance = float.MaxValue;
                for (int i = 0; i < transform.childCount; i++)
                {
                    float d = Mathf.Abs(Vector3.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition), transform.GetChild(i).transform.position));
                    if (d < Distance)
                    {
                        Distance = d;
                        I = i;
                    }
                }

                if (I != -1 && Distance < c)
                {
                    IsActive = false;
                    CanDo = false;
                    HexBoom(transform.GetChild(I).transform.position, Radius);
                    var a = transform.GetChild(I).GetComponent<TileLetter>();
                    a.Deselect();
                    a.SetInactive();
                }
            }
        }
    }



}
