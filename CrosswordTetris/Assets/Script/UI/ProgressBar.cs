using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    public float Percentage = .0f;
    public float Speed = .02f;

    float fill = 0f;

    //List<float> Stars;

    public RectTransform ProgressBarImage;
    public RectTransform ProgressBarBack;

    public List<RectTransform> StarImages;
    int m;
    private void Start()
    {
        Percentage = 0;
    }

    public void SetStars(int[] StarThreshHolds)
    {
        m = StarThreshHolds.Max();

        for (int i = 0; i < StarImages.Count; i++)
            StarImages[i].anchoredPosition = new Vector3(ProgressBarBack.sizeDelta.x * StarThreshHolds[i] / m, StarImages[i].anchoredPosition.y, 0);

        ProgressBarImage.sizeDelta = new Vector2(0, ProgressBarImage.sizeDelta.y);
    }

    public void Points(int Points)
    {
        Percentage = (float)Points / m;
    }

    private void Update()
    {
        if (Percentage <= 1)
            if (Mathf.Abs(fill - Percentage) >= Speed)
            {
                if (fill - Percentage > -Speed)
                    fill -= Speed;

                if (fill - Percentage < Speed)
                    fill += Speed;
                ProgressBarImage.sizeDelta = new Vector2(ProgressBarBack.sizeDelta.x * fill, ProgressBarImage.sizeDelta.y);
            }
    }
}