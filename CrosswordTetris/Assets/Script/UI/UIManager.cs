using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public TextMeshProUGUI OutputBox;
    public TextMeshProUGUI QuestionBox;
    public TextMeshProUGUI PointBox;
    public TextMeshProUGUI MovesUsedBox;
    public TextMeshProUGUI ExpBox;
    public TextMeshProUGUI FPSCount;
    public TextMeshProUGUI Coins;
    public TextMeshProUGUI Gems;
    public TileDisplay Display;
    public ProgressBar PointsProgress;
    public Animator TextField;
    public RectTransform TileGridBG;

    [Space(10)]
    public WinScreenManager WinPanel;
    public GameObject LosePanel;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
}
