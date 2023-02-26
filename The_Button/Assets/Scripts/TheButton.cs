//================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:       GameDev Tips & Tricks (Thomas Valentin Klink)
// Script by:    Daniel Heilmann (771144)
// Last changed: 26-02-23
//================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TheButton : MonoBehaviour
{
    public GameObject buttonModel;

    [Space(10)]
    [Header("Events")]
    public static UnityEvent<bool> OnButtonStateChanged;

    [Header("Visualisation Section")]
    [SerializeField]
    private bool _isPressed;

    private bool isPressed
    {
        get
        {
            return _isPressed;
        }
        set
        {
            if (value == _isPressed)
                return;

            _isPressed = value;
            UpdateModel(_isPressed);
            OnButtonStateChanged?.Invoke(_isPressed);
        }
    }

    private float startingHeight;
    private float pressedHeightChange = -0.22f;

    private void Start()
    {
        if (buttonModel == null)
        {
            Debug.LogWarning($"Variable \"buttonModel\" was not assigned. Animations will not play correctly.", this);
            return;
        }
        startingHeight = buttonModel.transform.localPosition.y;
    }

    private void UpdateModel(bool newPressedState)
    {
        Debug.Log($"Button is now {(isPressed ? "↓" : "↑")}");
        // Play animation here
        buttonModel.transform.localPosition = new Vector3(buttonModel.transform.localPosition.x, startingHeight + (newPressedState ? 1 : 0) * pressedHeightChange, buttonModel.transform.localPosition.z);
    }

    public void Click()
    {
        isPressed = !isPressed;
    }
}
