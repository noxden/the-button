using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackButton : ButtonHelper
{
    private Menu myMenu;

    private new void Start()
    {
        base.Start();
        myMenu = GetComponentInParent<Menu>();
    }

    public override void OnButtonPressed()
    {
        MenuHandler.instance.Close(myMenu.id);
    }
}
