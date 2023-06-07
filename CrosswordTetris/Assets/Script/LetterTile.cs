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
    bool isSelected = true;
    [SerializeField]
    bool isEmpty = true;
    [SerializeField]
    char character = ' ';

    private void OnMouseDown()
    {
        Debug.Log(character);
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
        if (!isEmpty && !isSelected)
        {
            isSelected = true;
            sr.color = selectedColor;

            transform.parent.GetComponent<TileGrid>().AddOutput(character);
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
        isSelected = false;
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
        return isSelected;
    }
}
