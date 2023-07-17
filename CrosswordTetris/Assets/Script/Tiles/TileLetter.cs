using System.Collections;
using UnityEngine;

public class TileLetter : MonoBehaviour
{

    public SpriteRenderer spriteRend;

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
    bool isGolden = false;
    public char character = ' ';

    public int X;
    public int Y;

    public float PopScale = .4f;
    public float ShrinkScale = .1f;

    private bool Initial = true;
    private float InitialX = 0;
    TileGrid parent;

    public void Set(TileLetter comp)
    {
        comp.spriteRend.sprite = spriteRend.sprite;
        comp.spriteRend.color = spriteRend.color;

        comp.characterMesh.text = characterMesh.text;
        comp.characterMesh.color = characterMesh.color;

    }

    private void OnMouseDown()
    {
        if (parent.PowerUpManager.Active == null)
            if (IsSelected())
                Deselect();
            else
                Select();
        else
            parent.PowerUpManager.Use(this);

    }

    public void GMAwake(int i, int width)
    {
        InitialX = transform.position.x;
        Y = (int)Mathf.Floor(i / width);
        X = (i % width); 
        spriteRend = GetComponentInChildren<SpriteRenderer>();
        characterMesh = GetComponentInChildren<TextMesh>();
        parent = transform.parent.GetComponent<TileGrid>();
        StartCoroutine(InactiveCouroutine(true,0));
    }
    
    //Setters
    public void Select()
    {
        if (!isEmpty && !IsSelected())
        {
            selectedIndex = parent.AddToOutput(character);
            spriteRend.sprite= parent.Settings.selected;

            characterMesh.color = parent.Settings.selectedFontColor;
        }
        parent.UpdatePoints();

    }
    public void Deselect()
    {
        if (!isEmpty && IsSelected())
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
    public void SetActive(string s)
    {
        if(s.Length <1)
        {
            SetActive('!');
        }
        else
        {
            SetActive(s[0]);
        }   
    }
    public void SetActive(char character)
    {

        multiplier = Random.Range(0, 100) < parent.ChanceOf2X ? 2 : 1;
        spriteRend.sprite = tileTexture;
        isEmpty = false;
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
        transform.position += Vector3.up * 10;
        transform.position = new Vector3(InitialX, transform.position.y, transform.position.z);

    }
    private void SetInactive()
    {
        if (selectedIndex != -1)
            parent.RemoveFromOutput(selectedIndex);
        spriteRend.sprite = emptyTexture;
        isEmpty = true;
        isGolden = false;
        selectedIndex = -1;
        character = ' ';
        characterMesh.text = "";
        spriteRend.sprite= parent.Settings.inactive;

    }



    //Coroutines
    private IEnumerator InactiveCouroutine(bool skip, float v)
    {
        isEmpty = true;

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

        SetInactive();

        if (Initial && parent.InitialSpawns.GetCell(X,Y).Length > 0)//&& (parent.InitialSpawns.GridSize.y > Y && parent.InitialSpawns.GridSize.x > X))
            SetActive(parent.InitialSpawns.GetCell(X,Y).ToUpper());
        else
            SetActive(parent.GetCharacter());

        Initial = false;

        spriteRend.gameObject.transform.localScale = Vector3.one;
        characterMesh.gameObject.transform.localScale = Vector3.one;
    }
    public void StartInactiveCouroutine(float delay)
    {
        StartCoroutine(InactiveCouroutine(false,delay));
    }
    public void StartInactiveCouroutine()
    {
        StartCoroutine(InactiveCouroutine(false,0));
    }

    //Getters

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
        isEmpty = false;
    }
    public void Set2X()
    {
        spriteRend.sprite = parent.Settings.active2X;
        characterMesh.color = parent.Settings.active2XFontColor;
        multiplier = 2;
        isEmpty = false;
    }

    public bool IsEmpty()
    {
        return isEmpty;
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
