using System.Collections.Generic;
using UnityEngine;

public class TileDisplay : MonoBehaviour
{
    public GameObject characterTilePrefab;
    public int maxLineLength = 10;
    public float spacing = 0.4f;
    public float gap = .5f;


    public void Display(string sentence)
    {
        string[] words = sentence.Replace(" ", ". ").Split('.');
        int currentLine = 0;
        int currentLineLength = 0;
        List<GameObject> lineTiles = new();

        if (transform.childCount == 0)
        {
            foreach (string word in words)
            {
                int wordLength = word.Length;

                if (currentLineLength + wordLength > maxLineLength)
                {
                    AdjustLineTilePositions(lineTiles, currentLineLength, currentLine);
                    lineTiles.Clear();

                    currentLine++;
                    currentLineLength = 0;
                }

                foreach (char character in word)
                {
                    GameObject tile = Instantiate(characterTilePrefab, transform);
                    AddCharacter(character, tile.GetComponent<Tile>());
                    lineTiles.Add(tile);

                    currentLineLength++;
                }

                if (currentLineLength < maxLineLength)
                {
                    currentLineLength++;
                }
            }

            AdjustLineTilePositions(lineTiles, currentLineLength, currentLine);

        }
        else
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                AddCharacter(sentence[i], transform.GetChild(i).GetComponent<Tile>());
            }

        }

        //string[] words = sentence.Replace(" "," .").Split('.');
    }
    private void AddCharacter(char c, Tile t)
    {
        if (c != '_')
            t.SetCharacter(c);
        if (c == ' ')
            t.gameObject.SetActive(false);

    }
    private void AdjustLineTilePositions(List<GameObject> lineTiles, int lineLength, int currentLine)
    {
        int totalTiles = lineTiles.Count;
        float halfLineWidth = lineLength / 2f;
        float startPos = -halfLineWidth + 0.5f;

        for (int i = 0; i < totalTiles; i++)
        {
            float xPos = startPos + i;
            lineTiles[i].transform.localPosition = new Vector3(xPos, -currentLine, 0f);
        }
    }
}