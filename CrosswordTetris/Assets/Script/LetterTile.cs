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

        if (IsSelected())
            Deselect();
        else
        {
            Select();
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
