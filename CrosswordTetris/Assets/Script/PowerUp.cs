using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerUp
{
    public int Amount = 0;

    public void Use(TileLetter tile)
    {
        if (Amount > 0)
        {
            Amount--;
            Activate(tile);
        }
    }
    public abstract void Activate(TileLetter tile);

}

public class Shovel : PowerUp
{
    public override void Activate(TileLetter tile)
    {
        tile.parent.UpdatePhrases(tile.Letter.ToString(),out HashSet<char> hs);
        bool b = hs.Contains(tile.Letter);
        tile.StartActivationCouroutine(0);
        if (b)
            tile.parent.SpawnFlyTile(tile,0);

    }
}
public class Reveal : PowerUp
{
    readonly int RevealAmount;
    public Reveal(int revealAmount)
    {
        RevealAmount = revealAmount;
    }

    public override void Activate(TileLetter tile)
    {
        TileGrid tileGrid = tile.GetComponentInParent<TileGrid>();
        List<int> keyValuePairs = new();

        for (int i = 0; i < tileGrid.ObjectivePhrase.Length; i++)
            if (SettingsData.IsAlphaNumeric(tileGrid.ObjectivePhrase[i]))
                keyValuePairs.Add(i);



        for (int i = 0; i < RevealAmount; i++) 
            if (keyValuePairs.Count != 0)
            {

                int rand = keyValuePairs[UnityEngine.Random.Range(0, keyValuePairs.Count)];

                tileGrid.DisplayPhrase = tileGrid.DisplayPhrase.Remove(rand, 1).Insert(rand, tileGrid.ObjectivePhrase[rand] + "");
                tileGrid.ObjectivePhrase = tileGrid.ObjectivePhrase.Remove(rand, 1).Insert(rand, " ");

                keyValuePairs.Remove(rand);
            }
        tileGrid.UpdateDisplayTile(tileGrid);
    }
}
public static class RayCast
{
    public static void RectBoom(Vector2 pos, int rad)
    {
        BoomRay(pos, Vector2.up, rad);
        BoomRay(pos, Vector2.right, rad);
        BoomRay(pos, -Vector2.up, rad);
        BoomRay(pos, -Vector2.right, rad);
    }
    public static void RectDefrost(Vector2 pos, int rad)
    {
        DefrostRay(pos, Vector2.up, rad);
        DefrostRay(pos, Vector2.right, rad);
        DefrostRay(pos, -Vector2.up, rad);
        DefrostRay(pos, -Vector2.right, rad);
    }

    private static void Ray(Vector2 pos, Vector2 vec, int rad, System.Action<TileLetter> action)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(pos, vec, rad);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider != null && i != 0 && i < rad + 1)
            {
                if (hits[i].collider.gameObject.TryGetComponent<TileLetter>(out var a))
                {
                    action.Invoke(a);
                }
            }
        }


    }

    static void BoomRay(Vector2 pos, Vector2 vec, int rad)
    {
        Ray(pos, vec, rad, (TileLetter a) => {
            if(a.IsSelected())
                a.Deselect();
            a.StartActivationCouroutine(0);
        });
    }
    static void DefrostRay(Vector2 pos, Vector2 vec, int rad)
    {
        Ray(pos, vec, rad, (TileLetter a) => {
            if (a.type == Type.Frozen)
            {
                a.type = Type.Normal;
                a.UpdateVisual();
            }

            if(a.type == Type.Debris)
            {
                a.StartActivationCouroutine(0);
            }
        });
    }

}

/*
     public static void HexBoom(Vector2 pos, int rad)
    {
        DeathBoomRay(pos, Vector2.up, rad);
        DeathBoomRay(pos, new Vector2(Mathf.Sqrt(3 / 4f), 0.5f), rad);
        DeathBoomRay(pos, new Vector2(Mathf.Sqrt(3 / 4f), -0.5f), rad);
        DeathBoomRay(pos, -Vector2.up, rad);
        DeathBoomRay(pos, -new Vector2(Mathf.Sqrt(3 / 4f), 0.5f), rad);
        DeathBoomRay(pos, -new Vector2(Mathf.Sqrt(3 / 4f), -0.5f), rad);
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
*/