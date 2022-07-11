using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveData(PlayerDataManager playerData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.sav";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(playerData);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static void SaveData(MenuDataManager menuData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/menu.sav";
        FileStream stream = new FileStream(path, FileMode.Create);

        MenuData data = new MenuData(menuData);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData LoadPlayerData()
    {
        string path = Application.persistentDataPath + "/player.sav";
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

    public static MenuData LoadMenuData()
    {
        string path = Application.persistentDataPath + "/menu.sav";
        if (File.Exists(path))
        {
            BinaryFormatter formattor = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            MenuData data = formattor.Deserialize(stream) as MenuData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    public static void DeletePlayerData()
    {
        string path = Application.persistentDataPath + "/player.sav";

        try
        {
            File.Delete(path);
        }
        catch
        {
            Debug.LogError("nothing to delete");
        }
    }

    public static void DeleteMenuData()
    {
        string path = Application.persistentDataPath + "/menu.sav";

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