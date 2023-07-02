using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;
using System.Collections;

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
public class TileGrid : MonoBehaviour
{
    public SettingsData Settings;


    public UIManager UIManager;

    readonly List<TileLetter> Tiles = new();

    public string[] InitialSpawns;
    public int[] StarWordRequires;

    public string ObjectivePhrase;
    public string DisplayPhrase;
    int WordsUsed = 0;


    [Space(10)]
    [SerializeField]
    int MinUserWordSize = 3;

    [SerializeField]
    float ChanceOfRandomWords = 40f;
    [SerializeField]
    float ChanceOf2X = 10f;

    [NonSerialized]
    public WordSelector WordHandler;
    public TileDisplay InputBox;
    public GameObjectPool TilePool;
    int points = 0;
    bool Initial = true;



    private void CheckFor3LetterWords()
    {
        for (int x = 0; x < Tiles.Count; x++)
        {
            for (int y = 0; y < Tiles.Count; y++)
            {
                for (int z = 0; z < Tiles.Count; z++)
                {
                    if(x!=y && y!=z && z != x)
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

        foreach (var Tile in Tiles){
            Tile.SetInactive();
        }
        Spawn();
    }

    public void GMAwake()
    {
        WordHandler = GetComponent<WordSelector>();
    }

    public void GMStart()
    {
        Array.Reverse(InitialSpawns);

        ObjectivePhrase = ObjectivePhrase.ToUpper();
        
        for (int i = 0; i < ObjectivePhrase.Length; i++)
            if (ObjectivePhrase[i] == ' ')            
                DisplayPhrase += " ";
            else
                DisplayPhrase += "_";
        

        InputBox.Display(DisplayPhrase);



        for (int i = 0; i < transform.childCount; i++)
            Tiles.Add(transform.GetChild(i).GetComponent<TileLetter>());


        //StartCoroutine(nameof(SpawnCour));
        Spawn();
    }

    void Spawn()
    {
        //UIManager.DebugText.text = "Spawn Invoked!";
        int i = 0;
        int j = 0;
        foreach (var tile in Tiles)
        {
            if (tile.IsEmpty())
            {
                j++;
                float mult = 1;
                if (Random.Range(0, 100) < ChanceOf2X)
                    mult = 2;

                int x = (int)Mathf.Floor(i / width);
                int y = (i % width);
                if (Initial && (InitialSpawns.Length >y && InitialSpawns[y].Length > x))
                    tile.SetActive(InitialSpawns[y][x].ToString().ToUpper()[0], mult);
                else
                    tile.SetActive(GetCharacter(), mult);
            }
            i++;
        }
        Initial = false;
        //SpawnInitial();
        CheckFor3LetterWords();
    }

    void SpawnGoldenTile()
    {
        Dictionary<char, int> tiles = new();
        foreach (var tile in Tiles)
        {
            tiles.TryAdd(tile.character, 0);
            tiles[tile.character]++;
        }
        //LogDictionary<char>(tiles);
        char m = tiles.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
        List<TileLetter> list = new ();
        foreach (var tile in Tiles)
            if(tile.character == m)
                list.Add(tile);


        list[Random.Range(0, list.Count)].SetGolden();
        


    }


    void LogDictionary<T>(Dictionary<T, int> dictionary)
    {
        List<KeyValuePair<T, int>> sortedElements = new (dictionary);

        // Sort the elements based on their counts in descending order
        sortedElements.Sort((x, y) => y.Value.CompareTo(x.Value));

        // Log the elements and their counts
        foreach (var kvp in sortedElements)
        {
            Debug.Log($"{kvp.Key}: {kvp.Value} times");
        }
    }


    char GetCharacter()
    {
        if (Random.Range(0, 100) < ChanceOfRandomWords)
        {
            return Settings.GetAlphabet()[Random.Range(0, 26)];
        }
        else
        {
            string str = ObjectivePhrase.Replace(" ", "");
            if(str.Length == 0)
            {
                return Settings.GetAlphabet()[Random.Range(0, 26)];
            }
            return str[Random.Range(0, str.Length)];
        }
    }



    public int AddToOutput(char c)
    {
        UIManager.OutputBox.text += c;
        return UIManager.OutputBox.text.Length - 1;
    }
    public void RemoveFromOutput(int i)
    {
        foreach (var tile in Tiles)
            if (tile.GetSelectedIndex() >= i)
                tile.SetSelectedIndex(tile.GetSelectedIndex() - 1);

        UIManager.OutputBox.text = UIManager.OutputBox.text.Remove(i,1);
    }


    public void SpawnFlyTile(char character,Vector3 position, Tile destination )
    {
        var fly = TilePool.Pool.Get();// Instantiate(FlyTile, item.transform.position, Quaternion.identity);
        fly.transform.SetPositionAndRotation(position, Quaternion.identity);
        fly.GetComponent<FlyTile>().Destination = destination;
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
        WordsUsed++;
        points += (int)pointsVal;
        

        HashSet<char> chars = new();
        for (int i = 0; i < ObjectivePhrase.Length; i++)
        {
            for (int j = 0; j < word.Length; j++)
            {
                if (ObjectivePhrase[i].Equals(word[j]))
                {
                    chars.Add(word[j]);
                    DisplayPhrase = DisplayPhrase.Remove(i, 1).Insert(i, ObjectivePhrase[i] + "");
                    ObjectivePhrase = ObjectivePhrase.Remove(i, 1).Insert(i, " ");
                }
            }
        }

        InputBox.Display(DisplayPhrase);
        bool b = false;
        for (int i = 0; i < InputBox.transform.childCount; i++)
        {
            foreach (var item in Tiles)
            {
                if (item.IsSelected())
                {
                    if (chars.Contains(item.character))
                    {
                        var tile = InputBox.transform.GetChild(i).GetComponent<Tile>();
                        if (tile.character == item.character)
                        {
                            b = true;
                            SpawnFlyTile(item.character, item.transform.position, tile);
                            break;
                        }
                    }

                }
            }
        }

        List<GoldenTileMeta> GoldenTiles = new();
        foreach (var item in Tiles)
            if (item.IsSelected() && item.IsGolden()) {
                GoldenTiles.Add(new GoldenTileMeta(item.transform.position, item.character));
            }

        foreach (var item in Tiles)
        {
            foreach (var item1 in GoldenTiles)
            {
                if(item.character == item1.character)
                {
                    //Debug.Log(item.character);
                    item1.didFunction++;
                    item.SetInactive();
                }
            }

            if (item.IsSelected())
            {
                item.Deselect();
                item.SetInactive();
            }
        }

        foreach(var item in GoldenTiles)
        {
            if (item.didFunction <= 1)
            {
                Debug.Log("Boom");
                PowerUp.RectBoom(item.position, 1);
                //StartCoroutine(PowerUp.RectBoomCour(item.position,1));

                //PowerUp.DeathBoomRay(item.transform.position, Vector2.left, 1);
                //PowerUp.DeathBoomRay(item.transform.position, Vector2.right, 1);
                //PowerUp.DeathBoomRay(item.transform.position, Vector2.up, 1);
                //PowerUp.DeathBoomRay(item.transform.position, Vector2.down, 1);
            }
        }


        UIManager.OutputBox.text = "";
        UIManager.PointBox.text = "Points : " + points;
        UIManager.WordsUsedBox.text = WordsUsed + " Words";

        //StartCoroutine(nameof(SpawnCour));
        if (ObjectivePhrase.Trim() == "")
        {
            Debug.Log("---WINNING SCREEN---");

            int stars = 0;
            foreach (var item in StarWordRequires)
            {
                if (WordsUsed <= item)
                {
                    stars++;
                }
            }

            Debug.Log(stars + " stars");

        }
        Spawn();
        if(b)SpawnGoldenTile();
    }


    [Space(50)]
    public GameObject prefab;
    public int width;
    public int height;
    public enum SpawnMode { HEX,RECT}
    public SpawnMode spawnMode;

    internal void GenerateGameObjects()
    {
        RemoveGameObjects();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (spawnMode == SpawnMode.HEX)
                {
                    Instantiate(prefab, new Vector3(
                        ((1.75f * x) * transform.localScale.x) + transform.position.x,
                        (((2 * y) + (x % 2)) * transform.localScale.x) + transform.position.y,
                        5), Quaternion.identity, transform);

                }
                else
                {
                    Instantiate(prefab, new Vector3(
                    (2*x * transform.localScale.x) + transform.position.x,
                    (2*y * transform.localScale.x) + transform.position.y,
                    5), Quaternion.identity, transform);

                }
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
