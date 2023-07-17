using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;
using System.Collections;
using Array2DEditor;

public class GoldenTileMeta
{
    public Vector3 position;
    public char character;
    public int didFunction = 0;

    public GoldenTileMeta(Vector3 position, char character)
    {
        this.position = position;
        this.character = character;
    }

}

[RequireComponent(typeof(WordSelector))]
[RequireComponent(typeof(GameObjectPool))]
[RequireComponent(typeof(GameManager))]
public class TileGrid : MonoBehaviour
{
    readonly List<TileLetter> Tiles = new();

    string ObjectiveQuestion = "Something witty here... idk";
    
    int points = 0;
    float ChanceOfRandomWords = 40f;
    int Moves;


    [NonSerialized]
    public string ObjectivePhrase;    
    [NonSerialized]
    public Array2DString InitialSpawns;
    [NonSerialized]
    public string DisplayPhrase;
    [NonSerialized]
    public float ChanceOf2X = 10f;
    [NonSerialized]
    public WordSelector WordHandler;
    [NonSerialized]
    public GameObjectPool TilePool;
    [NonSerialized]
    public PowerUpManager PowerUpManager;
    [NonSerialized]
    public int width;
    [NonSerialized]
    public int height;

    [SerializeField]
    int MinUserWordSize = 3;

    [Space(10)]
    public GameObject prefab;
    public ActiveLevel lvl;
    public SettingsData Settings;
    [Space(20)]
    public UIManager UIManager;
    [Space(20)]
    public float Spacing = 0;
    public float FlyTileSpawnDelay = .3f;
    public void GMAwake()
    {
        WordHandler = GetComponent<WordSelector>();
        TilePool = GetComponent<GameObjectPool>();
        PowerUpManager = GetComponent<PowerUpManager>();

        ObjectiveQuestion = lvl.Level.ObjectiveQuestion;
        ObjectivePhrase = lvl.Level.ObjectivePhrase;

        //int h = lvl.Level.InitalSpawns.GridSize.y- (lvl.Level.InitalSpawns.GridSize.x * lvl.Level.InitalSpawns.GridSize.y);
        //for (int x = 0; x < h; x++)
        //        lvl.Level.InitialWord.Add("");

        InitialSpawns = lvl.Level.InitalLetter;//.ToArray().Reverse().ToArray()

        Moves = lvl.Level.Moves;

        ChanceOf2X = lvl.Level.ChanceOf2X;
        ChanceOfRandomWords = lvl.Level.ChanceOfRandomCharacters;


        width = lvl.Level.InitalLetter.GridSize.x;
        height = lvl.Level.InitalLetter.GridSize.y;
        GenerateGameObjects();
    }

    public void GMStart()
    {
        ObjectivePhrase = ObjectivePhrase.ToUpper();

        for (int i = 0; i < ObjectivePhrase.Length; i++)
            if (!IsAlphaNumeric(ObjectivePhrase[i]))
                DisplayPhrase += ObjectivePhrase[i];
            else
                DisplayPhrase += "_";
        
        ActivateDisplayTile(true);

        UIManager.QuestionBox.text = ObjectiveQuestion;
        UIManager.MovesUsedBox.text = Moves + "";

        for (int i = 0; i < transform.childCount; i++)
            Tiles.Add(transform.GetChild(i).GetComponent<TileLetter>());



        //CheckFor3LetterWords();
        //StartCoroutine(nameof(SpawnCour));
    }

    public static bool IsAlphaNumeric(char v)
    {
        return (v >= 'a' && v <= 'z') || (v >= 'A' && v <= 'Z') || (v >= '0' && v <= '9');
    }

    IEnumerator SpawnGoldenTile()
    {
        yield return new WaitForSeconds(2f);
        Dictionary<char, int> tiles = new();
        foreach (var tile in Tiles)
        {
            if (!tile.IsEmpty())
            {
                tiles.TryAdd(tile.character, 0);
                tiles[tile.character]++;
            }
        }
        //LogDictionary<char>(tiles);
        char m = tiles.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
        List<TileLetter> list = new ();
        foreach (var tile in Tiles)
            if(tile.character == m)
                list.Add(tile);


        list[Random.Range(0, list.Count)].SetGolden();
        


    }


   public void ActivateDisplayTile(bool b)
    {
        UIManager.Display.Display(DisplayPhrase,b);

        if (ObjectivePhrase.Replace('#', ' ').Trim() == "")
        {
            Debug.Log("---WINNING SCREEN---");
            Debug.Log("Points: " + points);
        }

    }

    public char GetCharacter()
    {
        if (Random.Range(0, 100) < ChanceOfRandomWords)
        {
            return Settings.GetAlphabet()[Random.Range(0, 26)];
        }
        else
        {
            string str = ObjectivePhrase.Replace(" ", "").Replace("#","");
            if(str.Length == 0)
            {
                return Settings.GetAlphabet()[Random.Range(0, 26)];
            }
            return str[Random.Range(0, str.Length)];
        }
    }


    public void UpdatePoints()
    {
        float pointsVal = 0;
        foreach (var tile in Tiles)
            if (tile.IsSelected())
                pointsVal += tile.GetPoints();

        if (pointsVal == 0)

            UIManager.ExpBox.text = "";
        else
        UIManager.ExpBox.text = "+" + pointsVal + " XP";

    }

    public int AddToOutput(char c)
    {
        UIManager.OutputBox.text += c;
        return UIManager.OutputBox.text.Length - 1;
    }
    public void RemoveFromOutput(int i)
    {
        if (i < UIManager.OutputBox.text.Length)
        {
            foreach (var tile in Tiles)
                if (tile.GetSelectedIndex() >= i)
                    tile.SetSelectedIndex(tile.GetSelectedIndex() - 1);

            UIManager.OutputBox.text = UIManager.OutputBox.text.Remove(i, 1);
        }
    }


    public IEnumerator SpawnFlyTile(char character,Vector3 position, Tile destination , int i)
    {
        //@
        yield return new WaitForSeconds(i * FlyTileSpawnDelay);
        var fly = TilePool.Pool.Get();// Instantiate(FlyTile, item.transform.position, Quaternion.identity);
        fly.transform.SetPositionAndRotation(position, Quaternion.identity);
        var flytile = fly.GetComponent<FlyTile>();
        flytile.target = destination;
        flytile.Init();
        fly.GetComponent<Tile>().SetCharacter(character);
        fly.GetComponent<Tile>().Activate();
    }

    public void ResetOutput()
    {
        foreach (var tile in Tiles)
            tile.Deselect();
    }
    public void CheckTextBoxes()
    {

        string word = UIManager.OutputBox.text.ToUpper();
        
        float pointsVal = 0;        
        foreach (var tile in Tiles)
            if (tile.IsSelected())
                pointsVal += tile.GetPoints();

        if(!WordHandler.IsWord(word))
        {
            Debug.LogError(word + " is not a word!");
        }

        if (UIManager.OutputBox.text.Length >= MinUserWordSize && WordHandler.IsWord(word))
            MakeWord(pointsVal,word);
    }

    public void MakeWord(float pointsVal, string word)
    {
        Moves--;

        points += (int)pointsVal;
        HashSet<char> chars = new();
        for (int i = 0; i < ObjectivePhrase.Length; i++)
            for (int j = 0; j < word.Length; j++)
                if (ObjectivePhrase[i].Equals(word[j]))
                {
                    chars.Add(word[j]);
                    DisplayPhrase = DisplayPhrase.Remove(i, 1).Insert(i, ObjectivePhrase[i] + "");
                    ObjectivePhrase = ObjectivePhrase.Remove(i, 1).Insert(i, " ");
                }

        ActivateDisplayTile(false);
        bool b = SpawnFlyTiles(chars);
        UseGoldenTiles();

        UIManager.OutputBox.text = "";
        UIManager.PointBox.text = "Points : " + points;
        UIManager.MovesUsedBox.text = Moves + "";

        //StartCoroutine(nameof(SpawnCour));

        if (b)
            StartCoroutine(SpawnGoldenTile());

        CheckFor3LetterWords();

        if(Moves == 0)
        {
            Debug.Log("LOSING SCREEN");
            Debug.Log("OUT OF MOVES");
        }
    }

    private void CheckFor3LetterWords()
    {
        for (int x = 0; x < Tiles.Count; x++)
        {
            for (int y = 0; y < Tiles.Count; y++)
            {
                for (int z = 0; z < Tiles.Count; z++)
                {
                    if (x != y && y != z && z != x)
                    {
                        string s = Tiles[x].character + "" + Tiles[y].character + "" + Tiles[z].character;
                        if (WordHandler.IsWord(s))
                        {
                            Debug.LogWarning(s);
                            return;
                        }
                    }
                }
            }
        }

        foreach (var item in Tiles)
        {
            item.StartInactiveCouroutine();
        }
    }
    
    private bool SpawnFlyTiles(HashSet<char> chars)
    {
        bool b = false;
        int tiles = 0;
        for (int i = 0; i < UIManager.Display.transform.childCount; i++)
        {
            foreach (var item in Tiles)
            {
                if (item.IsSelected())
                {
                    if (chars.Contains(item.character))
                    {
                        var tile = UIManager.Display.transform.GetChild(i).GetComponent<Tile>();
                        if (tile.character == item.character)
                        {
                            b = true;
                            StartCoroutine(SpawnFlyTile(item.character, item.transform.position, tile,tiles));
                                           //@SpawnFlyTile(item.character, item.transform.position, tile,tiles);
                            tiles ++;
                            break;
                        }
                    }

                }
            }
        }

        return b;
    }

    private void UseGoldenTiles()
    {
        List<GoldenTileMeta> GoldenTiles = new();
        foreach (var item in Tiles)
            if (item.IsSelected() && item.IsGolden())
            {
                GoldenTiles.Add(new GoldenTileMeta(item.transform.position, item.character));
            }
        foreach (var item in Tiles)
        {
            foreach (var item1 in GoldenTiles)
            {
                if (item.character == item1.character)
                {
                    //Debug.Log(item.character);
                    item1.didFunction++;
                    item.StartInactiveCouroutine(.5f);
                }
            }

            if (item.IsSelected())
            {
                item.Deselect();
                item.StartInactiveCouroutine();
            }
        }

        foreach (var item in GoldenTiles)
            if (item.didFunction <= 1)
                RayCast.RectBoom(item.position, 1);

    }


    private void InstantiatePrefab(Vector3 position)
    {
        GameObject instantiatedPrefab = Instantiate(prefab,transform);
        instantiatedPrefab.transform.SetLocalPositionAndRotation(position, Quaternion.identity);
        instantiatedPrefab.transform.localScale = Vector3.one;
    }

    internal void GenerateGameObjects()
    {
        float scale = 4f;
        RemoveGameObjects();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
        {
                InstantiatePrefab(new Vector3(
                (( x * transform.localScale.x * scale) + transform.position.x - ((width - 1) * transform.localScale.x * scale / 2) )* (.85f+Spacing),
                (-y * transform.localScale.y * scale) + transform.position.y,
                5));
            }
        }
    }
    internal void RemoveGameObjects()
    {
        var tempArray = new GameObject[transform.childCount];

        for (int i = 0; i < tempArray.Length; i++)
        {
            tempArray[i] = transform.GetChild(i).gameObject;
        }

        foreach (var child in tempArray)
        {
            DestroyImmediate(child);
        }
    }
}
