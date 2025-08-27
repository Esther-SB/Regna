using System.IO;
using UnityEngine;

public static class SaveSystem
{
    // Ruta del archivo de guardado
    private static string path = Application.persistentDataPath + "/save.json";

    /*
    Guarda el estado del juego en un archivo JSON
    */
    public static void SaveGame(GameData data)
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(path, json);
    }

    /*
    Carga el estado del juego desde un archivo JSON
    */
    public static GameData LoadGame()
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<GameData>(json);
        }
        return null;
    }

    /*
    Verifica si existe un archivo de guardado
    */
    public static bool SaveExists()
    {
        return File.Exists(path);
    }

    /*
        Elimina el archivo de guardado
    */
    public static void DeleteSave()
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
