using UnityEngine;

[CreateAssetMenu(menuName = "Tools/Tool Data")]
public class ToolData : ScriptableObject
{
    public ToolType toolType;
    public string animationTrigger;
    public ResourceType[] canBreak;
}
