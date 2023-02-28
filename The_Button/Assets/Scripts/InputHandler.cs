//================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:       GameDev Tips & Tricks (Thomas Valentin Klink)
// Script by:    Daniel Heilmann (771144)
// Last changed: 28-02-23
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
        input.currentActionMap.FindAction("QuicksaveButton", true).performed += OnQuicksaveButtonPressed;
        input.currentActionMap.FindAction("QuickloadButton", true).performed += OnQuickloadButtonPressed;

        player = FindObjectOfType<Player>();
    }

    public void OnMouseClicked(InputAction.CallbackContext context)
    {
        if (MenuHandler.instance.GetCurrentMenu() == MenuID.GameOverlay)
            player?.TryClickTheButton(GetMousePosition());
    }

    public void OnButtonPressedDirectly(InputAction.CallbackContext context)
    {
        if (MenuHandler.instance.GetCurrentMenu() == MenuID.GameOverlay)
            player?.ClickTheButton();
    }

    public void OnMenuButtonPressed(InputAction.CallbackContext context)    //!< This method is so so very scuffed now... ;-; But at least it works.
    {
        Debug.Log($"MenuButton has been pressed.");
        BackButton backButton = FindObjectOfType<BackButton>(includeInactive: false);
        if (backButton != null)
        {
            backButton.OnButtonPressed();
        }
        else
        {
            if (!MenuHandler.instance.isOnTitleScreen())
            {
                if (MenuHandler.instance.GetCurrentMenu() != MenuID.Side)
                    MenuHandler.instance.Open(MenuID.Side);
                else if (MenuHandler.instance.GetCurrentMenu() == MenuID.Side)
                {
                    MenuHandler.instance.Open(MenuID.GameOverlay);
                }
            }
        }
    }

    public static Vector2 GetMousePosition()
    {
        return mousePosition.ReadValue<Vector2>();
    }

    public void OnQuicksaveButtonPressed(InputAction.CallbackContext context)
    {
        if (!MenuHandler.instance.isOnTitleScreen())
        {
            SaveHandler.Save(SaveType.Quick);
        }
    }

    public void OnQuickloadButtonPressed(InputAction.CallbackContext context)
    {
        if (!MenuHandler.instance.isOnTitleScreen())
        {
            SaveHandler.Load(SaveHandler.GetLatestSaveState(SaveType.Quick));
        }
    }
}
