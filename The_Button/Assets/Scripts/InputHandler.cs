//================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:       GameDev Tips & Tricks (Thomas Valentin Klink)
// Script by:    Daniel Heilmann (771144)
// Last changed: 26-02-23
//================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputHandler : MonoBehaviour
{
    private static InputAction mousePosition;
    private Player player;

    private void Start()
    {
        PlayerInput input = GetComponent<PlayerInput>();
        input.currentActionMap.FindAction("MouseClick", true).performed += OnMouseClicked;
        input.currentActionMap.FindAction("DirectButtonPress", true).performed += OnButtonPressedDirectly;
        input.currentActionMap.FindAction("MenuButton", true).performed += OnMenuButtonPressed;
        mousePosition = input.currentActionMap.FindAction("MousePosition", true);

        player = FindObjectOfType<Player>();
    }

    public void OnMouseClicked(InputAction.CallbackContext context)
    {
        // Debug.Log($"Mouse clicked at mouse position {GetMousePosition()}.");
        player?.TryClickTheButton(GetMousePosition());
    }

    public void OnButtonPressedDirectly(InputAction.CallbackContext context)
    {
        // Debug.Log($"Button pressed directly.");
        player?.ClickTheButton();
    }

    public void OnMenuButtonPressed(InputAction.CallbackContext context)
    {

        Debug.Log($"Menu button has been pressed.");
        // Open / Close menu
    }

    public static Vector2 GetMousePosition()
    {
        return mousePosition.ReadValue<Vector2>();
    }
}
