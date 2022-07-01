using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveData(PlayerDataManager playerData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/main.sav";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(playerData);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData LoadData()
    {
        string path = Application.persistentDataPath + "/main.sav";
        if (File.Exists(path))
        {
            BinaryFormatter formattor = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formattor.Deserialize(stream) as PlayerData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    public static void DeleteData()
    {
        string path = Application.persistentDataPath + "/main.sav";

        try
        {
            File.Delete(path);
        }
        catch
        {
            Debug.LogError("nothing to delete");
        }
    }
}