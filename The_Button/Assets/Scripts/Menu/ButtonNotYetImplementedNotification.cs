using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonNotYetImplementedNotification : ButtonHelper
{
    private const string message = "Unfortunately, this feature is not implemented yet.";

    public override void OnButtonPressed()
    {
        ScreenConsole.instance.Display(message, 6);
    }
}
