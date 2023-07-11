using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;
using System.Collections;
using UnityEditor;
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
public class TileGrid : MonoBehaviour
{
    public SettingsData Settings;


    public UIManager UIManager;

    readonly List<TileLetter> Tiles = new();

    public string[] InitialSpawns;
    public int[] StarWordRequires;

    public string ObjectiveQuestion = "Something witty here... idk";
    public string ObjectivePhrase;
    [NonSerialized]
    public string DisplayPhrase;
    int WordsUsed = 0;


    [Space(10)]
    [SerializeField]
    int MinUserWordSize = 3;

    [SerializeField]
    float ChanceOfRandomWords = 40f;
    public float ChanceOf2X = 10f;

    [NonSerialized]
    public WordSelector WordHandler;
    public TileDisplay InputBox;
    public GameObjectPool TilePool;
    int points = 0;
    


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

        foreach (var item in Tiles)
        {
            item.StartInactiveCouroutine();
        }
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
            if (!isAlphaNumeric(ObjectivePhrase[i]))
                DisplayPhrase += ObjectivePhrase[i];
            else
                DisplayPhrase += "_";
        

        InputBox.Display(DisplayPhrase);

        UIManager.QuestionBox.text = ObjectiveQuestion;

        for (int i = 0; i < transform.childCount; i++)
            Tiles.Add(transform.GetChild(i).GetComponent<TileLetter>());



        //CheckFor3LetterWords();
        //StartCoroutine(nameof(SpawnCour));
    }

    private bool isAlphaNumeric(char v)
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
                    item.StartInactiveCouroutine();
                }
            }

            if (item.IsSelected())
            {
                item.Deselect();
                item.StartInactiveCouroutine();
            }
        }

        foreach(var item in GoldenTiles)
            if (item.didFunction <= 1)
                PowerUp.RectBoom(item.position, 1);

        UIManager.OutputBox.text = "";
        UIManager.PointBox.text = "Points : " + points;
        UIManager.WordsUsedBox.text = WordsUsed + " Words";

        //StartCoroutine(nameof(SpawnCour));
        if (ObjectivePhrase.Trim() == "")
        {
            Debug.Log("---WINNING SCREEN---");

            int stars = 0;
            foreach (var item in StarWordRequires)
                if (WordsUsed <= item)
                    stars++;

            Debug.Log(stars + " stars");

        }

        if(b)
            StartCoroutine( SpawnGoldenTile());

        CheckFor3LetterWords();
    }


    [Space(50)]
    public GameObject prefab;
    public int width;
    public int height;
    public enum SpawnMode { HEX,RECT}
    public SpawnMode spawnMode;

    private void InstantiatePrefab(Vector3 position)
    {
        GameObject instantiatedPrefab = PrefabUtility.InstantiatePrefab(prefab,transform) as GameObject;
        instantiatedPrefab.transform.localPosition = position;
        instantiatedPrefab.transform.localRotation = Quaternion.identity;
        instantiatedPrefab.transform.localScale = Vector3.one;
    }

    internal void GenerateGameObjects()
    {

        float scale = 4f;
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
                    InstantiatePrefab(new Vector3(
                    (x * transform.localScale.x * scale) + transform.position.x - ((width-1) * transform.localScale.x * scale/ 2),
                    (y * transform.localScale.y * scale) + transform.position.y,
                    5));
                    //Instantiate(prefab, new Vector3(
                    //(2*x * transform.localScale.x) + transform.position.x -(transform.localScale.x * (width-1)),
                    //(2*y * transform.localScale.x) + transform.position.y,
                    //5), Quaternion.identity, transform);

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
