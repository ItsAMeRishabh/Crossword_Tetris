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
        if (IsSelected())
            Deselect();
        else
        {
            Select();
        }
    }

    public void GMAwake(int i, int width)
    {
        InitialX = transform.position.x;
        X = (int)Mathf.Floor(i / width);
        Y = (i % width); 
        spriteRend = GetComponentInChildren<SpriteRenderer>();
        characterMesh = GetComponentInChildren<TextMesh>();
        parent = transform.parent.GetComponent<TileGrid>();
        StartCoroutine(InactiveCouroutine(true));
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
        transform.position += Vector3.up * 8;
        transform.position = new Vector3(InitialX, transform.position.y, transform.position.z);
    }
    private void SetInactive()
    {
        spriteRend.sprite = emptyTexture;
        isEmpty = true;
        isGolden = false;
        selectedIndex = -1;
        character = ' ';
        characterMesh.text = "";
        spriteRend.sprite= parent.Settings.inactive;

    }



    //Coroutines
    private IEnumerator InactiveCouroutine(bool skip)
    {
        isEmpty = true;

        yield return new WaitForSeconds(0.0f);
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

        if (Initial && (parent.InitialSpawns.Length > Y && parent.InitialSpawns[Y].Length > X))
            SetActive(parent.InitialSpawns[Y][X].ToString().ToUpper()[0]);
        else
            SetActive(parent.GetCharacter());

        Initial = false;

        spriteRend.gameObject.transform.localScale = Vector3.one;
        characterMesh.gameObject.transform.localScale = Vector3.one;
    }
    public void StartInactiveCouroutine()
    {
        StartCoroutine(InactiveCouroutine(false));
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
