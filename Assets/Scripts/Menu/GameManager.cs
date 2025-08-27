using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameData gameData;

    /*
    Inicializa el GameManager
    */
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /*
    Cargamos la escena y restauramos la posición del jugador
    */
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Escena cargada: " + scene.name);
        var player = GameObject.FindWithTag("Player");
        if (player != null && gameData != null && gameData.playerPositionArray != null)
        {
            Vector3 savedPosition = gameData.GetPosition();
            player.transform.position = savedPosition;
            Debug.Log("Jugador colocado en posición guardada: " + savedPosition);
        }
        else
        {
            if (player == null)
                Debug.LogWarning("Jugador no encontrado en la escena.");
            if (gameData == null)
                Debug.LogWarning("GameData es null.");
        }
    }

    /*
    Carga el estado del juego desde un archivo JSON
    */
    public void LoadGame(GameData data)
    {
        gameData = data;
    }

    /*
    Guarda el estado del juego en un archivo JSON
    */
    public void SaveGame()
    {
        if (gameData == null)
        {
            gameData = new GameData();
        }
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            gameData.SetPosition(player.transform.position);
            Debug.Log("Guardando partida en: " + player.transform.position);
        }

        SaveSystem.SaveGame(gameData);
    }
}
