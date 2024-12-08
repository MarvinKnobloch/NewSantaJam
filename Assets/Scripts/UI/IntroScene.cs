using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroScene : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(2);
    }
}
