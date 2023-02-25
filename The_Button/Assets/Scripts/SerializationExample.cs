using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializedGameObject
{
    public string name;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
}

public class SerializationExample : MonoBehaviour
{
    string exampleFilePath;

    void Start()
    {
        string dataPath = Application.persistentDataPath;   //< The persistentDataPath refers to a folder in AppData
        exampleFilePath = dataPath + "/jsonExample.json";

        Write();
        Read();
    }

    private void Write()
    {
        SerializedGameObject serializedGameObject = new SerializedGameObject();
        serializedGameObject.name = gameObject.name;
        serializedGameObject.position = transform.position;
        serializedGameObject.rotation = transform.rotation;
        serializedGameObject.scale = transform.localScale;


        string result = JsonUtility.ToJson(serializedGameObject, true);
        Debug.Log($"{result}");

        System.IO.File.WriteAllText(exampleFilePath, result);
    }

    private void Read()
    {
        string readText = System.IO.File.ReadAllText(exampleFilePath);
        SerializedGameObject deserializedGameObject = JsonUtility.FromJson<SerializedGameObject>(readText);
        GameObject go = new GameObject();
        go.name = deserializedGameObject.name;
        go.transform.position = deserializedGameObject.position;
        go.transform.rotation = deserializedGameObject.rotation;
        go.transform.localScale = deserializedGameObject.scale;
    }
}
