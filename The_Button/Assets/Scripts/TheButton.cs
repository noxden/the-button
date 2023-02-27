//================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:       GameDev Tips & Tricks (Thomas Valentin Klink)
// Script by:    Daniel Heilmann (771144)
// Last changed: 27-02-23
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

    [SerializeField]
    private bool isClickable = true;

    public bool isPressed
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
            OnButtonToggled();
            OnButtonStateChanged?.Invoke(_isPressed);
        }
    }

    private float defaultHeight;
    private float pressedHeightChange = -0.15f;

    private void Start()
    {
        if (buttonModel == null)
        {
            Debug.LogWarning($"Variable \"buttonModel\" was not assigned. Animations will not play correctly.", this);
            return;
        }
        defaultHeight = buttonModel.transform.localPosition.y;
    }

    private void OnButtonToggled()
    {
        Debug.Log($"Button is now {(isPressed ? "↓" : "↑")}");
        StartCoroutine(ButtonAnimation(isPressed));
    }

    public void Click()
    {
        if (isClickable)
            isPressed = !isPressed;
    }

    IEnumerator ButtonAnimation(bool newButtonState)
    {
        //> Pre-Animation
        isClickable = false;
        if (!newButtonState)                        //< If the animation goes from "pressed -> not pressed", invert now, as
            ColorHandler.instance.InvertPalette();  //< the inverted palette is only used for the fully pressed down button.

        //> While loop for animation
        float duration = 0.1f;

        float start = buttonModel.transform.localPosition.y;
        float end = defaultHeight + (newButtonState ? 1 : 0) * pressedHeightChange;
        float current = 0;

        float timePassed = 0;
        while (timePassed < duration)
        {
            current = Mathf.SmoothStep(start, end, timePassed * (1 / duration));
            timePassed += Time.deltaTime;
            buttonModel.transform.localPosition = new Vector3(buttonModel.transform.localPosition.x, current, buttonModel.transform.localPosition.z);
            yield return new WaitForEndOfFrame();
        }

        //> Post-Animation
        if (newButtonState)
            ColorHandler.instance.InvertPalette();  //< Invert colors after the animation if it goes from "not pressed -> pressed".
        isClickable = true;
    }
}
