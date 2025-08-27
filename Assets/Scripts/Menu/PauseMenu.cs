using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool gamePaused = false;
    public GameObject pauseMenuUI;
    public GameObject optionsMenu;
    private string gameScene = "RegnaTest";

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape key pressed");
            if (gamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gamePaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gamePaused = true;
    }

    public void SaveGame()
    {
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            GameManager.instance.SaveGame();
        }
    }

    public void NewGame()
    {
        if (SaveSystem.SaveExists())
        {
            SaveSystem.DeleteSave();
        }

        var newData = new GameData();
        SaveSystem.SaveGame(newData);
        GameManager.instance.LoadGame(newData);
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameScene);
    }

    public void OpenOptions()
    {
        pauseMenuUI.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsMenu.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    public void QuitMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
