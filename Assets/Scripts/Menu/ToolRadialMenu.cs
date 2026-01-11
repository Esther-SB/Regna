using UnityEngine;
using System.Collections.Generic;

public class ToolRadialMenu : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject menuRoot;
    [SerializeField] private PlayerToolUser player;
    [SerializeField] private CharacterMovement movement;

    [Header("Tools")]
    [SerializeField] private List<ToolData> tools;

    [Header("Input")]
    [SerializeField] private KeyCode openKey = KeyCode.Tab;

    private bool isOpen;

    private void Start()
    {
        menuRoot.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(openKey))
            OpenMenu();

        if (Input.GetKeyUp(openKey))
            CloseMenu();
    }

    private void OpenMenu()
    {
        if (isOpen) return;

        isOpen = true;
        menuRoot.SetActive(true);

        movement?.PushMovementBlock();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void CloseMenu()
    {
        if (!isOpen) return;

        isOpen = false;
        menuRoot.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        movement?.PopMovementBlock();

        SelectToolByMouse();
    }

    private void SelectToolByMouse()
    {
        Vector2 mouse = Input.mousePosition;
        Vector2 center = new(Screen.width / 2f, Screen.height / 2f);
        Vector2 dir = mouse - center;

        if (dir.magnitude < 50f)
            return;

        // Ángulo 0° = derecha, sentido horario
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;

        float sectorSize = 360f / tools.Count;
        int index = Mathf.FloorToInt(angle / sectorSize);

        index = Mathf.Clamp(index, 0, tools.Count - 1);

        Debug.Log($"Angle: {angle} | Index: {index} | Tool: {tools[index].toolType}");
        player.SetTool(tools[index]);
    }

}
