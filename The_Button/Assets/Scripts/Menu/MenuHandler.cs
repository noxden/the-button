//================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:       GameDev Tips & Tricks (Thomas Valentin Klink)
// Script by:    Daniel Heilmann (771144)
// Last changed: 27-02-23
//================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MenuID { None, Main, Side, Load, Save, Settings, Credits, Achievements, LoadingScreen }

public class MenuHandler : MonoBehaviour
{
    public static MenuHandler instance;

    private Dictionary<MenuID, Menu> allMenus;

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
        allMenus = new Dictionary<MenuID, Menu>();

        Menu[] menusInScene = FindObjectsOfType<Menu>(includeInactive: true);
        foreach (var item in menusInScene)
        {
            if (!allMenus.TryAdd(item.id, item))
                Debug.LogWarning($"MenuHandler: Something went wrong while finding menus in scene.");
        }

        openedMenus = new List<Menu>();

        Open(MenuID.Main, MenuID.None);  //! FOR DEBUG ONLY
    }

    public void Open(MenuID id, MenuID source)
    {
        if (id == MenuID.None)
            return;

        Menu menu = allMenus[id];

        if (menu == null)
        {
            Debug.LogWarning($"Dictionary does not contain entry for menu of ID \"{id}\".");
            return;
        }

        if (openedMenus.Contains(menu))
        {
            Debug.LogWarning($"Menu \"{menu}\" is already open.");
            return;
        }

        menu.Open(source);
        openedMenus.Add(menu);
    }

    public void Close(MenuID id)
    {
        if (id == MenuID.None)
            return;

        Menu menu = allMenus[id];

        if (menu == null)
        {
            Debug.LogWarning($"Dictionary does not contain entry for menu of ID \"{id}\".");
            return;
        }

        menu.Close();
        openedMenus.Remove(menu);
    }

    public void CloseAll()
    {
        foreach (var menu in openedMenus)
        {
            menu.ForceClose();
        }
        openedMenus.Clear();
    }
}
