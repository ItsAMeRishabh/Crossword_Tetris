using UnityEngine;


[CreateAssetMenu(fileName = "Theme", menuName = "Config/Theme")]
public class Theme : ScriptableObject
{
    [Space(20)]
    public Sprite displayInactive;
    public Sprite display;
    public Color displayFontColor;


    [Space(20)]
    public Sprite inactive;
    public Sprite normal;
    public Sprite frozen;
    public Sprite bubble;
    public Sprite fly;
    public Sprite active2X;
    public Sprite selected;
    public Sprite golden;
    public Sprite debris;


    [Space(20)]

    public Sprite gem;
    public Sprite coin;

    [Space(20)]
    public Color normalFontColor;
    public Color frozenFontColor;
    public Color bubbleFontColor;
    public Color flyFontColor;
    public Color active2XFontColor;
    public Color selectedFontColor;
    public Color goldenFontColor;


    
}