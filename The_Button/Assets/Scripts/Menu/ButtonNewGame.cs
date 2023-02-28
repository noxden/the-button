using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonNewGame : ButtonHelper
{
    public override void OnButtonPressed()
    {
        SaveHandler.Load(new SaveState());
    }
}
