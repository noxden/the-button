using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleMenu : Menu
{
    private void OnEnable()
    {
        SaveHandler.Load(SaveHandler.GetLatestSaveState());
    }
}
