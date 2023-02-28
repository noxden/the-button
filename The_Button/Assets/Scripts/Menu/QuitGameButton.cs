using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGameButton : ButtonHelper
{
    public override void OnButtonPressed()
    {
        Debug.Log($"Closing game.");
        Application.Quit(exitCode:0);
    }
}
