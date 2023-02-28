using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSaveGame : ButtonHelper
{
    public override void OnButtonPressed()
    {
        SaveHandler.Save(SaveType.Manual);
    }
}
