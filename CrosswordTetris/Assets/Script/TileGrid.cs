using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;
using Random = UnityEngine.Random;



[RequireComponent(typeof(WordSelector))]
[RequireComponent(typeof(BombPowerUp))]

public class TileGrid : MonoBehaviour
{
    public SettingsData Settings;


    public TextMeshProUGUI OutputBox;
    public TextMeshProUGUI InputBox;
    public TextMeshProUGUI PointBox;

    List<LetterTile> Tiles;

    public string[] InitialSpawns;

    public string ObjectivePhrase;
    public string DisplayPhrase;


    [Space(10)]
    [SerializeField]
    int MinUserWordSize = 3;

    [SerializeField]
    float ChanceOfRandomWords = 40f;
    [SerializeField]
    float ChanceOf2X = 10f;

    WordSelector wordHandler;

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
        _Spawn();
    }
    
    
    private void Start()
    {
        ObjectivePhrase = ObjectivePhrase.ToUpper();

        for (int i = 0; i < ObjectivePhrase.Length; i++)
        {
            if (ObjectivePhrase[i] == ' ')            
                DisplayPhrase += " ";
            else
                DisplayPhrase += "_";
        }

        InputBox.text = DisplayPhrase;


        Tiles = new();
        wordHandler = GetComponent<WordSelector>();

        for (int i = 0; i < transform.childCount; i++)
            Tiles.Add(transform.GetChild(i).GetComponent<LetterTile>());

        SpawnInitial();

        StartCoroutine(nameof(Spawn));
    }
    void SpawnInitial()
    {
        for (int x = 0; x < InitialSpawns.Length; x++)
        {
            for (int y = 0; y < InitialSpawns[x].Length; y++)
            {
                int i = x % width + ((int)Math.Floor((decimal)y / width) * width);
                Tiles[i].SetActive(InitialSpawns[x][y],1);
            }
        }
    }
    void _Spawn()
    {
        foreach (var tile in Tiles)
        {
            if (tile.IsEmpty())
            {
                float mult = 1;
                if (Random.Range(0, 100) < ChanceOf2X)
                    mult = 2;

                tile.SetActive(GetCharacter(), mult);
            }
        }
        CheckFor3LetterWords();
    }
    IEnumerator Spawn()
    {

        yield return new WaitForSeconds(0.3f);

        _Spawn();
    }

    char GetRandomCharacter()
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

    char GetCharacter()
    {
        return GetRandomCharacter();
    }

    public int AddToOutput(char c)
    {
        OutputBox.text += c;
        return OutputBox.text.Length - 1;
    }
    public void RemoveFromOutput(int i)
    {
        foreach (var tile in Tiles)
            if (tile.GetSelectedIndex() >= i)
                tile.SetSelectedIndex(tile.GetSelectedIndex() - 1);

        OutputBox.text = OutputBox.text.Remove(i,1);
    }



    public void ResetOutput()
    {
        foreach (var tile in Tiles)
            tile.Deselect();
    }
    public void CheckTextBoxes()
    {
        string word = OutputBox.text.ToUpper();
        
        float pointsVal = 0;
        

        
        foreach (var tile in Tiles)
        {
            if (tile.IsSelected())
            {
                pointsVal += tile.GetPoints();
            }
        }

        if (OutputBox.text.Length >= MinUserWordSize && wordHandler.IsWord(word))
        {

            if(word.Length >= 4)
            {
                GetComponent<BombPowerUp>().CanDo = true;
            }

            foreach (var item in Tiles)
                if (item.IsSelected())
                {
                    item.Deselect();
                    item.SetInactive();
                }

            points += (int)pointsVal;
            OutputBox.text = "";
            PointBox.text = "Points : " + points;


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

            InputBox.text = DisplayPhrase.Replace('#', '_');
            if(ObjectivePhrase.Trim() == "")
                Debug.Log("---WINNING SCREEN---");
            StartCoroutine(nameof(Spawn));
        }


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
