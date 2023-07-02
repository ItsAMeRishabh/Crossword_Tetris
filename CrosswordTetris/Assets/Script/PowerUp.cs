using System.Collections;
using UnityEngine;

public abstract class PowerUp : MonoBehaviour
{
    public bool CanDo = false;
    public bool IsActive = true;
    public void Toggle()
    {
        IsActive = !IsActive;
    }
    public static void HexBoom(Vector2 pos, int rad)
    {
        DeathBoomRay(pos, Vector2.up, rad);
        DeathBoomRay(pos, new Vector2(Mathf.Sqrt(3 / 4f), 0.5f), rad);
        DeathBoomRay(pos, new Vector2(Mathf.Sqrt(3 / 4f), -0.5f), rad);
        DeathBoomRay(pos, -Vector2.up, rad);
        DeathBoomRay(pos, -new Vector2(Mathf.Sqrt(3 / 4f), 0.5f), rad);
        DeathBoomRay(pos, -new Vector2(Mathf.Sqrt(3 / 4f), -0.5f), rad);
    }
    public static void RectBoom(Vector2 pos, int rad)
    {
        DeathBoomRay(pos, Vector2.up, rad);
        DeathBoomRay(pos, Vector2.right, rad);
        DeathBoomRay(pos, -Vector2.up, rad);
        DeathBoomRay(pos, -Vector2.right, rad);
    }

    public static IEnumerator RectBoomCour(Vector3 pos, int rad)
    {
        float delay = .5f;
        yield return new WaitForSeconds(delay);
        DeathBoomRay(pos, Vector2.up, rad);
        yield return new WaitForSeconds(delay);
        DeathBoomRay(pos, Vector2.right, rad);
        yield return new WaitForSeconds(delay);
        DeathBoomRay(pos, -Vector2.up, rad);
        yield return new WaitForSeconds(delay);
        DeathBoomRay(pos, -Vector2.right, rad);
    }


    public static void DeathBoomRay(Vector2 pos, Vector2 vec, int rad)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(pos, vec, rad);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider != null && i != 0 && i < rad + 1)
            {
                var a = hits[i].collider.gameObject.GetComponent<TileLetter>();
                if (a != null)
                {
                    a.Deselect();
                    a.SetInactive();
                }
            }
        }

    }

}
