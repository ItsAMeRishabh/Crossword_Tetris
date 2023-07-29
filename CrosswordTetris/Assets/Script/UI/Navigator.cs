using UnityEngine;

public class Navigator : MonoBehaviour
{
    public void Goto(string scene)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
    }
}