using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;



[RequireComponent(typeof(WordSelector))]

public class TileGrid : MonoBehaviour
{
    public SettingsData Settings;


    public UIManager UIManager;

    readonly List<LetterTile> Tiles = new();

    public string[] InitialSpawns;
    public int[] StarWordRequires;

    public string ObjectivePhrase;
    string DisplayPhrase;
    int WordsUsed = 0;


    [Space(10)]
    [SerializeField]
    int MinUserWordSize = 3;

    [SerializeField]
    float ChanceOfRandomWords = 40f;
    [SerializeField]
    float ChanceOf2X = 10f;

    public WordSelector wordHandler;

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
                        if (wordHandler.IsWord(s))
                        {
                            Debug.Log(s);
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
        wordHandler = GetComponent<WordSelector>();
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
        

        UIManager.InputBox.text = DisplayPhrase;



        for (int i = 0; i < transform.childCount; i++)
            Tiles.Add(transform.GetChild(i).GetComponent<LetterTile>());


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
                {
                    tile.SetActive(InitialSpawns[y][x].ToString().ToUpper()[0], mult);
                }
                else
                    tile.SetActive(GetCharacter(), mult);
            }
            i++;
        }
        Initial = false;
        //SpawnInitial();
        CheckFor3LetterWords();
    }

    // IEnumerator SpawnCour()
    // {

    //     yield return new WaitForSecondsRealtime(0.3f);

    //     Spawn();
    // }

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
        
        if (UIManager.OutputBox.text.Length >= MinUserWordSize && wordHandler.IsWord(word))
            MakeWord(pointsVal,word);
    }

    public void MakeWord(float pointsVal, string word)
    {
        WordsUsed++;

        foreach (var item in Tiles)
            if (item.IsSelected())
            {
                item.Deselect();
                item.SetInactive();
            }

        points += (int)pointsVal;
        UIManager.OutputBox.text = "";
        UIManager.PointBox.text = "Points : " + points;


        for (int i = 0; i < ObjectivePhrase.Length; i++)
        {
            for (int j = 0; j < word.Length; j++)
            {
                if (ObjectivePhrase[i].Equals(word[j]))
                {
                    DisplayPhrase = DisplayPhrase.Remove(i, 1).Insert(i, ObjectivePhrase[i] + "");
                    ObjectivePhrase = ObjectivePhrase.Remove(i, 1).Insert(i, " ");
                }
            }
        }

        UIManager.InputBox.text = DisplayPhrase;
        if (ObjectivePhrase.Trim() == "")
            Debug.Log("---WINNING SCREEN---");
    
        int stars =0;
        foreach (var item in StarWordRequires)
        {
            if(WordsUsed <= item)
            {
                stars++;
            }
        }

        Debug.Log(stars + " stars");
        UIManager.WordsUsedBox.text = WordsUsed + " Words";

        //StartCoroutine(nameof(SpawnCour));
        Spawn();
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
