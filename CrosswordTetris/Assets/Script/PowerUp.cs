using UnityEngine;

public abstract class PowerUp : MonoBehaviour
{
    public bool CanDo = false;
    public bool IsActive = true;
    public void Toggle()
    {
        IsActive = !IsActive;
    }
    public void HexBoom(Vector2 pos, int rad)
    {
        DeathBoomRay(pos, Vector2.up, rad);
        DeathBoomRay(pos, new Vector2(Mathf.Sqrt(3 / 4f), 0.5f), rad);
        DeathBoomRay(pos, new Vector2(Mathf.Sqrt(3 / 4f), -0.5f), rad);
        DeathBoomRay(pos, -Vector2.up, rad);
        DeathBoomRay(pos, -new Vector2(Mathf.Sqrt(3 / 4f), 0.5f), rad);
        DeathBoomRay(pos, -new Vector2(Mathf.Sqrt(3 / 4f), -0.5f), rad);
    }

    public void DeathBoomRay(Vector2 pos, Vector2 vec, int rad)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(pos, vec, rad);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider != null && i != 0 && i < rad + 1)
            {
                hits[i].collider.gameObject.GetComponent<LetterTile>().SetInactive();
            }
        }

    }

}
