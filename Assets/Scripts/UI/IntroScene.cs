using UnityEngine;
using UnityEngine.SceneManagement;
using Santa;

public class IntroScene : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene((int)SceneEnum.Level1);
    }
}
