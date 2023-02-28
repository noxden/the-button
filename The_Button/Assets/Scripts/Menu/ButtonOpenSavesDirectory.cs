using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ButtonOpenSavesDirectory : ButtonHelper
{
    public override void OnButtonPressed()
    {
        // EditorUtility.RevealInFinder(Application.persistentDataPath + "/saves");
    }
}
