using UnityEngine;
using System.Collections.Generic;

public class PlayerToolUser : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterMovement movement;

    [Header("Hitboxes (Scene)")]
    [SerializeField] private List<ToolHitbox> hitboxes;

    [Header("Input")]
    [SerializeField] private KeyCode useToolKey = KeyCode.E;

    private ToolData currentTool;
    private DamageDealer currentHitbox;
    private bool isUsingTool;

    private void Reset()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (isUsingTool || currentTool == null) return;

        if (Input.GetKeyDown(useToolKey))
            UseTool();
    }

    private void UseTool()
    {
        animator.ResetTrigger(currentTool.animationTrigger);
        animator.SetTrigger(currentTool.animationTrigger);

        isUsingTool = true;
        movement?.PushMovementBlock();
    }

    // Animation Event
    public void AE_ToolActionEnd()
    {
        if (currentHitbox != null)
            currentHitbox.Activate(currentTool);

        movement?.PopMovementBlock();
        isUsingTool = false;
    }

    public void SetTool(ToolData tool)
    {
        currentTool = tool;
        currentHitbox = GetHitboxForTool(tool.toolType);
    }

    private DamageDealer GetHitboxForTool(ToolType type)
    {
        foreach (var h in hitboxes)
        {
            if (h.toolType == type)
                return h.hitbox;
        }
        return null;
    }
}
