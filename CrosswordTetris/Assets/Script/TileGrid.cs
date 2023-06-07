using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(WordSelector))]

public class TileGrid : MonoBehaviour
{
    public Transform TextParent;
    List<TextMeshProUGUI> TextBoxes;
    public List<LetterTile> Tiles;

    public TextMeshProUGUI OutputBox;


    WordSelector wordHandler;
    
    private void Start()
    {
        TextBoxes = new();
        wordHandler = GetComponent<WordSelector>();

        if (TextBoxes.Count == 0)
        {
            for (int i = 0; i < TextParent.childCount; i++)
            {
                TextBoxes.Add(TextParent.GetChild(i).GetComponent<TextMeshProUGUI>());
                TextBoxes[i].text = wordHandler.GetRandomWord();
            }
        }

        for (int i = 0; i < transform.childCount; i++)
            Tiles.Add(transform.GetChild(i).GetComponent<LetterTile>());
    
        StartCoroutine(LetterApplierTimer());
    }
    private IEnumerator LetterApplierTimer()
    {
        yield return new WaitForSeconds(1f);
        while (true)
        {
            List<LetterTile> inactives = new();

            foreach (var item in Tiles)
                if (item.IsEmpty()) 
                    inactives.Add(item);

            if (inactives.Count > 0)
                inactives[Random.Range(0, inactives.Count)].SetActive(GetCharacter());
            
            else break;

            yield return new WaitForSeconds(2f);
        }

        Debug.Log("---LOSING SCREEN HERE---");
    }
    char GetCharacter()
    {
        string concat = "";

        foreach (var item in TextBoxes)
            concat += item.text;        

        return concat[Random.Range(0, concat.Length)];

    }


    public void AddOutput(char c)
    {
        OutputBox.text += c;
    }

    
    public void CheckWord()
    {
        if (wordHandler.IsWord(OutputBox.text.ToUpper()))
        {
            foreach (var item in Tiles)
            {
                if (item.IsSelected())
                {
                    item.SetInactive();
                }
            }

            OutputBox.text = "";
        }
    }



    public GameObject prefab;
    public int width;
    public int height;

    internal void GenerateGameObjects()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Instantiate(prefab, new Vector3((1.75f * x) * transform.localScale.x, ((2 * y) + (x % 2)) * transform.localScale.x, 0), Quaternion.identity, transform);
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
