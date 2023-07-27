using UnityEngine;
public class ParallaxScroll : MonoBehaviour
{
    public RectTransform[] parallaxLayers;
    public float parallaxFactor = 0.5f;
    
    
    
    public void OnScrollValueChanged(Vector2 scrollPosition)
    {
        foreach (RectTransform layer in parallaxLayers)
        {
            layer.anchoredPosition = new Vector2(scrollPosition.x * parallaxFactor, scrollPosition.y * parallaxFactor);
        }
    }

}
