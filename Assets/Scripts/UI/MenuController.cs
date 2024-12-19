using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Santa;

public class MenuController : MonoBehaviour
{
    private Controls controls;

    private GameObject baseMenu;
    private GameObject currentOpenMenu;
    [NonSerialized] public bool gameIsPaused;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject ingameMenu;

    private void Awake()
    {
        controls = Keybindinputmanager.Controls;
        controls.Enable();

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            baseMenu = mainMenu;
            baseMenu.SetActive(true);
        }
        else
        {
            baseMenu = ingameMenu;
        }

        if (AudioController.Instance != null)
        {
            //Music
        }
    }
    void Update()
    {
        if (controls.Menu.MenuEsc.WasPerformedThisFrame())
        {
            HandleMenu();
        }

    }
    public void HandleMenu()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            if (mainMenu.activeSelf == true) return;
            else CloseSelectedMenu();
        }
        else
        {
            if (Player.Instance == null) return;

            if (GameManager.Instance.playerUI.messageBox.activeSelf) GameManager.Instance.playerUI.CloseMessageBox();
            else if (ingameMenu.activeSelf == false)
            {
                if (gameIsPaused == false)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    PauseGame();
                    ingameMenu.SetActive(true);

                }
                else CloseSelectedMenu();
            }
            else
            {
                ingameMenu.SetActive(false);
                EndPause();
            }
        }
    }

    public void OpenSelection(GameObject currentMenu)
    {
        {
            currentOpenMenu = currentMenu;
            currentMenu.SetActive(true);

            mainMenu.SetActive(false);
            ingameMenu.SetActive(false);

            AudioController.Instance.PlaySoundOneshot((int)AudioController.Sounds.menuButton);
        }
    }
    public void StartGame()
    {
        baseMenu.SetActive(false);
        baseMenu = ingameMenu;

        if (PlayerPrefs.GetInt("NewGame") == 0)
        {
            GameManager.Instance.LoadFormMenu();

            PlayerPrefs.SetInt("SceneNumber", (int)SceneEnum.Level1);
            PlayerPrefs.SetFloat("SavePlayerXPosition", 18);
            PlayerPrefs.SetFloat("SavePlayerYPosition", 1);
            PlayerPrefs.SetFloat("SavePlayerZPosition", -53);
            PlayerPrefs.SetFloat("SavePlayerRotation", 0);
            PlayerPrefs.SetInt("DoubleJumpUnlock", 0);
            PlayerPrefs.SetInt("DashUnlock", 0);

            PlayerPrefs.SetInt("NewGame", 1);
            SceneManager.LoadScene((int)SceneEnum.Level1); //SceneManager.LoadScene("IntroScene");
        }
        else
        {
            GameManager.Instance.LoadFormMenu();
            SceneManager.LoadScene(PlayerPrefs.GetInt("SceneNumber"));
        }
    }
    public void ResumeGame()
    {
        ingameMenu.SetActive(false);
        EndPause();
    }
    public void NewGame()
    {
        AudioController.Instance.PlaySoundOneshot((int)AudioController.Sounds.menuButton);
        currentOpenMenu.SetActive(false);

        PlayerPrefs.SetInt("NewGame", 0);

        gameIsPaused = false;
        Time.timeScale = 1;
        StartGame();
    }
    public void BackToMainMenu()
    {
        baseMenu.SetActive(false);
        baseMenu = mainMenu;

        baseMenu.SetActive(true);
        GameManager.Instance.ActivateGameUI(false);

        //GameManager.Instance.ResetProgress();

        AudioController.Instance.PlaySoundOneshot((int)AudioController.Sounds.menuButton);

        gameIsPaused = false;
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
    public void CloseSelectedMenu()
    {
        if (currentOpenMenu != null)
        {
            currentOpenMenu.SetActive(false);
            currentOpenMenu = null; // Clear previous menu after returning
            baseMenu.SetActive(true);
        }
        else
        {
            Debug.LogWarning("No previous menu to return to. Going back to inGameMenu.");
            baseMenu.SetActive(true);
        }
        AudioController.Instance.PlaySoundOneshot((int)AudioController.Sounds.menuButton);
    }

    private void PauseGame()
    {
        gameIsPaused = true;
        Time.timeScale = 0;

        AudioController.Instance.PlaySoundOneshot((int)AudioController.Sounds.menuButton);
    }
    private void EndPause()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameIsPaused = false;
        Time.timeScale = 1;

        AudioController.Instance.PlaySoundOneshot((int)AudioController.Sounds.menuButton);
    }
}
