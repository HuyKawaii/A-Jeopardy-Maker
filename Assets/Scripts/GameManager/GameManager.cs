using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    private GameData gameData;

    [HideInInspector]
    public static GameManager Instance;
    private NetworkVariable<int> questionIndex = new NetworkVariable<int>(-1);
    private NetworkVariable<int> readyPlayer = new NetworkVariable<int>(-1);
    private NetworkVariable<bool> questionReady = new NetworkVariable<bool>(false);
    private NetworkVariable<bool> isQuestionAnswered = new NetworkVariable<bool>(false);

    private NetworkVariable<int> numberOfCategory = new NetworkVariable<int>(0);
    private NetworkVariable<int> numberOfQuestionTier = new NetworkVariable<int>(0);
    private NetworkVariable<FixedString512Bytes> question = new NetworkVariable<FixedString512Bytes>("");
    private NetworkVariable<FixedString512Bytes> answer = new NetworkVariable<FixedString512Bytes>("");
    private NetworkVariable<FixedString512Bytes> titleAndCategoryListString = new NetworkVariable<FixedString512Bytes>("");
    private string title = "";
    private List<string> categoryList;
    public int questionValue { get { return gameData.CalculatePointByQuestionIndex(questionIndex.Value); } }

    public int numberOfOfflinePlayer { private set; get; }
    public bool isOnlineGame { private set; get; }
    public List<PlayerNetwork> playerList { private set; get; }
    public delegate void PlayerReadyDelegate(int playerIndex);
    public delegate void PlayerChangeDelegate(int index);
    public PlayerChangeDelegate onPlayerJoinCallback;
    public PlayerChangeDelegate onPlayerLeaveCallback;
    public delegate void NumberOfPlayerDelegate();
    public NumberOfPlayerDelegate onNumberOfPlayerChangeCallback;


    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        numberOfOfflinePlayer = 5;
        playerList = new List<PlayerNetwork>();

        questionIndex.OnValueChanged += (int oldValue, int newValue) =>{
            if (newValue >= 0)
                UIManager.Instance.isQuestionDataRequired = true;
            else
            {
                UIManager.Instance.ReturnToGrid();
                if (IsServer)
                {
                    questionReady.Value = false;
                    readyPlayer.Value = -1;
                }
                   
            }
        };

        isQuestionAnswered.OnValueChanged += (bool oldValue, bool newValue) => {
            UIManager.Instance.ToggleAnswer();
        };

        readyPlayer.OnValueChanged += (int oldValue, int newValue) => {
            if (newValue >= 0)
            {
                SoundManager.Instance.PlayAudio();
                UIManager.Instance.PlayerReadyUI(newValue);
                if (IsServer)
                    questionReady.Value = false;
            }
            else
                UIManager.Instance.UnreadyPlayerUI(oldValue);
        };

        questionReady.OnValueChanged += (bool oldValue, bool newValue) => {
            UIManager.Instance.ToggleQuestionReadyUI();
            if (IsServer)
                if (oldValue == false)
                    readyPlayer.Value = -1;
        };
    }

    void Update()
    {
        
    }

    public GameData GetGameData()
    {
        return gameData;
    }

    public void HostInstantiateOnlineGame()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            numberOfCategory.Value = gameData.numberOfCategory;
            numberOfQuestionTier.Value = gameData.numberOfQuestionTier;
            titleAndCategoryListString.Value = GetTitleAndCategoryListString();
        }
    }

    public void LoadTitleAndCategory()
    {
        if (titleAndCategoryListString.Value != "")
        {
            Debug.Log("Fixed string: " + titleAndCategoryListString.Value);
            categoryList = new List<string>();
            string titleAndCategory = titleAndCategoryListString.Value.ToString();
            Debug.Log("String: " + titleAndCategory);
            string[] titleAndCategoryList = titleAndCategory.Split(';');
            title = titleAndCategoryList[0];
            for (int i = 1; i < titleAndCategoryList.Length; i++)
            {
                categoryList.Add(titleAndCategoryList[i]);
            }
        }
       
    }

    private string GetTitleAndCategoryListString()
    {
        string value = "";
        value += gameData.title;
        
        for (int i = 0; i < gameData.numberOfCategory; i++)
        {
            value += ';' + gameData.questionList[i].category;
        }

        return value;
    }
    public void StartOnlineGame()
    {
        isOnlineGame = true;
        SceneManager.LoadScene("OnlineScene");
    }

    public void StartOfflineGame()
    {
        isOnlineGame = false;
        SceneManager.LoadScene("OfflineScene");
    }

    public void PlayerReady(PlayerNetwork player)
    {
        PlayerReadyServerRpc(playerList.IndexOf(player));
    }

    public void PlayerJoin(PlayerNetwork player)
    {
        playerList.Add(player);
        if (onPlayerJoinCallback != null)
            onPlayerJoinCallback(playerList.IndexOf(player));
        Debug.Log("New player joined");
    }

    public void PlayerLeave(PlayerNetwork player)
    {
        if (onPlayerLeaveCallback != null)
            onPlayerLeaveCallback(playerList.IndexOf(player));
        playerList.Remove(player);
        Debug.Log("A player leaved");
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayerReadyServerRpc(int index)
    {
        if (readyPlayer.Value >= 0 || !questionReady.Value)
            return;
        readyPlayer.Value = index;
    }

    [ClientRpc]
    public void SetTitleAndCategoryListClientRpc(string titleAndCategory)
    {
        categoryList = new List<string>();
        string[] titleAndCategoryList = titleAndCategory.Split(';');
        title = titleAndCategoryList[0];
        for (int i = 1; i < titleAndCategoryList.Length; i++)
        {
            categoryList.Add(titleAndCategoryList[i]);
        }
        UIManager.Instance.DisplayTitle();
        UIManager.Instance.DisplayCategory();
    }

    public void SetGameData(GameData gameData)
    {
        this.gameData = gameData;
    }

    public int GetNumberOfCategory()
    {
        return numberOfCategory.Value;
    }

    public int GetNumberOfQuestionTier()
    {
        return numberOfQuestionTier.Value;
    }

    public string GetCategory(int categoryIndex)
    {
        return categoryList[categoryIndex];
    }

    public int GetQuestionIndex()
    {
        return questionIndex.Value;
    }

    public string GetQuestion()
    {
        return question.Value.ToString();
    }

    public string GetAnswer()
    {
        return answer.Value.ToString();
    }

    public string GetTitle()
    {
        return title;
    }

    public bool IsGameReady()
    {
        return gameData != null;
    }

    public void SetQuestionReady()
    {
        if (IsServer)
            questionReady.Value = true;
    }

    public bool IsQuestionReady()
    {
        return questionReady.Value;
    }

    public void ToggleQuestionAnswered()
    {
        if (IsServer)
            isQuestionAnswered.Value = !isQuestionAnswered.Value;
    }

    public void SetQuestionAnswered(bool isAnswered)
    {
        if (IsServer)
            isQuestionAnswered.Value = isAnswered;
    }

    public bool IsQuestionAnswered()
    {
        return isQuestionAnswered.Value;
    }

    public void SetNumberOfPlayer(int numberOfPlayer)
    {
        if (numberOfPlayer < 2)
        {
            this.numberOfOfflinePlayer = 2;
        }
        else if (numberOfPlayer > 8)
            this.numberOfOfflinePlayer = 8;
        else
            this.numberOfOfflinePlayer = numberOfPlayer;

        if (onNumberOfPlayerChangeCallback != null)
            onNumberOfPlayerChangeCallback();
    }

    public void SelectQuestion(int questionIndex)
    {
        if (NetworkManager.Singleton.IsHost)
        {
            Debug.Log("Select question " + questionIndex);
            this.questionIndex.Value = questionIndex;
            question.Value = gameData.GetQuestion(questionIndex);
            answer.Value = gameData.GetAnswer(questionIndex);
        }
    }

    public void DeselectQuestion()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            questionIndex.Value = -1;
            question.Value = "";
            answer.Value = "";
        }
    }
}
