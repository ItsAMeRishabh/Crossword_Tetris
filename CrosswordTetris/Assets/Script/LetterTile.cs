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

    [SerializeField]
    Color color;
    [SerializeField]
    Color selectedColor;


    [SerializeField]
    int selectedIndex = -1;
    [SerializeField]
    bool isEmpty = true;
    [SerializeField]
    char character = ' ';

    private void OnMouseDown()
    {
        if (IsSelected())
            Deselect();
        else
            Select();
    }

    public void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        characterMesh = GetComponentInChildren<TextMesh>();
        SetInactive();
    }

    public void Select()
    {
        if (!isEmpty && !IsSelected())
        {
            selectedIndex = transform.parent.GetComponent<TileGrid>().AddToOutput(character); ;
            sr.color = selectedColor;
        }
    }
    public void Deselect()
    {
        if (!isEmpty && IsSelected())
        {
            transform.parent.GetComponent<TileGrid>().RemoveFromOutput(selectedIndex);
            selectedIndex =  -1;
            sr.color = color;
        }
    }

    public void SetActive(char character)
    {
        sr.sprite = tileTexture;
        isEmpty = false;
        this.character = character;
        sr.color = color;
        characterMesh.text = character+"";
    }

    public void SetInactive()
    {
        sr.sprite = emptyTexture;
        isEmpty = true;
        selectedIndex = -1;
        character = ' ';
        characterMesh.text = "";
        sr.color = color;
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
