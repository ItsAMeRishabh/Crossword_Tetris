using UnityEngine;

public class SlidingPanel : MonoBehaviour
{
    public RectTransform panel;
    public float slideSpeed = 500f;
    public bool isPanelOpen = false;

    private Vector2 closedPosition;
    private Vector2 openPosition;

    private void Start()
    {
        closedPosition = panel.anchoredPosition;
        openPosition = new Vector2(closedPosition.x + panel.rect.width, closedPosition.y);
        panel.anchoredPosition = closedPosition;
    }

    public void TogglePanel()
    {
        if (isPanelOpen)
            SlideOutPanel();
        else
            SlideInPanel();
    }

    public void SlideInPanel()
    {
        if (!isPanelOpen)
        {
            isPanelOpen = true;
            StopAllCoroutines();
            StartCoroutine(SlidePanelCoroutine(openPosition));
        }
    }

    public void SlideOutPanel()
    {
        if (isPanelOpen)
        {
            isPanelOpen = false;
            StopAllCoroutines();
            StartCoroutine(SlidePanelCoroutine(closedPosition));
        }
    }

    private System.Collections.IEnumerator SlidePanelCoroutine(Vector2 targetPosition)
    {
        while (panel.anchoredPosition != targetPosition)
        {
            panel.anchoredPosition = Vector2.MoveTowards(panel.anchoredPosition, targetPosition, slideSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
