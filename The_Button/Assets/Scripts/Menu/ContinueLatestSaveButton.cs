using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueLatestSaveButton : ButtonHelper
{
    public override void OnButtonPressed()
    {
        SaveHandler.Load(SaveHandler.GetLatestSaveState());
    }
}
