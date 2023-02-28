//================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:       GameDev Tips & Tricks (Thomas Valentin Klink)
// Script by:    Daniel Heilmann (771144)
// Last changed: 27-02-23
// Notes:
//  Unfortunately, none of the additional statistics made it
//  into the final application. "isButtonPressed" is the only
//  "stat" that is currently being saved.
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
    public SaveType type;
    public int index;
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
    public int manualSaveIndex { get; private set; }
    public int autosaveIndex { get; private set; }
    public int quicksaveIndex { get; private set; }

    public void IncreaseSaveIndex(SaveType saveType)
    {
        switch (saveType)
        {
            case SaveType.Manual:
                manualSaveIndex++;
                break;
            case SaveType.Auto:
                autosaveIndex++;
                break;
            case SaveType.Quick:
                quicksaveIndex++;
                break;
        }
    }

    public int GetSaveIndex(SaveType saveType)
    {
        switch (saveType)
        {
            case SaveType.Manual:
                return manualSaveIndex;
            case SaveType.Auto:
                return autosaveIndex;
            case SaveType.Quick:
                return quicksaveIndex;
        }
        return 0;
    }
}

public class SaveHandler : MonoBehaviour
{
    private const string FileEnding = ".save";

    public static SaveHandler instance { get; private set; }

    private static string dataFolderPath;
    private static string savesFolderPath;
    private static string savePersistentFilePath;
    private static SavePersistentData data;

    public static bool canSave = true;

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
        // else
        // {
        //     WritePersistentData();
        // }
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
    public static bool Save(SaveType saveType, out string writeFilePath)     //< Returns whether saving was successful.    //? Potential properties: SaveType, ...
    {
        if (!canSave)
        {
            Debug.LogWarning($"You cannot save right now.");   //? Maybe disable "Save Game" UI button in that case?
            writeFilePath = null;
            return false;
        }

        writeFilePath = savesFolderPath + $"{saveType.ToString()}Save-{data.GetSaveIndex(saveType)}{FileEnding}";
        ScreenConsole.instance.Display($"Saving to \"{Path.GetFileNameWithoutExtension(writeFilePath)}\"...");

        //> Fill data into new save state
        SaveState saveState = new SaveState();
        saveState.timestamp = System.DateTime.Now.ToFileTime();
        saveState.type = saveType;
        saveState.index = data.GetSaveIndex(saveType);
        saveState.isButtonPressed = FindObjectOfType<TheButton>().isPressed;

        //> Convert to json and then save in file
        string serializedSaveState = JsonUtility.ToJson(saveState, true);
        File.WriteAllText(writeFilePath, serializedSaveState);

        data.IncreaseSaveIndex(saveType);
        return true;
    }

    public static bool Save(SaveType saveType)  //< Overload that makes out parameter optional
    {
        return Save(saveType, out _);
    }

    public static SaveState Read(string readFilePath)   //< Returns whether loading was successful.    //? Potential property: FilePath which should be read from
    {
        //Debug.Log($"Reading \"{Path.GetFileNameWithoutExtension(readFilePath)}\"...");

        //> Read json file and then fill its data back into a new save state
        if (!File.Exists((readFilePath)))
        {
            Debug.LogWarning($"The file you are trying to read does not exist at {readFilePath}.");
            return null;
        }
        string readText = File.ReadAllText(readFilePath);
        SaveState deserializedSaveState = JsonUtility.FromJson<SaveState>(readText);
        return deserializedSaveState;
    }

    public static bool Load(SaveState saveState)
    {
        if (saveState == null)
        {
            Debug.LogWarning($"SaveState could not be obtained, it might be corrupted -> Loading of SaveState failed.");
            return false;
        }
        Dictionary<string, SaveState> allSaveStatesWithPaths = GetAllSaveStatesWithPathsInReverseOrder();
        foreach (var item in allSaveStatesWithPaths)
        {
            if (saveState.timestamp == item.Value.timestamp)
            {
                ScreenConsole.instance.Display($"Loaded \"{Path.GetFileNameWithoutExtension(item.Key)}\"...");
                // ScreenConsole.instance.Display($"Loaded \"{saveState.type.ToString()}Save-{saveState.index}\"...");
            }
            break;
        }

        //> Inject read data into their respective objects
        FindObjectOfType<TheButton>().isPressed_silent = saveState.isButtonPressed;
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

    public static void ForceAutosave()  //< Currently not necessary anywhere
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

    private static List<string> GetAllSaveFilePaths()
    {
        List<string> allSaveFilePaths = new List<string>(Directory.GetFiles(savesFolderPath));
        allSaveFilePaths.RemoveAll(x => x.Contains(FileEnding) == false);

        if (allSaveFilePaths.Count == 0)
        {
            Debug.Log($"Could not find any save files in \"{savesFolderPath}\".");
            return null;
        }

        //Debug.Log($"Found the following ({allSaveFilePaths.Count}) files:\n{PathsToFileNameString(allSaveFilePaths)}");
        return allSaveFilePaths;
    }

    public static Dictionary<string, SaveState> GetAllSaveStatesWithPathsInReverseOrder()
    {
        List<string> allSaveFilePaths = GetAllSaveFilePaths();
        allSaveFilePaths.Reverse();

        Dictionary<string, SaveState> allSaveStates = new Dictionary<string, SaveState>();
        foreach (string entry in allSaveFilePaths)
            allSaveStates.Add(entry, Read(entry));

        return allSaveStates;
    }

    public static SaveState GetLatestSaveState()
    {
        List<string> allSaveFilePaths = GetAllSaveFilePaths();
        if (allSaveFilePaths == null || allSaveFilePaths.Count == 0)
            return null;

        string filePathOfLatestSave = "";
        long latestSaveTime = 0L;
        foreach (string entry in allSaveFilePaths)
        {
            long temp = File.GetLastWriteTime(entry).ToFileTime();
            // Debug.Log($"{Path.GetFileNameWithoutExtension(entry)} was written on {File.GetLastWriteTime(entry).ToString()}.");
            if (temp > latestSaveTime)
            {
                latestSaveTime = temp;
                filePathOfLatestSave = entry;
            }
        }
        return Read(filePathOfLatestSave);
    }

    public static SaveState GetLatestSaveState(SaveType saveType)
    {
        List<string> allSaveFilePaths = GetAllSaveFilePaths();
        if (allSaveFilePaths == null || allSaveFilePaths.Count == 0)
            return null;

        string filePathOfLatestSave = "";
        System.DateTime latestSaveTime = new System.DateTime();
        foreach (string entry in allSaveFilePaths)
        {
            if (Read(entry).type == saveType)      //!< This implementation is pretty scuffed / very unclean.
            {
                System.DateTime temp = File.GetLastWriteTime(entry);
                if (temp > latestSaveTime)
                {
                    latestSaveTime = temp;
                    filePathOfLatestSave = entry;
                }
            }
        }
        return Read(filePathOfLatestSave);
    }

    public static void PurgeAllData()
    {
        List<string> filePaths;
        filePaths = new List<string>(Directory.GetFiles(dataFolderPath));
        foreach (string filePath in filePaths)
            File.Delete(filePath);

        filePaths = new List<string>(Directory.GetFiles(savesFolderPath));
        foreach (string filePath in filePaths)
            File.Delete(filePath);
    }

    //> This method exists solely for debug display purposes.
    private static string PathsToFileNameString(List<string> input)
    {
        string output = "";
        foreach (string entry in input)
        {
            output += Path.GetFileName(entry) + ", ";
        }
        output = output.Remove(output.Length - 2);
        return output;
    }
}
