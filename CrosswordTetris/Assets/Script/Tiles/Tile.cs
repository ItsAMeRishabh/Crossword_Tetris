using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour
{

    public SpriteRenderer sr;
    public ParticleSystem ps;

    public TextMesh characterMesh;

    Sprite tileTexture;
    Color fontcolor;
    Sprite emptyTexture;


    [SerializeField]
    bool isEmpty = true;
    public char character = ' ';

    public SettingsData Settings;
    
    public void Awake()
    {
        //sr = GetComponent<SpriteRenderer>();
        //characterMesh = GetComponentInChildren<TextMesh>();
        if (TryGetComponent(out FlyPather _))
        {
            tileTexture = Settings.fly;
            emptyTexture = Settings.inactive;
            fontcolor = Settings.flyFontColor;
            //characterMesh
        }
        else
        {
            tileTexture = Settings.display;
            emptyTexture = Settings.displayInactive;
            fontcolor = Settings.displayFontColor;
        }

        Empty();
    }

    public void Activate()
    {
        ps.Play();
        sr.sprite = tileTexture;
        //sr.sprite = Settings.display;
        characterMesh.color = fontcolor;// Settings.displayFontColor;
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
        //sr.sprite = Settings.displayInactive;
    }

    public bool IsEmpty()
    {
        return isEmpty;
    }
}
