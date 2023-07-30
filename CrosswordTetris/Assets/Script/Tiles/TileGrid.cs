using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;
using System.Collections;
using Array2DEditor;
using UnityEngine.UIElements;

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
    public List<int> TilesNeeded = new();

    string ObjectiveQuestion = "Something witty here... idk";

    int Points = 0;
    int Stars = 0;
    float ChanceOfRandomWords = 40f;
    int Moves;
    bool HasWon = false;
    public int[] StarsThreshHolds;


    public int _unstable;
    public int Unstable
    {
        get
        {
            return _unstable;
        }
        set
        {
            _unstable = value;
            if(_unstable == 0)
            {
                StartCoroutine(Win());
                StartCoroutine(Lose());
            }
        }
    }


    [NonSerialized]
    public bool Ended = false;
    [NonSerialized]
    public string ObjectivePhrase;
    [NonSerialized]
    public Array2DString InitialSpawns;
    [NonSerialized]
    public string DisplayPhrase;
    [NonSerialized]
    public float ChanceOfCoin;
    [NonSerialized]
    public float ChanceOfGem;
    public WordSelector WordHandler;
    public GameObjectPool TilePool;
    public PowerUpManager PowerUpManager;
    [NonSerialized]
    public int width;
    [NonSerialized]
    public int height;

    [SerializeField]
    int MinUserWordSize = 3;

    [Space(10)]
    public GameObject prefab;
    public LevelHandler lvl;
    public SettingsData Settings;
    [Space(20)]
    public UIManager UIManager;
    [Space(20)]
    public float Spacing = 0;
    public float FlyTileSpawnDelay = .3f;
    public float ObjectiveDelay = 1f;

    public int Coins = 0;
    public int Gems = 0;


    //Base Functions

    private void Update()
    {
        UIManager.FPSCount.text = "FPS : " + (1 / Time.deltaTime).ToString("0");    
    }

    public float bgoffset;
    public float bgscale;

    public void GMAwake()
    {
        //Assign stuff
        //WordHandler = GetComponent<WordSelector>();
        //TilePool = GetComponent<GameObjectPool>();
        //PowerUpManager = GetComponent<PowerUpManager>();

        //Import level data
        Level level = lvl.Levels[lvl.Current];
        ObjectiveQuestion = level.ObjectiveQuestion;
        ObjectivePhrase = level.ObjectivePhrase.ToUpper();
        InitialSpawns = level.InitalLetter;
        Moves = level.Moves;
        ChanceOfCoin = level.ChanceOfCoin;
        ChanceOfGem = level.ChanceOfGem;
        ChanceOfRandomWords = level.ChanceOfRandomCharacters;
        width = level.InitalLetter.GridSize.x;
        height = level.InitalLetter.GridSize.y;

        UIManager.TileGridBG.sizeDelta = new Vector2(bgoffset + (width * 131 * bgscale), bgoffset + (height * 136 * bgscale));

        StarsThreshHolds = (int[])level.StarsThreshHolds.Clone();
        UIManager.PointsProgress.SetStars(StarsThreshHolds);

        for (int i = 0; i < width; i++)
            TilesNeeded.Add(height);


        //Generate the grid
        GenerateGameObjects();
    }
    public void GMStart()
    {
        Settings.Caliberate();
        //Set Display Phrase
        for (int i = 0; i < ObjectivePhrase.Length; i++)
        {
            if (!SettingsData.IsAlphaNumeric(ObjectivePhrase[i]))
                DisplayPhrase += ObjectivePhrase[i];
            else
                DisplayPhrase += "_";
        }


        UpdateDisplayTile(true);
        DisplayPhrase = DisplayPhrase.Replace("#", "");
        ObjectivePhrase = ObjectivePhrase.Replace("#", "");

        UpdateUI();
        UIManager.QuestionBox.text = ObjectiveQuestion;
        
        //Get all tiles
        for (int i = 0; i < transform.childCount; i++)
        {
            Tiles.Add(transform.GetChild(i).GetComponent<TileLetter>());
            Tiles[^1].GMPreAwake();
        }

        StartCoroutine(ShowObjective());
    }
    IEnumerator Win()
    {
        if (ObjectivePhrase.Trim().Length == 0)
        {
            yield return new WaitForSeconds(1);
            UpdateUI();
            if (lvl.CompletedLevels <= lvl.Current)
                lvl.SetCompletedLevels(lvl.Current + 1);
            //lvl.CompletedLevels = 
            HasWon = true;
            Debug.Log("---WINNING SCREEN---");
            Debug.Log("Points: " + Points);
            Debug.Log("Stars: " + Stars);
            //Time.timeScale = 0;
            Ended = true;
            UIManager.WinPanel.Win(DisplayPhrase,Stars,Points, Coins, Gems);
        }
    }
    IEnumerator Lose()
    {
        if (Moves == 0 && !HasWon)
        {
            yield return new WaitForSeconds(1);
            UpdateUI();
            UIManager.LosePanel.SetActive(true);
            Ended = true;
        }
    }

    IEnumerator ShowObjective()
    {
        yield return new WaitForSeconds(ObjectiveDelay);

        foreach (var tile in Tiles)
            tile.GMAwake(this);
        
    }

    //Spawn Functions
    IEnumerator SpawnGoldenTile()
    {
        yield return new WaitForSeconds(1.5f);

        //Get all tile characters
        Dictionary<char, int> tiles = new();
        foreach (var tile in Tiles)
        {
            if (tile.type == Type.Normal)
            {
                tiles.TryAdd(tile.Letter, 0);
                tiles[tile.Letter]++;
            }
        }

        //Get most common character
        if (tiles.Count > 0)
        {
            char m = tiles.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;

            //Get all tiles with that character
            List<TileLetter> list = new();
            foreach (var tile in Tiles)
                if (tile.Letter == m)
                    list.Add(tile);


            //Set a random tile to golden
            var g  = list[Random.Range(0, list.Count)];
            g.type = Type.Golden;
            g.UpdateVisual();
        }
    }
    IEnumerator SpawnFlyTileCour(char character, Vector3 position, Tile destination, int offset)
    {
        yield return new WaitForSeconds(offset * FlyTileSpawnDelay);
        var fly = TilePool.Pool.Get();// Instantiate(FlyTile, item.transform.position, Quaternion.identity);
        fly.transform.SetPositionAndRotation(position, Quaternion.identity);

        var flypath = fly.GetComponent<FlyPather>();
        flypath.target = destination;
        flypath.Init();
        
        var tile = fly.GetComponent<Tile>();
        tile.SetCharacter(character);
        tile.Activate();
    }

    public bool SpawnFlyTile(TileLetter tile , int offset)
    {
        bool CouldSpawn = false;
        //StartCoroutine(SpawnFlyTileCour(character, position, destination, offset));
        for (int i = 0; i < UIManager.Display.transform.childCount; i++)
        {
            var distile = UIManager.Display.transform.GetChild(i).GetComponent<Tile>();
            if (distile.character == tile.Letter)
            {
                CouldSpawn = true;
                StartCoroutine(SpawnFlyTileCour(tile.Letter, tile.transform.position, distile, offset));
                //break;
            }
        }

        return CouldSpawn;
    }


    //UI Update Functions
    public void UpdateDisplayTile(bool b)
    {
        UIManager.Display.Display(DisplayPhrase, b);

    }
    public float UpdatePoints()
    {

        float pointsVal = 0;
        foreach (var tile in Tiles)
            if (tile.IsSelected())
            {
                pointsVal += tile.GetPoints();
            }
        UIManager.ExpBox.text = "+" + pointsVal + " XP";
        //Debug.LogError(pointsVal + " PTS");
        return pointsVal;
    }

    private void UpdateUI()
    {

        UIManager.PointsProgress.Points(Points);
        UIManager.OutputBox.text = "";
        UIManager.PointBox.text = "Points : " + Points;
        UIManager.MovesUsedBox.text = Moves + "";
        UIManager.Coins.text = (Coins + lvl.Coins) + "";
        UIManager.Gems.text = (Gems + lvl.Gems) + " Gems";
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

        UIManager.OutputBox.text = "";
    }


    //Spaggeti Functions
    public void CheckTextBoxes()
    {

        string Word = UIManager.OutputBox.text.ToUpper();
        float pointsVal = UpdatePoints();


        if (!WordHandler.IsWord(Word))
        {
            UIManager.TextField.ResetTrigger("Incorrect");
            UIManager.TextField.SetTrigger("Incorrect");
            Debug.LogError(Word + " is not a word!");
            return;
        }
        if (!(UIManager.OutputBox.text.Length >= MinUserWordSize))
        {
            UIManager.TextField.ResetTrigger("Incorrect");
            UIManager.TextField.SetTrigger("Incorrect");
            Debug.LogError("Word should be longer than " + MinUserWordSize);
            return;
        }

        UIManager.TextField.ResetTrigger("Correct");
        UIManager.TextField.SetTrigger("Correct");
        for (int i = 0; i < StarsThreshHolds.Length; i++)
        {

            if (pointsVal >= StarsThreshHolds[i] && StarsThreshHolds[i] != -1)
            {
                Stars++;
                StarsThreshHolds[i] = -1;
            }
        }


        Moves--;
        Points += (int)pointsVal;

        UpdatePhrases(Word, out HashSet<char> chars);

        SpawnFlyTiles(chars, out bool CouldSpawn);
        UseGoldenTiles();


        //StartCoroutine(nameof(SpawnCour));

        if (CouldSpawn) StartCoroutine(SpawnGoldenTile());

        CheckFor3LetterWords();

        foreach (var tile in Tiles)
            if (tile.type == Type.Disabled)
                tile.StartActivationCouroutine(0);

        UpdateUI();
    }


    void SpawnFlyTiles(HashSet<char> chars, out bool CouldSpawn)
    {
        CouldSpawn = false;
        int num = 0;
        foreach (var tile in Tiles)
        {
            if (tile.IsSelected() && chars.Contains(tile.Letter))
            {
                //chars.Count(c => c == tile.character);
                if (SpawnFlyTile(tile, num))
                {
                    num++;
                    CouldSpawn = true;
                    chars.Remove(tile.Letter);
                }
            }
        }
    }


    //Misc Functions
    void UseGoldenTiles()
    {
        List<GoldenTileMeta> GoldenTiles = new();
        foreach (var item in Tiles)
            if (item.IsSelected() && item.type == Type.Golden)
            {
                GoldenTiles.Add(new GoldenTileMeta(item.transform.position, item.Letter));
            }
        foreach (var item in Tiles)
        {
            foreach (var item1 in GoldenTiles)
            {
                if (item.Letter == item1.character)
                {
                    Points += item.GetPoints(); UIManager.PointsProgress.Points(Points);

                    //Debug.Log(item.character);
                    item1.didFunction++;
                    item.StartActivationCouroutine(.5f);
                }
            }

            if (item.IsSelected())
            {
                item.Deselect();
                item.StartActivationCouroutine(0);
            }
        }

        foreach (var item in GoldenTiles)
            if (item.didFunction <= 1)
                RayCast.RectBoom(item.position, 1);

    }
    async void CheckFor3LetterWords()
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
                            string s = Tiles[x].Letter + "" + Tiles[y].Letter + "" + Tiles[z].Letter;
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
                item.StartActivationCouroutine(0);

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

        UpdateDisplayTile(false);
    }



    //Gameobject Functions
    void InstantiatePrefab(Vector3 position, int X, int Y)
    {
        GameObject instantiatedPrefab = Instantiate(prefab, transform);
        instantiatedPrefab.transform.SetLocalPositionAndRotation(position, Quaternion.identity);
        instantiatedPrefab.transform.localScale = Vector3.one;
        instantiatedPrefab.GetComponent<TileLetter>().SetCoords(X,Y);
    }
    void GenerateGameObjects()
    {
        float scale = 4f;
        RemoveGameObjects();
        for (int y = height-1; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                InstantiatePrefab(new Vector3(
                ((x * transform.localScale.x * scale) + transform.position.x - ((width - 1) * transform.localScale.x * scale / 2)) * (.85f + Spacing),
                (y * transform.localScale.y * scale) ,
                5), x,height-y-1 );
            }
        }
    }
    void RemoveGameObjects()
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