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

public class TileGrid : MonoBehaviour
{
    public Transform TextParent;

    public SettingsData Settings;
    public TextMeshProUGUI OutputBox;
    public TextMeshProUGUI PointBox;

    [SerializeField]
    int MinUserWordSize = 3;
    [SerializeField]
    int InitialTileSpawn = 7;

    [Space(10)]

    [SerializeField]
    float InitialSpawnRate = 2f;
    [SerializeField]
    float SpawnRateChange = -0.05f;
    [SerializeField]
    float SpawnRate = 0f;
    [SerializeField]
    float CapRate = 0f;


    List<LetterTile> Tiles;
    List<TextMeshProUGUI> TextBoxes;
    WordSelector wordHandler;
    int points = 0;



    private void Start()
    {
        TextBoxes = new();
        Tiles = new();
        wordHandler = GetComponent<WordSelector>();


        SpawnRate = InitialSpawnRate;

        for (int i = 0; i < TextParent.childCount; i++) { 
            TextBoxes.Add(TextParent.GetChild(i).GetComponent<TextMeshProUGUI>());
            TextBoxes[i].text = "";
        }
        for (int i = 0; i < transform.childCount; i++)
            Tiles.Add(transform.GetChild(i).GetComponent<LetterTile>());

        StartCoroutine(LetterApplierTimer());
    }

    private IEnumerator LetterApplierTimer()
    {

        SetWords();
        for (int i = 0; i < InitialTileSpawn; i++)
        {
            yield return new WaitForSeconds(0.1f);
            if (!Spawn()) break;
        }
        yield return new WaitForSeconds(1f);

        while (true)
        {
            if (!Spawn()) break;

            SpawnRate = Math.Max(SpawnRate+ SpawnRateChange,CapRate);

            yield return new WaitForSeconds(SpawnRate);
        }

        Debug.Log("---LOSING SCREEN HERE---");
    }

    bool Spawn()
    {
        List<LetterTile> inactives = new();
        foreach (var item in Tiles)
            if (item.IsEmpty())
                inactives.Add(item);

        if (inactives.Count > 0)
            inactives[Random.Range(0, inactives.Count)].SetActive(GetCharacter());
        else return false;
        return true;
    }

    char GetCharacter()
    {
        if (wordHandler.ActiveWords.Count == 0)
            Debug.Log("---WIN SCREEN HERE---");

        string concat = "";

        foreach (var item in TextBoxes)
            concat += item.text;        

        return concat[Random.Range(0, concat.Length)];

    }

    public void SetWords()
    {
        foreach (var item in TextBoxes)
        {
            if(item.text == "")
                item.text = GetObjectiveWord();
        }
    }

    public string GetObjectiveWord()
    {
        if (wordHandler.ObjectiveWords.Count == 0)
            return "";
        int i = Random.Range(0, wordHandler.ObjectiveWords.Count);
        wordHandler.ActiveWords.Add(wordHandler.ObjectiveWords.ElementAt(i));
        wordHandler.ObjectiveWords.RemoveAt(i);
        return wordHandler.ActiveWords.ElementAt(wordHandler.ActiveWords.Count-1).word;
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


    public void CheckWord()
    {
        bool b = false;
        foreach (var item in TextBoxes)
            if(item.text == OutputBox.text && item.text != "")
            {
                Word w = new();

                for (int i = 0; i < wordHandler.ActiveWords.Count; i++)
                {
                    Debug.Log(wordHandler.ActiveWords.ElementAt(i).word + item.text);
                    if (wordHandler.ActiveWords.ElementAt(i).word.Equals(item.text))
                    {
                        w = wordHandler.ActiveWords.ElementAt(i);
                        wordHandler.ActiveWords.Remove(wordHandler.ActiveWords.ElementAt(i));
                        break;
                    }
                }

                b = true;
                points+= w.points;
                item.text = "";
                SetWords();
                break;
            }

        if (!b)
        {
            string word = OutputBox.text.ToUpper();
            b = (OutputBox.text.Length >= MinUserWordSize && wordHandler.IsWord(word));
            if (b)
            {
                foreach (var item in Settings.LetterPoints)
                {
                    foreach (char c in word)
                    {
                        if(item.word.Equals(c+""))
                        {
                            points += item.points;
                        }
                    }
                }
            }

        }
            
        if(b){
            foreach (var item in Tiles)
                if (item.IsSelected())
                    item.SetInactive();

            OutputBox.text = "";
            PointBox.text = "Points : " + points;
        }
    }


    [Space(50)]
    public GameObject prefab;
    public int width;
    public int height;

    internal void GenerateGameObjects()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Instantiate(prefab, new Vector3((1.75f * x) * transform.localScale.x, ((2 * y) + (x % 2)) * transform.localScale.x, 5), Quaternion.identity, transform);
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
