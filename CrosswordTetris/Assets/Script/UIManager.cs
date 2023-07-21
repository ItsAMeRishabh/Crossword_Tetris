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
    public GameObject WinPanel;
    public GameObject LosePanel;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
}
