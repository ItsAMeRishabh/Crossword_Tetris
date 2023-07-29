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

    [Space(10)]
    public WinScreenManager WinPanel;
    public GameObject LosePanel;
    public SlidingPanel ObjectivePanel;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
}
