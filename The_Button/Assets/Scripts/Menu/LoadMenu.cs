//================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:       GameDev Tips & Tricks (Thomas Valentin Klink)
// Script by:    Daniel Heilmann (771144)
// Last changed: 27-02-23
//================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadMenu : Menu
{
    [SerializeField]
    private Transform parentTransform;  //< Needs to be assigned manually.
    [SerializeField]
    private GameObject buttonPrefab;

    private void Start()
    {
        id = MenuID.Load;
    }

    private void OnEnable()
    {
        Populate();
    }

    private void OnDisable()
    {
        // Delete all children of entriesparent
        List<GameObject> children = new List<GameObject>();
        int childCount = parentTransform.childCount;
        if (childCount <= 0)
            return;
            
        for (int i = 0; i < childCount; i++)
        {
            Destroy(parentTransform.GetChild(0).gameObject);
        }
    }

    private void Populate()
    {
        Dictionary<string, SaveState> allSaveStates = SaveHandler.GetAllSaveStatesWithPathsInReverseOrder();
        if (allSaveStates.Count == 0)
            return;

        foreach (KeyValuePair<string, SaveState> item in allSaveStates)
        {
            GameObject go = Instantiate(buttonPrefab, parentTransform, false);
            SaveStateEntry entry = go.GetComponent<SaveStateEntry>();            //? These two lines can be shortened. 
            entry.Setup(item.Key, item.Value);                                   //? Might make it less readable though.
        }
    }
}
