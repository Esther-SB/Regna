using UnityEngine;

[System.Serializable]
public class GameData
{
    public int playerLevel;
    public float playerHealth;
    public float[] playerPositionArray; // Menos problemas al pasar a JSON que VECTOR3

    public GameData()
    {
        playerLevel = 1;
        playerHealth = 100f;
        playerPositionArray = new float[3] { 0f, 0f, 0f };
    }

    public void SetPosition(Vector3 newPosition)
    {
        playerPositionArray[0] = newPosition.x;
        playerPositionArray[1] = newPosition.y;
        playerPositionArray[2] = newPosition.z;
    }

    public Vector3 GetPosition()
    {
        return new Vector3(playerPositionArray[0], playerPositionArray[1], playerPositionArray[2]);
    }
}
