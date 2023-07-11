using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour
{

    public SpriteRenderer sr;

    public TextMesh characterMesh;

    [SerializeField]
    Sprite tileTexture;
    [SerializeField]
    Sprite emptyTexture;


    [SerializeField]
    bool isEmpty = true;
    public char character = ' ';

    public SettingsData Settings;
    
    public void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        characterMesh = GetComponentInChildren<TextMesh>();
        Empty();
    }

    public void Activate()
    {
        sr.sprite = tileTexture;
        sr.sprite = Settings.display;
        characterMesh.color = Settings.displayFontColor;
        characterMesh.text = character + "";
    }

    public void SetCharacter(char character)
    {
        isEmpty = false;
        this.character = character;
    }

    public void Empty()
    {
        sr.sprite = emptyTexture;
        isEmpty = true;
        character = ' ';
        characterMesh.text = "";
        sr.sprite = Settings.displayInactive;
    }

    public bool IsEmpty()
    {
        return isEmpty;
    }
}
