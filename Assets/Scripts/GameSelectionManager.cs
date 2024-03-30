using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSelectionManager : MonoBehaviour
{
    public static GameSelectionManager Instance;
    string directoryPath;
    public List<GameData> gameDataList;
    public int selectedIndex
    { private set; get; }
    public delegate void OnGameSelected(int index);
    public OnGameSelected onGameSelectedCallback;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }
        directoryPath = Application.persistentDataPath;
    }

    private void Start()
    {
        GetListOfGames();
    }

    public void GetListOfGames()
    {
        gameDataList = new List<GameData>();
        selectedIndex = -1;
        DirectoryInfo info = new DirectoryInfo(directoryPath);
        FileInfo[] files = info.GetFiles();
        foreach (FileInfo file in files)
        {
            Debug.Log(file.Name);
            if (Path.GetExtension(file.Name).EndsWith(".json"))
            {
                GameData gameData = SaveSystem.LoadGameData(file.FullName);
                if (gameData == null || gameData.title == "")
                {
                    Debug.Log("Wrong format. Skipping");
                    continue;
                }
                gameDataList.Add(gameData);
            }
        }

        SelectGameUI.Instance.DisplayGameList();
    }

    public void SelectGame(int index)
    {
        onGameSelectedCallback(index);
        selectedIndex = index;
    }

    public void ConfirmSelectedGame()
    {
        GameManager.Instance.SetGameData(gameDataList[selectedIndex]);
    }

    public string GetSelectedGameTitle()
    {
        return gameDataList[selectedIndex].title;
    }
}
