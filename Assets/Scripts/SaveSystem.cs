using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    public static bool SaveData(GameData gameData)
    {
        string jsonData = JsonUtility.ToJson(gameData);
        string saveFilePath = Application.persistentDataPath + "/" + gameData.GetTitle() + ".json";
        Debug.Log("Saving to: " + saveFilePath);

        try
        {
            File.WriteAllText(saveFilePath, jsonData);
            Debug.Log("Saved to: " + saveFilePath);
            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return false;
        }
        
    }

    public static GameData LoadGameData(string path)
    {
        string jsonData = File.ReadAllText(path);
        Debug.Log(jsonData);
        try
        {
            GameData gameData = JsonUtility.FromJson<GameData>(jsonData);
            return gameData;
        }
        catch(Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }
}
