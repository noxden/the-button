//================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:       GameDev Tips & Tricks (Thomas Valentin Klink)
// Script by:    Daniel Heilmann (771144)
// Last changed: 27-02-23
//================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class SaveStateEntry : MonoBehaviour
{
    private SaveState saveState;
    [SerializeField]
    private TextMeshProUGUI titleField;
    [SerializeField]
    private TextMeshProUGUI timestampField;
    [SerializeField]
    private TextMeshProUGUI identifierField;

    private Button button;

    private void Start()
    {
        button = GetComponentInChildren<Button>();
        button.onClick.AddListener(OnButtonPressed);
    }

    public void Setup(string path, SaveState newSaveState)
    {
        saveState = newSaveState;
        titleField.text = Path.GetFileNameWithoutExtension(path);
        timestampField.text = System.DateTime.FromFileTime(saveState.timestamp).ToString("dd/MM/yy, hh:mm");
        identifierField.text = $"{newSaveState.type}Save-{newSaveState.index}";
    }

    public void OnButtonPressed()
    {
        MenuHandler.instance.Open(MenuID.LoadingScreen, MenuID.Load);
        SaveHandler.Load(saveState);
    }
}
