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
public enum ClosingType { JustClose, Return }

public class Menu : MonoBehaviour
{
    public MenuID id;

    [SerializeField]
    private OpeningType openingType;
    [SerializeField]
    private ClosingType closingType;
    private MenuID openedBy;

    public void Open(MenuID source)
    {
        openedBy = source;
        if (openingType == OpeningType.Solo)
            MenuHandler.instance.CloseAll();

        this.gameObject.SetActive(true);
    }

    public void Open()  //! FOR DEBUG PURPOSES ONLY
    {
        Open(MenuID.None);
    }

    public void Close()
    {
        if (closingType == ClosingType.Return)
            MenuHandler.instance.Open(openedBy, this.id);
        openedBy = MenuID.None;
        this.gameObject.SetActive(false);
    }

    public void ForceClose()
    {
        openedBy = MenuID.None;
        this.gameObject.SetActive(false);
    }
}
