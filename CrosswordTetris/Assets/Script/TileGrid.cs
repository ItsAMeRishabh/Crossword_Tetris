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
    public SettingsData Settings;


    public Transform TextParent;
    public Transform NextUpParent;
    public TextMeshProUGUI OutputBox;
    public TextMeshProUGUI PointBox;

    List<TextMeshProUGUI> TextBoxes = new();
    List<TextMeshProUGUI> NextUpBoxes = new();
    List<LetterTile> Tiles;

    [SerializeField]
    int MinUserWordSize = 3;
    [SerializeField]
    int InitialTileSpawn = 7;

    [Space(10)]

    [SerializeField]
    float ChanceOfRandomWords = 40f;
    [SerializeField]
    float InitialSpawnRate = 2f;
    [SerializeField]
    float SpawnRateChange = -0.05f;
    [SerializeField]
    float CapRate = 0f;

    float SpawnRate = 0f;

    WordSelector wordHandler;
    int points = 0;


    //[NonSerialized]
    public bool powerUpAvailible;
    //[NonSerialized]
    public bool powerUpInUse;

    public void UsePowerUp()
    {
        if (powerUpAvailible)
        {
            powerUpInUse = !powerUpInUse;

        }
        Debug.Log("Boom active : " + powerUpAvailible);
    }

    private void Start()
    {
        Tiles = new();
        wordHandler = GetComponent<WordSelector>();


        SpawnRate = InitialSpawnRate;

        for (int i = 0; i < TextParent.childCount; i++)
        {
            TextBoxes.Add(TextParent.GetChild(i).GetComponent<TextMeshProUGUI>());
            TextBoxes[i].text = "";
        }

        SetWords();

        //for (int i = NextUpParent.childCount - 1; i >= 0; i--)
        //{
            //}
            for (int i = 0; i < NextUpParent.childCount; i++)
            {
                NextUpBoxes.Add(NextUpParent.GetChild(i).GetComponent<TextMeshProUGUI>());
            NextUpBoxes[i].text = GetRandomCharacter() + "";
        }


        for (int i = 0; i < transform.childCount; i++)
            Tiles.Add(transform.GetChild(i).GetComponent<LetterTile>());

        StartCoroutine(LetterApplierTimer());
    }

    private IEnumerator LetterApplierTimer()
    {

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

    char GetRandomCharacter()
    {
        if (Random.Range(0, 100) < ChanceOfRandomWords)
        {
            return Settings.GetAlphabet()[Random.Range(0, 26)];
        }
        else
        {
            if (wordHandler.ActiveWords.Count == 0)
                Debug.Log("---WIN SCREEN HERE---");

            string concat = "";

            foreach (var item in TextBoxes)
                concat += item.text;

            return concat[Random.Range(0, concat.Length)];
        }
    }

    char GetCharacter()
    {
        char ret = NextUpBoxes[0].text[0];

        for (int i = 1; i < NextUpBoxes.Count; i++)
        {
            NextUpBoxes[i - 1].text = NextUpBoxes[i].text;
        }

        NextUpBoxes[NextUpBoxes.Count - 1].text = GetRandomCharacter()+"";

        return ret;
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



    public void ResetOutput()
    {
        foreach (var tile in Tiles)
            tile.Deselect();
    }
    public void CheckWord()
    {
        string word = OutputBox.text.ToUpper();
        bool b = false;
        float pointsVal = 0;
        float mult = 1;


        foreach (var item in TextBoxes)
            if (item.text == OutputBox.text && item.text != "")
            {
                Word w = new();

                for (int i = 0; i < wordHandler.ActiveWords.Count; i++)
                {
                    if (wordHandler.ActiveWords.ElementAt(i).word.Equals(item.text))
                    {
                        w = wordHandler.ActiveWords.ElementAt(i);
                        wordHandler.ActiveWords.Remove(wordHandler.ActiveWords.ElementAt(i));
                        break;
                    }
                }

                b = true;
                mult = w.value;
                item.text = "";
                SetWords();
                break;
            }

        if (!b)
            b = (OutputBox.text.Length >= MinUserWordSize && wordHandler.IsWord(word));
       
        foreach (var item in Settings.LetterPoints)
        {
            foreach (char c in word)
            {
                if (item.word.Equals(c + ""))
                {
                    pointsVal += item.value;
                }
            }
        }

        if (b){
            if(word.Length > 3)
            {
                Debug.Log("BOOM AVAILIBLE");
                powerUpAvailible = true;
            }

            foreach (var item in Tiles)
                if (item.IsSelected())
                    item.SetInactive();

            if(mult!=1)
            Debug.Log("Bonus : " + ((pointsVal*mult) - pointsVal));
            points += (int)(pointsVal * mult);
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
        RemoveGameObjects();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Instantiate(prefab, new Vector3(
                    ((1.75f * x) * transform.localScale.x)+transform.position.x,
                    (((2 * y) + (x % 2)) * transform.localScale.x)+transform.position.y ,
                    5), Quaternion.identity, transform);
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
