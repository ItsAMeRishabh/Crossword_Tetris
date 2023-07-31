using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public float Percentage = .0f;
    public float Speed = .02f;

    float fill = 0f;

    //List<float> Stars;

    public RectTransform ProgressBarImage;
    public RectTransform ProgressBarBack;

    readonly List<RectTransform> StarTransform = new();
    public List<Image> StarImages;

    public Sprite StarActive;
    public Sprite StarInactive;

    int m;
    private void Start()
    {
        Percentage = 0;
    }

    public void SetStars(int[] StarThreshHolds)
    {
        m = StarThreshHolds.Max();

        for (int i = 0; i < StarImages.Count; i++)
        {
            StarTransform.Add(StarImages[i].GetComponent<RectTransform>());
            StarTransform[i].anchoredPosition = new Vector3(ProgressBarBack.sizeDelta.x * StarThreshHolds[i] / m, StarTransform[i].anchoredPosition.y, 0);
            StarImages[i].sprite = StarInactive;
        }
        ProgressBarImage.sizeDelta = new Vector2(0, ProgressBarImage.sizeDelta.y);
    }

    public void Points(int Points)
    {
        Percentage = (float)Points / m;
    }

    private void Update()
    {
        if (fill <= 1 && Mathf.Abs(fill - Percentage) >= Speed)
        {
            //if ()
            if (fill - Percentage > -Speed)
                fill -= Speed;

            if (fill - Percentage < Speed)
                fill += Speed;
            ProgressBarImage.sizeDelta = new Vector2(ProgressBarBack.sizeDelta.x * fill, ProgressBarImage.sizeDelta.y);

            for (int i = 0; i < StarImages.Count; i++)
            {
                if (fill >= StarTransform[i].anchoredPosition.x / ProgressBarBack.sizeDelta.x)
                    StarImages[i].sprite = StarActive;
                else
                    StarImages[i].sprite = StarInactive;
            }

        }
    }
}