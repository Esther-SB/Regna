using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject pauseMenuUI;
    public GameObject optionsMenu;
    public GameObject controlsMenu;
    private string gameScene = "RegnaTest";
    public static bool gamePaused = false;

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

    /*
    Reactiva el juego
    */
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gamePaused = false;
    }

    /*
    Pausa el juego
    */
    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gamePaused = true;
    }

    /*
    Guarda el estado del juego (por ahora solo la posición del jugador)
    */
    public void SaveGame()
    {
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            GameManager.instance.SaveGame();
        }
    }

    /*
    Elimina la partida guardada y los playerprefs
    */
    public void NewGame()
    {
        if (SaveSystem.SaveExists())
        {
            SaveSystem.DeleteSave();
        }

        PlayerPrefs.DeleteAll();
        var newData = new GameData();
        SaveSystem.SaveGame(newData);
        GameManager.instance.LoadGame(newData);
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameScene);
    }

    /*
    Abre el menú de opciones
    */
    public void OpenOptions()
    {
        pauseMenuUI.SetActive(false);
        optionsMenu.SetActive(true);
    }

    /*
    Cierra el menú de opciones
    */
    public void CloseOptions()
    {
        optionsMenu.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    /*
    Abre el menú de controles
    */
    public void OpenControls()
    {
        pauseMenuUI.SetActive(false);
        controlsMenu.SetActive(true);
    }

    /*
    Cierra el menú de controles
    */
    public void CloseControls()
    {
        controlsMenu.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    /*
    Vuelve al menú principal
    */
    public void QuitMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
