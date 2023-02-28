using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenMenuButton : ButtonHelper
{
    [SerializeField]
    private MenuID menu;

    public override void OnButtonPressed()
    {
        MenuHandler.instance.Open(menu);
    }
}
