using System.Collections;
using UnityEngine;

public enum Type
{
    Normal,
    Frozen,
    Bubble
}
public class TileLetter : MonoBehaviour
{

    public SpriteRenderer spriteRend;

    public TextMesh characterMesh;

    public Type type;

    [SerializeField]
    Sprite tileTexture;
    [SerializeField]
    Sprite emptyTexture;


    public float multiplier = 1;

    [SerializeField]
    int selectedIndex = -1;
    [SerializeField]
    //bool isEmpty = true;
    bool isGolden = false;
    //bool isFrozen = false;

    public char character = ' ';

    public int X;
    public int Y;

    public float PopScale = .4f;
    public float ShrinkScale = .1f;

    private bool Initial = true;
    private float InitialX = 0;
    
    public TileGrid parent;

    public void Set(TileLetter comp)
    {
        comp.spriteRend.sprite = spriteRend.sprite;
        comp.spriteRend.color = spriteRend.color;

        comp.characterMesh.text = characterMesh.text;
        comp.characterMesh.color = characterMesh.color;

    }

    private void OnMouseDown()
    {
        if (parent.Ended)
            return;
        if (parent.PowerUpManager.Active == null)
            if (IsSelected())
                Deselect();
            else
                Select();
        else
            parent.PowerUpManager.Use(this);

    }

    public void GMAwake()
    {
        InitialX = transform.position.x;
        spriteRend = GetComponentInChildren<SpriteRenderer>();
        characterMesh = GetComponentInChildren<TextMesh>();
        parent = transform.parent.GetComponent<TileGrid>();
        StartActivationCouroutine(true,0);
    }
    
    public void SetCoords(int X, int Y)
    {
        this.X = X;
        this.Y = Y;
    }

    //Setters
    public void Select()
    {
        //GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        if (!IsSelected())
        {
            selectedIndex = parent.AddToOutput(character);
            spriteRend.sprite= parent.Settings.selected;

            characterMesh.color = parent.Settings.selectedFontColor;
        }
        parent.UpdatePoints();

    }
    public void Deselect()
    {
        if (IsSelected())
        {
            transform.parent.GetComponent<TileGrid>().RemoveFromOutput(selectedIndex);
            selectedIndex =  -1;
            if (multiplier == 1)
            {
                SetDefault();
            }
            else
            {
                Set2X();

            }
            if (isGolden) {
                SetGolden();
            }
            parent.UpdatePoints();

        }
    }
    
    void Activate(char character)
    {
        if (selectedIndex != -1)
            parent.RemoveFromOutput(selectedIndex);
        isGolden = false;
        selectedIndex = -1;
        

        multiplier = Random.Range(0, 100) < parent.ChanceOf2X ? 2 : 1;
        spriteRend.sprite = tileTexture;
        //isEmpty = false;
        this.character = character;

        if(multiplier == 1)
        {
            SetDefault();
        }
        else
        {
            Set2X();
        }

        characterMesh.text = character+"";
        //Debug.Log(character);
        if(type != Type.Bubble)  
            transform.position += Vector3.up * 10;
        transform.position = new Vector3(InitialX, transform.position.y, transform.position.z);

    }


    //Coroutines
    IEnumerator ActivationCouroutine(bool skip, float v)
    {
        
        yield return new WaitForSeconds(v);
        if (!skip)
        {

            while (spriteRend.gameObject.transform.localScale.x < 1.3f)
            {
                spriteRend.gameObject.transform.localScale += Vector3.one * PopScale;
                characterMesh.gameObject.transform.localScale += Vector3.one * PopScale;
                yield return new WaitForSeconds(0.04f);
            }
            while (spriteRend.gameObject.transform.localScale.x > 0.1f)
            {
                spriteRend.gameObject.transform.localScale -= Vector3.one * ShrinkScale;
                characterMesh.gameObject.transform.localScale -= Vector3.one * ShrinkScale;
                yield return new WaitForSeconds(0.03f);
            }
        }


        if (Initial && parent.InitialSpawns.GetCell(X, Y).Length > 0)
        {
            string cell = parent.InitialSpawns.GetCell(X, Y).ToUpper();
            switch (cell.Length)
            {
                case 1:
                    Activate(cell[0]);
                    break;
                case 2:
                    if (SettingsData.IsAlphaNumeric(cell[0]))
                    {
                        if (cell[1] == '@')
                        {
                            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                            type = Type.Bubble;
                        }
                        Activate(cell[0]);
                    }
                    else
                    {
                        Activate(cell[1]);
                    }
                    break;
                case 3:
                    Activate(cell[1]);
                    break;
                default:
                    break;
            }
        }
        else
            Activate(parent.GetCharacter());

        Initial = false;

        spriteRend.gameObject.transform.localScale = Vector3.one;
        characterMesh.gameObject.transform.localScale = Vector3.one;

        //GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

    }
    public void StartActivationCouroutine(float delay)
    {
        StartCoroutine(ActivationCouroutine(false, delay));
    }
    public void StartActivationCouroutine(bool b,float delay)
    {
        StartCoroutine(ActivationCouroutine(b, delay));
    }

    //Getters/Setters

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




    //Prop Edits

    public void SetGolden()
    {
        spriteRend.sprite= parent.Settings.golden;
        characterMesh.color = parent.Settings.goldenFontColor;
        isGolden = true;
    }
    public void SetDefault()
    {
        spriteRend.sprite = parent.Settings.active;
        characterMesh.color = parent.Settings.activeFontColor;
        //isEmpty = false;
    }
    public void Set2X()
    {
        spriteRend.sprite = parent.Settings.active2X;
        characterMesh.color = parent.Settings.active2XFontColor;
        multiplier = 2;
        //isEmpty = false;
    }

    public bool IsSelected()
    {
        return selectedIndex != -1;
    }
    public bool IsGolden()
    {
        return isGolden;
    }

}
