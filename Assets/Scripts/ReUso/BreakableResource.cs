using UnityEngine;

public class BreakableResource : MonoBehaviour
{
    [SerializeField] private ResourceType resourceType;

    public bool CanBeBrokenBy(ToolData tool)
    {
        foreach (var type in tool.canBreak)
        {
            if (type == resourceType)
                return true;
        }
        return false;
    }

    public void Break()
    {
        Debug.Log($"{name} roto");
        Destroy(gameObject);
    }
}
