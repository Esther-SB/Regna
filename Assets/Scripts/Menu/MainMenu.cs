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
        SceneManager.LoadScene(gameScene);
    }
}
