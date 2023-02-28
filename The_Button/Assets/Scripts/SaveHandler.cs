//================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:       GameDev Tips & Tricks (Thomas Valentin Klink)
// Script by:    Daniel Heilmann (771144)
// Last changed: 27-02-23
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
    public int manualSaveIndex;
    public int autosaveIndex;
    public int quicksaveIndex;
}

public class SaveHandler : MonoBehaviour
{
    private const string FileEnding = ".save";

    public static SaveHandler instance;

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
    void Start()
    {
        ReadPersistentData();
        Load(GetLatestSaveState());
    }

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
    public static bool Save(SaveType saveType)     //< Returns whether saving was successful.    //? Potential properties: SaveType, ...
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
        saveState.type = saveType;
        saveState.index = GetSaveIndex(saveType);
        saveState.isButtonPressed = FindObjectOfType<TheButton>().isPressed;

        //> Convert to json and then save in file
        string serializedSaveState = JsonUtility.ToJson(saveState, true);
        File.WriteAllText(writeFilePath, serializedSaveState);

        data.manualSaveIndex++;
        return true;
    }


    public static SaveState Read(string readFilePath)   //< Returns whether loading was successful.    //? Potential property: FilePath which should be read from
    {
        Debug.Log($"Reading \"{Path.GetFileNameWithoutExtension(readFilePath)}\"...");

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
        Debug.Log($"Loading \"{saveState.type} {saveState.index}\"...");

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

        Debug.Log($"Found the following ({allSaveFilePaths.Count}) files:\n{PathsToFileNameString(allSaveFilePaths)}");
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
        if (allSaveFilePaths.Count == 0)     //< Not necessary, as giving "null" to Load() just does not load anything -> Scene stays unchanged. 
            return new SaveState();          //< And because the default scene is equal to one loaded from a new SaveState, there should be no noticable difference.

        string filePathOfLatestSave = "";
        System.DateTime latestSaveTime = new System.DateTime();
        foreach (string entry in allSaveFilePaths)
        {
            System.DateTime temp = File.GetLastWriteTime(entry);
            if (temp > latestSaveTime)
            {
                latestSaveTime = temp;
                filePathOfLatestSave = entry;
            }
        }
        return Read(filePathOfLatestSave);
    }

    private static int GetSaveIndex(SaveType saveType)
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
