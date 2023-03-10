//================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:       GameDev Tips & Tricks (Thomas Valentin Klink)
// Script by:    Daniel Heilmann (771144)
// Last changed: 27-02-23
//================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OpeningType { Solo, Overlay }
public enum ClosingType { JustClose, Return, Static }

public class Menu : MonoBehaviour
{
    public MenuID id;

    [SerializeField]
    public OpeningType openingType;
    [SerializeField]
    public ClosingType closingType;

    public MenuID returnToMenuOnClose = MenuID.None;

    public void Open()
    {
        this.gameObject.SetActive(true);
    }

    public void Open(MenuID previousMenu)
    {
        if (closingType != ClosingType.Static)
            returnToMenuOnClose = previousMenu;
        this.gameObject.SetActive(true);
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }
}
