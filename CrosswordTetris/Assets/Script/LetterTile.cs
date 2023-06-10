using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class LetterTile : MonoBehaviour
{

    SpriteRenderer sr;

    TextMesh characterMesh;

    [SerializeField]
    Sprite tileTexture;
    [SerializeField]
    Sprite emptyTexture;


    Color inactiveColor;
    Color activeColor;
    Color activeFontColor;
    Color selectedColor;
    Color selectedFontColor;


    [SerializeField]
    int selectedIndex = -1;
    [SerializeField]
    bool isEmpty = true;
    [SerializeField]
    char character = ' ';

    TileGrid parent;


    private void OnMouseDown()
    {
        if (parent.powerUpInUse&& parent.powerUpAvailible)
        {
            HexBoom(2);
            parent.powerUpInUse = false;
            parent.powerUpAvailible = false; ;

        }

        if (IsSelected())
            Deselect();
        else
        {
            Select();
        }
    }

    public void HexBoom(int rad)
    {
        SetInactive();
        DeathBoomRay(Vector2.up, rad);
        DeathBoomRay(new Vector2(Mathf.Sqrt(3 / 4f), 0.5f), rad);
        DeathBoomRay(new Vector2(Mathf.Sqrt(3 / 4f), -0.5f), rad);
        DeathBoomRay(-Vector2.up, rad);
        DeathBoomRay(-new Vector2(Mathf.Sqrt(3 / 4f), 0.5f), rad);
        DeathBoomRay(-new Vector2(Mathf.Sqrt(3 / 4f), -0.5f), rad);
    }

    public void DeathBoomRay(Vector2 vec, int rad)
    {
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, vec,rad);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider != null&&i!=0 && i < rad+1)
                {
                //yield return new WaitForSeconds(.2f);
                    hits[i].collider.gameObject.GetComponent<LetterTile>().SetInactive();
                }
            }

    }

    public void Start()
    {
        parent = transform.parent.GetComponent<TileGrid>();
        var settings = parent.Settings;

        inactiveColor = settings.inactiveColor;
        activeColor = settings.activeColor;
        activeFontColor = settings.activeFontColor;
        selectedColor = settings.selectedColor;
        selectedFontColor = settings.selectedFontColor;

        sr = GetComponent<SpriteRenderer>();
        characterMesh = GetComponentInChildren<TextMesh>();
        SetInactive();
    }

    public void Select()
    {
        if (!isEmpty && !IsSelected())
        {
            selectedIndex = parent.AddToOutput(character); ;
            sr.color = selectedColor;

            characterMesh.color = selectedFontColor;
        }
    }
    public void Deselect()
    {
        if (!isEmpty && IsSelected())
        {
            transform.parent.GetComponent<TileGrid>().RemoveFromOutput(selectedIndex);
            selectedIndex =  -1;
            sr.color = activeColor;
            characterMesh.color = activeFontColor;
        }
    }

    public void SetActive(char character)
    {
        sr.sprite = tileTexture;
        isEmpty = false;
        this.character = character;
        sr.color = activeColor;
        characterMesh.color = activeFontColor;
        characterMesh.text = character+"";
    }

    public void SetInactive()
    {
        sr.sprite = emptyTexture;
        isEmpty = true;
        selectedIndex = -1;
        character = ' ';
        characterMesh.text = "";
        sr.color = inactiveColor;

    }

    public bool IsEmpty()
    {
        return isEmpty;
    }
    public bool IsSelected()
    {
        return selectedIndex != -1;
    }

    public int GetSelectedIndex()
    {
        return selectedIndex;
    }
    public void SetSelectedIndex(int i)
    {
        selectedIndex = i;
    }

}
