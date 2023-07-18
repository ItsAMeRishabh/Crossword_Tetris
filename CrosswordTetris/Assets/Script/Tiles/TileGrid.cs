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

    int Points = 0;
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

    //Base Functions
    public void GMAwake()
    {
        //Assign stuff
        WordHandler = GetComponent<WordSelector>();
        TilePool = GetComponent<GameObjectPool>();
        PowerUpManager = GetComponent<PowerUpManager>();

        //Import level data
        ObjectiveQuestion = lvl.Level.ObjectiveQuestion;
        ObjectivePhrase = lvl.Level.ObjectivePhrase.ToUpper();
        InitialSpawns = lvl.Level.InitalLetter;
        Moves = lvl.Level.Moves;
        ChanceOf2X = lvl.Level.ChanceOf2X;
        ChanceOfRandomWords = lvl.Level.ChanceOfRandomCharacters;
        width = lvl.Level.InitalLetter.GridSize.x;
        height = lvl.Level.InitalLetter.GridSize.y;

        //Generate the grid
        GenerateGameObjects();
    }
    public void GMStart()
    {
        //Set Display Phrase
        for (int i = 0; i < ObjectivePhrase.Length; i++)
        {
            if (!SettingsData.IsAlphaNumeric(ObjectivePhrase[i]))
                DisplayPhrase += ObjectivePhrase[i];
            else
                DisplayPhrase += "_";
        }

        UpdateDisplayTile(true);

        UIManager.QuestionBox.text = ObjectiveQuestion;
        UIManager.MovesUsedBox.text = Moves + "";
        UIManager.ExpBox.text = "";

        //Get all tiles
        for (int i = 0; i < transform.childCount; i++)
            Tiles.Add(transform.GetChild(i).GetComponent<TileLetter>());
    }
    public void Win()
    {
        Debug.Log("---WINNING SCREEN---");
        Debug.Log("Points: " + Points);
    }
    public void Lose()
    {

    }


    //Spawn Functions
    IEnumerator SpawnGoldenTile()
    {
        yield return new WaitForSeconds(1.5f);

        //Get all tile characters
        Dictionary<char, int> tiles = new();
        foreach (var tile in Tiles)
        {
            if (!tile.IsEmpty())
            {
                tiles.TryAdd(tile.character, 0);
                tiles[tile.character]++;
            }
        }

        //Get most common character
        char m = tiles.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;

        //Get all tiles with that character
        List<TileLetter> list = new();
        foreach (var tile in Tiles)
            if (tile.character == m)
                list.Add(tile);


        //Set a random tile to golden
        list[Random.Range(0, list.Count)].SetGolden();
    }
    public IEnumerator SpawnFlyTile(char character, Vector3 position, Tile destination, int i)
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



    //UI Update Functions
    public void UpdateDisplayTile(bool b)
    {
        UIManager.Display.Display(DisplayPhrase, b);

        if (ObjectivePhrase.Replace('#', ' ').Trim() == "")
        {
            Win();
        }

    }
    public float UpdatePoints()
    {
        if (WordHandler.IsWord(UIManager.OutputBox.text.ToUpper()) && UIManager.OutputBox.text.Length >= MinUserWordSize)
        {
            float pointsVal = 0;
            foreach (var tile in Tiles)
                if (tile.IsSelected())
                {
                    pointsVal += tile.GetPoints();
                }
            UIManager.ExpBox.text = "+" + pointsVal + " XP";
            return pointsVal;
        }
        else
        {
            UIManager.ExpBox.text = "";
            return 0;
        }
    }





    //Output Functions
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
    public void ResetOutput()
    {
        foreach (var tile in Tiles)
            tile.Deselect();
    }


    //Spaggeti Functions
    public void CheckTextBoxes()
    {

        string Word = UIManager.OutputBox.text.ToUpper();
        float pointsVal = UpdatePoints();


        if (!WordHandler.IsWord(Word))
        {
            Debug.LogError(Word + " is not a word!");
            return;
        }
        if (!(UIManager.OutputBox.text.Length >= MinUserWordSize))
        {
            Debug.LogError("Word should be longer than " + MinUserWordSize);
            return;
        }

        
        Moves--;
        Points += (int)pointsVal;


        UpdatePhrases(Word,out HashSet<char> chars);

        UpdateDisplayTile(false);
        SpawnFlyTiles(chars, out bool CouldSpawn);
        UseGoldenTiles();

        UIManager.OutputBox.text = "";
        UIManager.PointBox.text = "Points : " + Points;
        UIManager.MovesUsedBox.text = Moves + "";

        //StartCoroutine(nameof(SpawnCour));

        if (CouldSpawn)StartCoroutine(SpawnGoldenTile());

        CheckFor3LetterWords();


        if (Moves == 0)Lose();
    }
    private void SpawnFlyTiles(HashSet<char> chars, out bool CouldSpawn)
    {
        CouldSpawn = false;
        int tiles = 0;
        for (int i = 0; i < UIManager.Display.transform.childCount; i++)
        {
            foreach (var item in Tiles)
            {
                if (item.IsSelected() && chars.Contains(item.character))
                {
                    var tile = UIManager.Display.transform.GetChild(i).GetComponent<Tile>();
                    if (tile.character == item.character)
                    {
                        CouldSpawn = true;
                        StartCoroutine(SpawnFlyTile(item.character, item.transform.position, tile, tiles));
                        tiles++;
                        break;
                    }
                }

            }
        }        
    }



    //Misc Functions
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
    private async void CheckFor3LetterWords()
    {
        await System.Threading.Tasks.Task.Run(() =>
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
        });
    }
    public char GetCharacter()
    {
        if (Random.Range(0, 100) < ChanceOfRandomWords)
        {
            return Settings.GetRandomChar();
        }
        else
        {
            string str = ObjectivePhrase.Replace(" ", "").Replace("#", "");
            if (str.Length == 0)
            {
                return Settings.GetRandomChar();
            }
            return str[Random.Range(0, str.Length)];
        }
    }
    public void UpdatePhrases(string Word, out HashSet<char>  chars)
    {
        chars = new();
        for (int i = 0; i < ObjectivePhrase.Length; i++)
            for (int j = 0; j < Word.Length; j++)
                if (ObjectivePhrase[i].Equals(Word[j]))
                {
                    chars.Add(Word[j]);
                    DisplayPhrase = DisplayPhrase.Remove(i, 1).Insert(i, ObjectivePhrase[i] + "");
                    ObjectivePhrase = ObjectivePhrase.Remove(i, 1).Insert(i, " ");
                }
    }



    //Gameobject Functions
    private void InstantiatePrefab(Vector3 position)
    {
        GameObject instantiatedPrefab = Instantiate(prefab, transform);
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
                ((x * transform.localScale.x * scale) + transform.position.x - ((width - 1) * transform.localScale.x * scale / 2)) * (.85f + Spacing),
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