using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPurgeAllData : ButtonHelper
{
    public override void OnButtonPressed()
    {
        SaveHandler.PurgeAllData();
        Application.Quit(1);
    }
}
