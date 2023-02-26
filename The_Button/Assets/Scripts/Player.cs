//================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:       GameDev Tips & Tricks (Thomas Valentin Klink)
// Script by:    Daniel Heilmann (771144)
// Last changed: 26-02-23
//================================================================
//? Currently lacking support for multi-button setup. Which button would be clicked via DirectPress, if no button has been clicked before?
//? Though then it wouldn't be "The Button" anymore, would it? -> No multi-button support!


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private TheButton button;

    private void Start()
    {
        button = FindObjectOfType<TheButton>();
    }

    public void TryClickTheButton(Vector2 mousePosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.tag == "TheButton")
            {
                button?.Click();
            }
        }
    }

    public void ClickTheButton()
    {
        button?.Click();
    }
}
