using System.Collections;
using TMPro;
using UnityEngine;

public enum Type
{
    Normal,
    Frozen,
    Bubble,
    Debris,
    Golden,
    Disabled

}
public enum Bonus
{
    None,
    Gem,
    Coin
}
public class TileLetter : MonoBehaviour
{

    public SpriteRenderer spriteRend;
    public SpriteRenderer bonusRend;

    public TextMeshPro characterMesh;

    public Type type = Type.Normal;
    public Bonus bonus = Bonus.None;


    //public float multiplier = 1;

    [SerializeField]
    int selectedIndex = -1;
    int BubbleBlocked = -1;

    public char Letter;

    public int X;
    public int Y;

    public float PopScale = .4f;
    public float ShrinkScale = .1f;

    private bool Initial = true;
    private float InitialX = 0;
    private float TopY = 0;

    public TileGrid parent;
    public Rigidbody2D rb;



    private void OnMouseDown()
    {
        if (parent.Ended)
            return;

        if (type == Type.Frozen)
            return;

        if (parent.PowerUpManager.Active == null)
        {
            if (IsSelected())
                Deselect();
            else
                Select();

            parent.UpdatePoints();
        }
        else
            parent.PowerUpManager.Use(this);

    }

    public void GMPreAwake()
    {
        characterMesh.text = "";
        spriteRend.sprite = null;
        bonusRend.sprite = null;
    }

    public void GMAwake(TileGrid tg)
    {
        InitialX = transform.position.x;
        TopY = transform.position.y + 10;
        parent = tg;
        StartActivationCouroutine(true, 0);
    }

    public void SetCoords(int X, int Y)
    {
        this.X = X;
        this.Y = Y;
    }

    //Setters
    public void Select()
    {
        selectedIndex = parent.AddToOutput(Letter);
        UpdateVisual();
    }

    public void Deselect()
    {
        parent.RemoveFromOutput(selectedIndex);
        selectedIndex = -1;
        UpdateVisual();
    }

    void Activate(char character)
    {
        transform.position = new Vector3(InitialX, transform.position.y, transform.position.z);
        if (parent.TilesNeeded[X] > 0)
        {

            if (type != Type.Debris)
            {
                if (Random.Range(0f, 99f) < parent.ChanceOfGem)
                    bonus = Bonus.Gem;

                if (Random.Range(0f, 99f) < parent.ChanceOfGem)
                    bonus = Bonus.Coin;
            }

            if (type == Type.Disabled)
            {
                type = Type.Normal;
            }

            if (type == Type.Bubble && BubbleBlocked == -1)
            {
                BubbleBlocked = parent.TilesNeeded[X];
                parent.TilesNeeded[X] = 0;
            }
            else
            {
                parent.TilesNeeded[X]--;
            }

            if (selectedIndex != -1)
                parent.RemoveFromOutput(selectedIndex);


            //type = Type.Normal;
            Letter = character;
            selectedIndex = -1;
        }
        else
        {
            type = Type.Disabled;
        }

        if (type != Type.Bubble)
        {
            transform.position = new Vector3(InitialX, TopY, transform.position.z);
        }

        UpdateVisual();

    }

    //Coroutines
    IEnumerator ActivationCouroutine(bool skip, float v)
    {
        parent.Unstable++;
        if (Bonus.Coin == bonus)
        {
            bonus = Bonus.None;
            parent.Coins++;
        }
        if (bonus == Bonus.Gem)
        {
            bonus = Bonus.None;
            parent.Gems++;
        }

        yield return new WaitForSeconds(v);
        if (!skip)
        {
            if (type == Type.Debris)
            {
                type = Type.Normal;
                UpdateVisual();
            }
            if (type == Type.Bubble)
            {
                parent.TilesNeeded[X] = BubbleBlocked;
                BubbleBlocked = -1;
                type = Type.Normal;
                UpdateVisual();
                parent.Unstable--;
                yield break;
            }
            if (type != Type.Disabled)
            {
                parent.TilesNeeded[X]++;
                RayCast.RectDefrost(transform.position, 1);
            }




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
                    switch (cell[0])
                    {
                        case '%':
                            type = Type.Debris;
                            Activate(' ');
                            break;
                        default:
                            Activate(cell[0]);
                            break;
                    }

                    break;
                case 2:
                    if (SettingsData.IsAlphaNumeric(cell[0]))
                    {
                        switch (cell[1])
                        {
                            case '@':
                                type = Type.Bubble;
                                break;

                            case '#':
                                type = Type.Frozen;
                                break;
                        }
                        Activate(cell[0]);
                    }
                    else
                    {
                        switch (cell[0])
                        {
                            case '$':
                                bonus = Bonus.Coin;
                                break;

                            case '^':
                                bonus = Bonus.Gem;
                                break;
                        }
                        Activate(cell[1]);
                    }
                    break;
                case 3:
                    switch (cell[2])
                    {
                        case '@':
                            type = Type.Bubble;
                            break;

                        case '#':
                            type = Type.Frozen;
                            break;
                    }

                    switch (cell[0])
                    {
                        case '$':
                            bonus = Bonus.Coin;
                            break;

                        case '^':
                            bonus = Bonus.Gem;
                            break;
                    }
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


        parent.Unstable--;
        //GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

    }
    public void StartActivationCouroutine(float delay)
    {
        StartCoroutine(ActivationCouroutine(false, delay));
    }
    public void StartActivationCouroutine(bool b, float delay)
    {
        StartCoroutine(ActivationCouroutine(b, delay));
    }

    //Getters/Setters

    public int GetPoints()
    {
        foreach (var item in parent.Settings.LetterMetaData)
        {
            if (item.letter == Letter)
            {
                return (int)item.value;
            }
        }


        return 0;
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

    public void UpdateVisual()
    {
        switch (bonus)
        {
            case Bonus.None:
                bonusRend.sprite = null;
                break;
            case Bonus.Gem:
                bonusRend.sprite = parent.Settings.gem;
                break;
            case Bonus.Coin:
                bonusRend.sprite = parent.Settings.coin;
                break;
        }

        if (Type.Debris != type && Type.Disabled != type)
        {
            characterMesh.text = Letter + "";
        }

        if (IsSelected())
        {
            spriteRend.sprite = parent.Settings.selected;
            characterMesh.color = parent.Settings.selectedFontColor;
            return;
        }

        if (type != Type.Disabled)
        {
            rb.simulated = true;
            spriteRend.enabled = true;
            rb.bodyType = RigidbodyType2D.Dynamic;

        }

        switch (type)
        {
            case Type.Normal:
                spriteRend.sprite = parent.Settings.normal;
                characterMesh.color = parent.Settings.normalFontColor;
                break;
            case Type.Frozen:
                spriteRend.sprite = parent.Settings.frozen;
                characterMesh.color = parent.Settings.frozenFontColor;
                break;
            case Type.Bubble:
                rb.bodyType = RigidbodyType2D.Static;
                spriteRend.sprite = parent.Settings.bubble;
                characterMesh.color = parent.Settings.bubbleFontColor;
                break;
            case Type.Golden:
                spriteRend.sprite = parent.Settings.golden;
                characterMesh.color = parent.Settings.goldenFontColor;
                break;
            case Type.Debris:
                spriteRend.sprite = parent.Settings.debris;
                break;
            case Type.Disabled:
                rb.simulated = false;
                spriteRend.enabled = false;
                //characterMesh.text = " ";
                break;
            default:
                break;
        }
    }

    public bool IsSelected()
    {
        return selectedIndex != -1;
    }

}