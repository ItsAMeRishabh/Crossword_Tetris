using UnityEngine;

public class TileDisplay : MonoBehaviour
{
    public GameObject characterTilePrefab;
    public float lineGap = 0.4f;
    public float gap = .5f;
    public float space = .4f;


    public void Display(string sentence, bool activate)
    {
        if (transform.childCount == 0)
        {
            string[] lines = sentence.Split('#');
            for (int i = 0; i < lines.Length; i++)
            {
                DisplayLine(lines[i], i);
            }
        }
        else
        {
            //string sentence1 = sentence.Replace("#", "");
            for (int i = 0; i < sentence.Length; i++)
            {
                Tile t = transform.GetChild(i).GetComponent<Tile>();
                if (sentence[i] != '_')
                    t.SetCharacter(sentence[i]);

                if (activate)
                    t.Activate();
            }
        }
    }

    public void DisplayLine(string line, int y)
    {
        //string[] words = line.Replace(" ", ". ").Split('.');
        GameObject[] List = new GameObject[line.Length];
        float x= 0;
        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            GameObject cgo = Instantiate(characterTilePrefab, transform);

            cgo.transform.localPosition = new Vector3(x, -y * lineGap, 0);

            if (AddCharacter(c, cgo.GetComponent<Tile>()))
                x += gap;
            else
                x += space;

            List[i] = cgo;
        }

        AdjustLineTilePositions(List);
    }
     
    private bool AddCharacter(char c, Tile t)
    {
        if (c != '_')
            t.SetCharacter(c);
        if (c == ' ')
            t.gameObject.SetActive(false);

        return c != ' ';
    }
    private void AdjustLineTilePositions(GameObject[] line)
    {
        float max = 0;
        float min = float.MaxValue;
        
        foreach (GameObject child in line)
            if (child.activeSelf)
            {
                if (child.transform.localPosition.x > max)
                    max = child.transform.localPosition.x;
                if (child.transform.localPosition.x < min)
                    min = child.transform.localPosition.x;
            }
        

        float lineLengthHalf = (-max+min) / 2f;
        for (int i = 0; i < line.Length; i++)
        {
            line[i].transform.localPosition = new Vector3(line[i].transform.localPosition.x + lineLengthHalf, line[i].transform.localPosition.y, line[i].transform.localPosition.z);
        }

    }

}