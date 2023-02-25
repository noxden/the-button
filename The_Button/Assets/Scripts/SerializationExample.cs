using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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
        exampleFilePath = dataPath + "/binaryExample.save";

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

        using (BinaryWriter writer = new BinaryWriter(File.Open(exampleFilePath, FileMode.OpenOrCreate)))
        {
            writer.Write(serializedGameObject.name);

            writer.Write(serializedGameObject.position.x);
            writer.Write(serializedGameObject.position.y);
            writer.Write(serializedGameObject.position.z);

            writer.Write(serializedGameObject.rotation.x);
            writer.Write(serializedGameObject.rotation.y);
            writer.Write(serializedGameObject.rotation.z);

            writer.Write(serializedGameObject.scale.x);
            writer.Write(serializedGameObject.scale.y);
            writer.Write(serializedGameObject.scale.z);
        }
    }

    private void Read()
    {
        SerializedGameObject deserializedGameObject = new SerializedGameObject();

        using (BinaryReader reader = new BinaryReader(File.Open(exampleFilePath, FileMode.OpenOrCreate)))
        {
             //> This is less save than JSON, as errors can occur if the binary file does not contain the information we are expecting to read here.
            deserializedGameObject.name = reader.ReadString();

            deserializedGameObject.position.x = reader.ReadSingle();
            deserializedGameObject.position.y = reader.ReadSingle();
            deserializedGameObject.position.z = reader.ReadSingle();

            deserializedGameObject.rotation.x = reader.ReadSingle();
            deserializedGameObject.rotation.y = reader.ReadSingle();
            deserializedGameObject.rotation.z = reader.ReadSingle();

            deserializedGameObject.scale.x = reader.ReadSingle();
            deserializedGameObject.scale.y = reader.ReadSingle();
            deserializedGameObject.scale.z = reader.ReadSingle();
        }

        GameObject go = new GameObject();
        go.name = deserializedGameObject.name;
        go.transform.position = deserializedGameObject.position;
        go.transform.rotation = deserializedGameObject.rotation;
        go.transform.localScale = deserializedGameObject.scale;
    }
}
