using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // GameObject para el menuprincipal
    public GameObject mainMenu;

    // GameObject para las opciones de menu
    public GameObject optionsMenu;

    // Nombre de la escena GameScene
    private string gameScene = "RegnaTest";

    /*
        Metodo para abrir el menu principal
    */
    public void OpenMainMenuPanel()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    /*
        Metodo para abrir el menu de opciones
    */
    public void OpenOptionsPanel()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    /*
        Metodo para salir del juego
    */
    public void ExitGame()
    {
        Application.Quit();
    }

    /*
        Metodo para iniciar el juego
    */
    public void PlayGame()
    {
        if (SaveSystem.SaveExists())
        {
            var data = SaveSystem.LoadGame();
            Debug.Log("Cargando partida guardada: " + data.GetPosition());
            GameManager.instance.LoadGame(data);
        }
        else
        {
            var newData = new GameData();
            SaveSystem.SaveGame(newData);
            Debug.Log("Partida guardada: " + newData.GetPosition());
            GameManager.instance.LoadGame(newData);
        }
        SceneManager.LoadScene(gameScene);
    }
}
