using System;
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
    Color active2XColor;
    Color active2XFontColor;
    Color selectedColor;
    Color selectedFontColor;

    [SerializeField]
    public float multiplier = 1;

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
        active2XColor = settings.active2XColor;
        active2XFontColor = settings.active2XFontColor;
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
            if (multiplier == 1)
            {
                sr.color = activeColor;
                characterMesh.color = activeFontColor;
            }
            else
            {
                sr.color = active2XColor;
                characterMesh.color = active2XFontColor;

            }
        }
    }

    public void SetActive(char character, float mult)
    {
        multiplier = mult;
        sr.sprite = tileTexture;
        isEmpty = false;
        this.character = character;

        if(multiplier == 1)
        {
            sr.color = activeColor;
            characterMesh.color = activeFontColor;
        }
        else
        {
            sr.color = active2XColor;
            characterMesh.color = active2XFontColor;

        }


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

    public int GetPoints()
    {
        float RET = 0;
        foreach (var item in parent.Settings.LetterPoints)
        {
            if(item.word == character + "")
            {
                RET = item.value;
            }
        }


        return (int)(RET * multiplier);
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
