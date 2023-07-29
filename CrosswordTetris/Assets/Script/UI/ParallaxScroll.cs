using UnityEngine;
public class ParallaxScroll : MonoBehaviour
{
    public RectTransform[] parallaxLayers;
    public float parallaxFactor = 0.5f;
    public Vector2 previousScroll = Vector2.zero;
    
    
    public void OnScrollValueChanged(Vector2 scrollPosition)
    {
        float Delta = (-scrollPosition.y + previousScroll.y);
        int i = 1;
        foreach (RectTransform layer in parallaxLayers)
        {
            Vector2 newPosition = layer.anchoredPosition;
            newPosition.y += Delta * (parallaxFactor * i);
            layer.anchoredPosition = newPosition;
            //layer.anchoredPosition = new Vector2(scrollPosition.x * parallaxFactor, scrollPosition.y * parallaxFactor);
            i++;
        }

        previousScroll = scrollPosition;
    }

}
