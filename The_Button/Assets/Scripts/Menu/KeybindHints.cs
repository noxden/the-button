using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeybindHints : MonoBehaviour
{
    [SerializeField]
    private Transform parent;
    [SerializeField]
    private GameObject hintPrefab;
    [SerializeField]
    private List<string> keybinds;

    void Start()
    {
        if (hintPrefab == null || keybinds.Count == 0)
        {
            Debug.LogWarning($"{this} has not been setup properly. Please fill exposed fields and restart.");
            return;
        }

        foreach (string entry in keybinds)
        {
            GameObject go = Instantiate(hintPrefab, parent);
            go.GetComponentInChildren<TextMeshProUGUI>().text = entry;
        }
    }
}
