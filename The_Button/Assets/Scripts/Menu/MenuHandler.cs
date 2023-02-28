//================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:       GameDev Tips & Tricks (Thomas Valentin Klink)
// Script by:    Daniel Heilmann (771144)
// Last changed: 27-02-23
// Notes:
//  Unfortunately, I did not have enough time to implement a 
//  Save menu
//================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MenuID { None, Title, GameOverlay, Side, Load, Save, Settings, Credits, Achievements, Stats, Confirm, LoadingScreen }

public class MenuHandler : MonoBehaviour
{
    public static MenuHandler instance { get; private set; }

    private Dictionary<MenuID, Menu> allMenus;

    [Header("Visualisation Section")]
    [SerializeField]
    private List<Menu> openedMenus;

    private void Awake()
    {
        // Singleton-Setup
        if (instance == null) { instance = this; }
        else { DestroyImmediate(this); }
    }

    private void Start()
    {
        Menu[] menusInScene = FindObjectsOfType<Menu>(includeInactive: true);
        foreach (Menu menu in menusInScene)
        {
            menu.gameObject.SetActive(false);   //< So that menus can be kept on in the editor without influencing the runtime game.
        }

        allMenus = new Dictionary<MenuID, Menu>();
        foreach (Menu menu in menusInScene)
        {
            if (!allMenus.TryAdd(menu.id, menu))
                Debug.LogWarning($"MenuHandler: Something went wrong while finding menus in scene.");
        }

        openedMenus = new List<Menu>();

        //!> FOR DEBUG ONLY
        Open(MenuID.Title);
    }

    public void Open(MenuID id)
    {
        if (!TryResolveID(id, out Menu menu))
            return;

        if (openedMenus.Contains(menu))
        {
            Debug.LogWarning($"Menu \"{menu}\" is already open.");
            return;
        }

        MenuID currentMenu = GetCurrentMenu();
        if (menu.openingType == OpeningType.Solo)
            CloseAll();

        menu.Open(currentMenu);
        openedMenus.Add(menu);
    }

    public void Close(MenuID id)
    {
        if (!TryResolveID(id, out Menu menu))
            return;

        if (menu.closingType == ClosingType.Return)
            Open(menu.returnToMenuOnClose);

        menu.Close();
        openedMenus.Remove(menu);
    }

    public void CloseAll()
    {
        foreach (var menu in openedMenus)
        {
            menu.Close();
        }
        openedMenus.Clear();    //< Excess capacity does not need to be trimmed, as "openedMenus" will be filled again soon.
    }

    public bool isOnTitleScreen()
    {
        foreach (Menu menu in openedMenus)
        {
            if (menu.id == MenuID.Title)
                return true;
        }
        return false;
    }

    public MenuID GetCurrentMenu()
    {
        if (openedMenus.Count > 0)
            return openedMenus[openedMenus.Count - 1].id;
        else
            return MenuID.None;
    }

    private bool TryResolveID(MenuID id, out Menu menu)
    {
        if (id == MenuID.None)
        {
            Debug.Log($"Menu ID was \"{MenuID.None}\", aborting action.");
            menu = null;
            return false;
        }

        if (!allMenus.TryGetValue(id, out menu))    //< Puts menu value from dictionary directly into this method's out parameter.
        {
            Debug.LogWarning($"Dictionary does not contain entry for menu of ID \"{id}\".");
            return false;
        }
        return true;
    }
}