using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class LetterTile : MonoBehaviour
{

    public SpriteRenderer sr;

    public TextMesh characterMesh;

    [SerializeField]
    Sprite tileTexture;
    [SerializeField]
    Sprite emptyTexture;


    public float multiplier = 1;

    [SerializeField]
    int selectedIndex = -1;
    [SerializeField]
    bool isEmpty = true;
    public char character = ' ';

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

    public void GMAwake()
    {
        sr = GetComponent<SpriteRenderer>();
        characterMesh = GetComponentInChildren<TextMesh>();
        parent = transform.parent.GetComponent<TileGrid>();      
        SetInactive();
    }

    public void Select()
    {
        if (!isEmpty && !IsSelected())
        {
            selectedIndex = parent.AddToOutput(character);
            sr.color = parent.Settings.selectedColor;

            characterMesh.color = parent.Settings.selectedFontColor;
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
                sr.color = parent.Settings.activeColor;
                characterMesh.color = parent.Settings.activeFontColor;
            }
            else
            {
                sr.color = parent.Settings.active2XColor;
                characterMesh.color = parent.Settings.active2XFontColor;

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
            sr.color = parent.Settings.activeColor;
            characterMesh.color = parent.Settings.activeFontColor;
        }
        else
        {
            sr.color = parent.Settings.active2XColor;
            characterMesh.color = parent.Settings.active2XFontColor;

        }


        characterMesh.text = character+"";
        transform.position += Vector3.up * 8;
    }

    public void SetInactive()
    {
        sr.sprite = emptyTexture;
        isEmpty = true;
        selectedIndex = -1;
        character = ' ';
        characterMesh.text = "";
        sr.color = parent.Settings.inactiveColor;
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
