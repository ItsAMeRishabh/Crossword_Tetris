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
    public TileDisplay Display;
    [Space(10)]
    public GameObject WinPanel;
    public GameObject LosePanel;
    public SlidingPanel ObjectivePanel;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
}
