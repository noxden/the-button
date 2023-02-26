//================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:       GameDev Tips & Tricks (Thomas Valentin Klink)
// Script by:    Daniel Heilmann (771144)
// Last changed: 26-02-23
//================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum SaveType { Manual, Quick, Auto }

[System.Serializable]
public class SaveState
{
    public long timestamp;
    public SaveType saveType;
    public int saveIndex;
    public bool isButtonPressed;
    public int amountButtonPressesTotal;
    public int amountMouseClicksTotal;
    public int amountMouseClicksHit;
    public float durationSinceLastButtonPressCurrent;
    public float durationSinceLastButtonPressHighest;
    public float durationLightsOnHighest;
    public float durationLightsOnTotal;
    public float durationLightsOffHighest;
    public float durationLightsOffTotal;
    public float playTimeTotal;
    public int timesGameWasSaved;
}

[System.Serializable]
public class SavePersistentData
{
    public int manualSaveIndex;
    public int autosaveIndex;
    public int quicksaveIndex;
}

public class SaveHandler : MonoBehaviour
{
    private const string Identifier_ManualSave = "ManualSave";
    private const string Identifier_Quicksave = "QuickSave";
    private const string Identifier_Autosave = "AutoSave";
    private const string FileEnding = ".save";

    private static SaveHandler instance;

    private string dataFolderPath;
    private string savesFolderPath;
    private string savePersistentFilePath;
    private SavePersistentData data;

    public bool canSave = true;

    private void Awake()
    {
        //> Singleton Setup
        if (instance == null) { instance = this; DontDestroyOnLoad(this); }
        else { DestroyImmediate(this); }

        //> Set up path references and folder structure.
        dataFolderPath = Application.persistentDataPath + "/";   //< The persistentDataPath refers to a folder in AppData
        savesFolderPath = dataFolderPath + "saves/";
        savePersistentFilePath = dataFolderPath + "persistentData";  //< Missing file ending to disincentivize user from opening it

        if (!Directory.Exists(savesFolderPath))
            Directory.CreateDirectory(savesFolderPath);
    }

    //# Handling persistent data
    void Start() => ReadPersistentData();

    private void OnApplicationQuit() => WritePersistentData();

    private void ReadPersistentData()
    {
        string filePath = savePersistentFilePath;
        if (File.Exists(filePath))
        {
            string readText = File.ReadAllText(filePath);
            data = JsonUtility.FromJson<SavePersistentData>(readText);
            if (data == null)
            {
                Debug.LogWarning($"Persistent data at \"{filePath}\" seems to be corrupted. Regenerating...");
                data = new SavePersistentData();
            }
        }
        else
        {
            WritePersistentData();
        }
    }

    private void WritePersistentData()
    {
        string filePath = savePersistentFilePath;
        if (data == null)
            data = new SavePersistentData();
        string serializedData = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, serializedData);
    }

    //# Handling normal save data
    // TODO: Use reflection to make write work for all different save types with minimal code
    public bool Save(SaveType saveType)     //< Returns whether saving was successful.    //? Potential properties: SaveType, ...
    {
        if (!canSave)
        {
            Debug.LogWarning($"You cannot save right now.");   //? Maybe disable "Save Game" UI button in that case?
            return false;
        }

        string writeFilePath = savesFolderPath + $"{saveType.ToString()}Save-{data.manualSaveIndex}{FileEnding}";
        Debug.Log($"Saving to \"{Path.GetFileNameWithoutExtension(writeFilePath)}\"...");

        //> Fill data into new save state
        SaveState saveState = new SaveState();
        saveState.timestamp = System.DateTime.Now.ToFileTime();
        saveState.saveType = saveType;
        saveState.saveIndex = GetSaveIndex(saveType);

        //> Convert to json and then save in file
        string serializedSaveState = JsonUtility.ToJson(saveState, true);
        File.WriteAllText(writeFilePath, serializedSaveState);

        data.manualSaveIndex++;
        return true;
    }

    public void Save() => Save(SaveType.Manual);    //! OVERLOAD FOR DEBUG PURPOSES. NEEDED TO MANUALLY ASSIGN TO BUTTON.

    public bool Load(string readFilePath)   //< Returns whether loading was successful.    //? Potential property: FilePath which should be read from
    {
        Debug.Log($"Loading...");

        readFilePath = GetLatestSave();     //! FOR DEBUG PURPOSES ONLY
        Debug.Log($"The latest save is {Path.GetFileNameWithoutExtension(readFilePath)}.");

        //> Read json file and then fill its data back into a new save state
        if (!File.Exists((readFilePath)))
        {
            Debug.LogWarning($"The file you are trying to read does not exist at {readFilePath}.");
            return false;
        }
        string readText = File.ReadAllText(readFilePath);
        SaveState deserializedSaveState = JsonUtility.FromJson<SaveState>(readText);
        if (deserializedSaveState == null)
        {
            Debug.LogWarning($"Could not read json file at path \"{readFilePath}\". Loading of SaveState failed.");
            return false;
        }

        //> Inject read data into their respective objects
        // = deserializedSaveState.isButtonPressed;
        // = deserializedSaveState.amountButtonPressesTotal;
        // = deserializedSaveState.amountMouseClicksTotal;
        // = deserializedSaveState.amountMouseClicksHit;
        // = deserializedSaveState.durationSinceLastButtonPressCurrent;
        // = deserializedSaveState.durationSinceLastButtonPressHighest;
        // = deserializedSaveState.durationLightsOnHighest;
        // = deserializedSaveState.durationLightsOnTotal;
        // = deserializedSaveState.durationLightsOffHighest;
        // = deserializedSaveState.durationLightsOffTotal;
        // = deserializedSaveState.playTimeTotal;
        // = deserializedSaveState.timesGameWasSaved;
        return true;
    }

    public void ForceAutosave()
    {
        if (canSave)
            Save(SaveType.Auto);
        else
        {
            bool originalValue = canSave;   //< Temporarily sets canSave to true to work around the "canSave" check in "Save()".
            canSave = true;
            Save(SaveType.Auto);
            canSave = originalValue;
        }
    }

    private string PathsToFileNames(string[] input)
    {
        string output = "";
        foreach (string entry in input)
        {
            output += Path.GetFileName(entry) + ", ";
        }
        output = output.Remove(output.Length - 2);
        return output;
    }

    private string GetLatestSave()
    {
        string[] allSaveFilesPaths = Directory.GetFiles(savesFolderPath);
        Debug.Log($"Found the following ({allSaveFilesPaths.Length}) files: {PathsToFileNames(allSaveFilesPaths)}");

        string filePathOfLatestSave = "";
        System.DateTime latestSaveTime = new System.DateTime();
        foreach (string entry in allSaveFilesPaths)
        {
            System.DateTime temp = File.GetLastWriteTime(entry);
            if (temp > latestSaveTime)
            {
                latestSaveTime = temp;
                filePathOfLatestSave = entry;
            }
        }
        return filePathOfLatestSave;
    }

    private int GetSaveIndex(SaveType saveType)
    {
        switch (saveType)
        {
            case SaveType.Manual:
                return data.manualSaveIndex;
            case SaveType.Quick:
                return data.quicksaveIndex;
            case SaveType.Auto:
                return data.autosaveIndex;
            default:
                return 0;
        }
    }
}
