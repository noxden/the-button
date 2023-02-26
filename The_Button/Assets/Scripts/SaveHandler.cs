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

public enum SaveType { Manual, Auto, Quick }

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

// TODO: Still needs to be implemented.
// TODO: Should be loaded on game start and saved on game end.
[System.Serializable]
public class SavePersistentData
{
    public int manualSaveIndex;
    public int autosaveIndex;
    public int quicksaveIndex;
}

public class SaveHandler : MonoBehaviour
{
    private const string Identifier_Quicksave = "QuickSave";
    private const string Identifier_Autosave = "AutoSave";
    private const string Identifier_ManualSave = "ManualSave";
    private const string FileEnding = ".save";

    public bool canSave = true;

    private SavePersistentData data;

    private string dataFolderPath;
    private string savesFolderPath;
    private string savePersistentFilePath;

    void Start()
    {
        dataFolderPath = Application.persistentDataPath + "/";   //< The persistentDataPath refers to a folder in AppData
        savesFolderPath = dataFolderPath + "saves/";
        savePersistentFilePath = dataFolderPath + "persistentData.json";

        if (!Directory.Exists(savesFolderPath))
            Directory.CreateDirectory(savesFolderPath);

        ReadPersistentData();
        // Write();
        // Write();
        // Write();
        // Read();

        long filetime = System.DateTime.Now.ToFileTime();
        // Debug.Log($"{System.DateTime.Now.ToString("ddMMyyyyHHmmss")}{FileEnding} | {filetime} | {System.DateTime.FromFileTime(filetime)}");
    }

    private void OnApplicationQuit()
    {
        WritePersistentData();
    }

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

    // TODO: Use reflection to make write work for all different save types with minimal code
    public void Write()     //? Potential properties: SaveType, 
    {
        if (!canSave)
        {
            Debug.Log($"You cannot save right now.");   //? Maybe disable "Save Game" UI button in that case?
            return;
        }

        Debug.Log($"Saving...");
        string writeFilePath = savesFolderPath + $"{Identifier_ManualSave}-{data.manualSaveIndex}{FileEnding}";  // $"{System.DateTime.Now.ToString("yyyyMMddHHmmss")}.json";
        SaveState saveState = new SaveState();

        //> Fill data into new save state
        saveState.timestamp = System.DateTime.Now.ToFileTime();
        saveState.saveType = SaveType.Manual;
        saveState.saveIndex = data.manualSaveIndex;

        //> Convert to json and then save in file
        string serializedSaveState = JsonUtility.ToJson(saveState, true);
        // Debug.Log($"{serializedSaveState}");
        File.WriteAllText(writeFilePath, serializedSaveState);

        data.manualSaveIndex++;
    }

    private void ForceWrite()
    {
        if (canSave)
        {
            Write();
        }
        else
        {
            bool original = canSave;
            canSave = true;
            Write();
            canSave = original;
        }

    }

    public void Read()     //? Potential property: FilePath which should be read from
    {
        Debug.Log($"Loading...");

        string[] allSaveFilesPaths = Directory.GetFiles(savesFolderPath);
        Debug.Log($"Found the following ({allSaveFilesPaths.Length}) files: {SavePathsToFileNames(allSaveFilesPaths)}");

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
        Debug.Log($"The latest save is from {latestSaveTime.ToString()}.");

        string readFilePath = filePathOfLatestSave;     //! Needs to be updated later on

        //> Read json file and then fill its data back into a new save state
        if (!File.Exists((readFilePath)))
        {
            Debug.LogWarning($"The file you are trying to read does not exist at {readFilePath}.");
            return;
        }
        string readText = File.ReadAllText(readFilePath);
        SaveState deserializedSaveState = JsonUtility.FromJson<SaveState>(readText);
        if (deserializedSaveState == null)
        {
            Debug.LogWarning($"Could not read json file at path \"{readFilePath}\". Loading of SaveState failed.");
            return;
        }

        //> Inject read data into their respective objects
        // = deserializedSaveState.realDateTimeOnSave;
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
    }

    private string SavePathsToFileNames(string[] input)
    {
        string output = "";
        foreach (string entry in input)
        {
            int indexOfSavePath = entry.IndexOf("saves/");
            output += $"{entry.Remove(0, indexOfSavePath + 6)}, ";
        }
        output = output.Remove(output.Length - 2);
        return output;
    }
}
