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

[RequireComponent(typeof(GameObjectPool))]

public class TileGrid : MonoBehaviour
{
    int Points = 0;
    int Moves;
    bool HasWon = false;

    readonly List<TileLetter> Tiles = new();

    [NonSerialized]
    public Array2DString InitialSpawns;
    [NonSerialized]
    public int[] StarsThreshHolds;
    [NonSerialized]
    public List<int> TilesNeeded = new();
    [NonSerialized]
    public List<int> Suggestions = new();



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
            if (_unstable == 0)
            {
                StartCoroutine(Win());
                StartCoroutine(Lose());
                CheckForIdleWords();

            }
        }
    }
    
    public bool Ended = false;

    string ObjectiveQuestion = "Something witty here... idk";
    string ObjectivePhrase;
    string DisplayPhrase;
    readonly Dictionary<char, int> ObjectiveFreq = new();
    int Freq = 0;


    int width;
    int height;
    readonly int MinUserWordSize = 3;

    [NonSerialized]
    public int Coins = 0;
    [NonSerialized]
    public int Gems = 0;

    [NonSerialized]
    public float ChanceOfGem;
    [NonSerialized]
    public float ChanceOfCoin;

    public GameObjectPool TilePool;
    public PowerUpManager PowerUpManager;


    [Space(10)]
    public GameObject Tile;
    public LevelHandler LevelHandler;
    public Theme Theme;
    public LanguagePack Lang;
    public UIManager UIManager;

    [Space(20)]
    [Header("Visual Spacings")]
    public float Spacing = 0;
    public float bgoffset;
    public float bgscale;

    [Header("Timings")]
    public float FlyTileSpawnDelay ;
    public float InitialWait;
    public float SuggestionDelay;
    public float VowelConstant = .3f;

    readonly float InitialSuggestTime = 7f; // 4 seconds
    readonly float LoopSuggestTime = 4f;

    float interactionTimer = 0f;
    float interactionTimer2 = 0f;
    //Base Functions

    private void Update()
    {
        UIManager.FPSCount.text = "FPS : " + (1 / Time.deltaTime).ToString("0");

        if (Input.GetMouseButtonDown(0))
            interactionTimer = 0f;

        interactionTimer += Time.deltaTime;
        interactionTimer2 += Time.deltaTime;

        if (interactionTimer >= InitialSuggestTime && interactionTimer2 >= LoopSuggestTime)
        {
            interactionTimer2 = 0f;
            StartCoroutine(MakeSuggestion());
        }
    }


    public void Awake()
    {
        Level level = LevelHandler.Levels[LevelHandler.Current];

        ObjectiveQuestion = level.ObjectiveQuestion;
        ObjectivePhrase = level.ObjectivePhrase.ToUpper();

        foreach(char c in ObjectivePhrase)
            if(ObjectiveFreq.ContainsKey(c))
                ObjectiveFreq[c] = 2;
            else
                ObjectiveFreq.Add(c, 1);

        foreach (var item in ObjectiveFreq)
            Freq += item.Value;


        Moves = level.Moves;
        width = level.InitalLetter.GridSize.x;
        height = level.InitalLetter.GridSize.y;
        ChanceOfCoin = level.ChanceOfCoin;
        ChanceOfGem = level.ChanceOfGem;

        InitialSpawns = level.InitalLetter;
        UIManager.TileGridBG.sizeDelta = new Vector2(bgoffset + (width * 131 * bgscale), bgoffset + (height * 136 * bgscale));

        StarsThreshHolds = (int[])level.StarsThreshHolds.Clone();
        UIManager.PointsProgress.SetStars(StarsThreshHolds);

        for (int i = 0; i < width; i++)
            TilesNeeded.Add(height);


        //Generate the grid
        GenerateGameObjects();
    }
    public void Start()
    {
        //Set Display Phrase
        for (int i = 0; i < ObjectivePhrase.Length; i++)
        {
            if (!LanguagePack.IsAlpha(ObjectivePhrase[i]))
                DisplayPhrase += ObjectivePhrase[i];
            else
                DisplayPhrase += "_";
        }


        UpdateDisplayTile(true);
        DisplayPhrase = DisplayPhrase.Replace("#", "");
        ObjectivePhrase = ObjectivePhrase.Replace("#", "");
        
        UpdateUI();
        UIManager.QuestionBox.text = ObjectiveQuestion;

        for (int i = 0; i < transform.childCount; i++)
            Tiles.Add(transform.GetChild(i).GetComponent<TileLetter>());

        StartCoroutine(ShowObjective());
    }



    IEnumerator Win()
    {
        if (ObjectivePhrase.Trim().Length == 0)
        {
            yield return new WaitForSeconds(1);
            UpdateUI();
            if (LevelHandler.CompletedLevels <= LevelHandler.Current)
                LevelHandler.SetCompletedLevels(LevelHandler.Current + 1);
            //LevelHandler.CompletedLevels = 
            HasWon = true;
            int Stars = 0;

            foreach (var threshold in StarsThreshHolds)
            {
                if (Points >= threshold)
                    Stars++;
            }

            Debug.Log("---WINNING SCREEN---");
            Debug.Log("Points: " + Points);
            Debug.Log("Stars: " + Stars);
            //Time.timeScale = 0;
            Ended = true;
            UIManager.WinPanel.Win(DisplayPhrase, Stars, Points, Coins, Gems);
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
        yield return new WaitForSeconds(InitialWait);

        foreach (var tile in Tiles)
            tile.Init(this);

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
    IEnumerator MakeSuggestion()
    {
        foreach(int i in Suggestions)
        {
            yield return new WaitForSeconds(SuggestionDelay);
            Tiles[i].Suggest();
        }
    }




    public bool SpawnFlyTile(TileLetter tile, int offset)
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
    void SpawnFlyTiles(List<char> chars)
    {
        //CouldSpawn = false;
        int num = 0;
        foreach (var tile in Tiles)
        {
            if (tile.IsSelected() && chars.Contains(tile.Letter))
            {
                //chars.Count(c => c == tile.character);
                if (SpawnFlyTile(tile, num))
                {
                    num++;
                    //CouldSpawn = true;
                    chars.Remove(tile.Letter);
                }
            }
        }
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
    void UpdateUI()
    {

        UIManager.PointsProgress.Points(Points);
        UIManager.OutputBox.text = "";
        UIManager.PointBox.text = "Points : " + Points;
        UIManager.MovesUsedBox.text = Moves + "";
        UIManager.Coins.text = (Coins + LevelHandler.Coins) + "";
        UIManager.Gems.text = (Gems + LevelHandler.Gems) + " Gems";
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




    public void CheckTextBoxes()
    {
        //for(int i = 0; i < ObjectiveFreq.Count; i++)
        //{
        //    Debug.Log(ObjectiveFreq.ElementAt(i).Key + " : " + ObjectiveFreq.ElementAt(i).Value);
        //}

        string Word = UIManager.OutputBox.text.ToUpper();
        float pointsVal = UpdatePoints();


        DateTime t = DateTime.Now;
        if (!Lang.IsWord(Word))
        {

            UIManager.TextField.ResetTrigger("Incorrect");
            UIManager.TextField.SetTrigger("Incorrect");
            Debug.LogError(Word + " is not a word!");
            return;
        }
        if (!(UIManager.OutputBox.text.Length >= MinUserWordSize))
        {
            Debug.Log("Time taken : " + (DateTime.Now - t).TotalMilliseconds + "ms");
            UIManager.TextField.ResetTrigger("Incorrect");
            UIManager.TextField.SetTrigger("Incorrect");
            Debug.LogError("Word should be longer than " + MinUserWordSize);
            return;
        }

        UIManager.TextField.ResetTrigger("Correct");
        UIManager.TextField.SetTrigger("Correct");

        Moves--;
        Points += (int)pointsVal;

        
        SpawnFlyTiles(UpdatePhrases(Word));//cudapwn

        //if (CouldSpawn)
        //    consecutiveNoReveals = 0;
        //else
        //    consecutiveNoReveals++;
        //UseGoldenTiles();
        //if (CouldSpawn) StartCoroutine(SpawnGoldenTile());



        foreach (var tile in Tiles)
            if (tile.type == Type.Disabled)
                tile.StartActivationCouroutine(0);

        foreach (var item in Tiles)
        {
            if (item.IsSelected())
            {
                item.Deselect();
                item.StartActivationCouroutine(0);
            }
        }

        UpdateUI();
    }
    
    async void CheckForIdleWords()
    {
        await System.Threading.Tasks.Task.Run(() =>
        {

            Suggestions.Clear();

            for (int w = 0; w < Tiles.Count; w++)
                for (int x = 0; x < Tiles.Count; x++)
                    for (int y = 0; y < Tiles.Count; y++)
                        for (int z = 0; z < Tiles.Count; z++)
                            if (w != x && x != y && y != z && z != w && z != x && w != y)
                            {
                                string s = Tiles[w].Letter + "" + Tiles[x].Letter + "" + Tiles[y].Letter + "" + Tiles[z].Letter;
                                if (Lang.IsWord(s))
                                {
                                    Suggestions.Add(w);
                                    Suggestions.Add(x);
                                    Suggestions.Add(y);    
                                    Suggestions.Add(z);

                                    Debug.LogWarning(s);
                                    return;
                                }
                            }


            for (int x = 0; x < Tiles.Count; x++)
                for (int y = 0; y < Tiles.Count; y++)
                    for (int z = 0; z < Tiles.Count; z++)
                        if (x != y && y != z && z != x)
                        {
                            string s = Tiles[x].Letter + "" + Tiles[y].Letter + "" + Tiles[z].Letter;
                            if (Lang.IsWord(s))
                            {
                                Suggestions.Add(x);
                                Suggestions.Add(y);
                                Suggestions.Add(z);

                                Debug.LogWarning(s);
                                return;
                            }
                        }



            Debug.LogError("BAD SITUATION");

            //foreach (var item in Tiles)
            //{
            //    item.StartActivationCouroutine(0);

            //}
        });
    }
    
    
    
    public char GetCharacter()
    {
        int t = Tiles.Count;
        int v = 0;

        foreach (var item in Tiles)
        {
            if(item.Letter == 'A' || item.Letter == 'E' || item.Letter == 'I' || item.Letter == 'O' || item.Letter == 'U')
            {
                v++;
            }
        }

        float s = 1.4f;
        if(Random.Range(1,Lang.totalWeight + (Freq * s)) > Lang.totalWeight)
        {
            int r = Random.Range(0, Freq);

            foreach (var item in ObjectiveFreq)
            {
                r -= item.Value;
                if (r <= 0)
                {
                    char ch = item.Key;
                    ObjectiveFreq[item.Key]--;
                    if (ObjectiveFreq[item.Key] == 0)
                        ObjectiveFreq.Remove(item.Key);
                    return ch;
                }
            }
        }

        return Lang.GetRandomChar((float)v / t < VowelConstant);
    }

    public List<char> UpdatePhrases(string Word)
    {
        HashSet<char> chars = new();
        for (int i = 0; i < ObjectivePhrase.Length; i++)
            for (int j = 0; j < Word.Length; j++)
                if (ObjectivePhrase[i].Equals(Word[j]))
                {
                    chars.Add(Word[j]);
                    DisplayPhrase = DisplayPhrase.Remove(i, 1).Insert(i, ObjectivePhrase[i] + "");
                    ObjectivePhrase = ObjectivePhrase.Remove(i, 1).Insert(i, " ");
                }

        UpdateDisplayTile(false);
        return chars.ToList();
    }


    //Gameobject Functions
    void InstantiatePrefab(Vector3 position, int X, int Y)
    {
        GameObject instantiatedPrefab = Instantiate(Tile, transform);
        instantiatedPrefab.transform.SetLocalPositionAndRotation(position, Quaternion.identity);
        instantiatedPrefab.transform.localScale = Vector3.one;
        instantiatedPrefab.GetComponent<TileLetter>().SetCoords(X, Y);
    }
    void GenerateGameObjects()
    {
        float scale = 4f;
        RemoveGameObjects();
        for (int y = height - 1; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                InstantiatePrefab(new Vector3(
                ((x * transform.localScale.x * scale) + transform.position.x - ((width - 1) * transform.localScale.x * scale / 2)) * (.85f + Spacing),
                (y * transform.localScale.y * scale),
                5), x, height - y - 1);
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


































































//Spawn Functions
//IEnumerator SpawnGoldenTile()
//{
//    yield return new WaitForSeconds(1.5f);

//    //Get all tile characters
//    Dictionary<char, int> tiles = new();
//    foreach (var tile in Tiles)
//    {
//        if (tile.type == Type.Normal)
//        {
//            tiles.TryAdd(tile.Letter, 0);
//            tiles[tile.Letter]++;
//        }
//    }

//    //Get most common character
//    if (tiles.Count > 0)
//    {
//        char m = tiles.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;

//        //Get all tiles with that character
//        List<TileLetter> list = new();
//        foreach (var tile in Tiles)
//            if (tile.Letter == m)
//                list.Add(tile);


//        //Set a random tile to golden
//        var g  = list[Random.Range(0, list.Count)];
//        g.type = Type.Golden;
//        g.UpdateVisual();
//    }
//}







//Misc Functions
//void UseGoldenTiles()
//{
//List<GoldenTileMeta> GoldenTiles = new();
//foreach (var item in Tiles)
//    if (item.IsSelected() && item.type == Type.Golden)
//    {
//        GoldenTiles.Add(new GoldenTileMeta(item.transform.position, item.Letter));
//    }
//foreach (var item in Tiles)
//{
//foreach (var item1 in GoldenTiles)
//{
//    if (item.Letter == item1.character)
//    {
//        Points += item.GetPoints(); 
//        UIManager.PointsProgress.Points(Points);

//        //Debug.Log(item.character);
//        item1.didFunction++;
//        item.StartActivationCouroutine(.5f);
//    }
//}

//if (item.IsSelected())
//{
//    item.Deselect();
//    item.StartActivationCouroutine(0);
//}
//}

//foreach (var item in GoldenTiles)
//    if (item.didFunction <= 1)
//        RayCast.RectBoom(item.position, 1);

//}
