using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField]
    private RectTransform gameUI;
    [SerializeField]
    private TextMeshProUGUI gameTitle;
    [SerializeField]
    private Button leaveGameButton;
    [SerializeField]
    private RectTransform loadingScreen;

    [Header("Question grid")]
    [SerializeField]
    private RectTransform questionGrid;
    [SerializeField]
    private RectTransform questionUIPrefab;
    [SerializeField]
    private RectTransform questionContainer;

    [Header("Question content panel")]
    [SerializeField]
    private RectTransform questionContentUI;
    [SerializeField]
    private Button returnToGridButton;
    [SerializeField]
    private Button toggleAnswerButton;
    [SerializeField]
    private CanvasGroup questionContentCanvasGroup;
    [SerializeField]
    private CanvasGroup answerContentCanvasGroup;

    [Header("Category bar")]
    [SerializeField]
    private RectTransform categoryBar;
    [SerializeField]
    private RectTransform categoryCellPrefab;

    [Header("Lobby")]
    [SerializeField]
    private RectTransform playerUIPrefab;
    [SerializeField] 
    private List<RectTransform> playerUIList;
    [SerializeField]
    private RectTransform lobbyUI;
    [SerializeField]
    private Button setReadyButton;
    [SerializeField]
    private RectTransform offlinePlayerUIPrefab;

    private CanvasGroup questionGridCanvasGroup;
    private CanvasGroup categoryBarCanvasGroup;
    private int row;
    private int col;
    private List<RectTransform> questionUIs;

    private Color newQuestionColor = Color.white;
    private Color answeredQuestionColor = new Color(0, 0, 0, 0.5f);
    private Color playerColor = new Color(0, 0, 0, 0.5f);
    private Color playerReadyColor = new Color(0.8f, 0.2f, 0.2f, 0.5f);
    private Color questionNotReadyColor = new Color(1.0f, 0, 0, 0.5f);
    private Color questionReadyColor = new Color(0, 1.0f, 0, 0.5f);

    private bool isUILoaded = false;
    public bool isQuestionDataRequired = false;
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
    }

    void Start()
    {
        gameUI.gameObject.SetActive(false);
        loadingScreen.gameObject.SetActive(true);
        AssignButton();
        SetupLobbyUI();
    }

    private void Update()
    {
        HandleGameUIPollForInitialGameData();
        HandleGameUIPollForQuestionData();
    }

    private void OnDisable()
    {
        if (GameManager.Instance.isOnlineGame)
        {
            GameManager.Instance.onPlayerJoinCallback -= PlayerJoinUpdateLobbyUI;
            GameManager.Instance.onPlayerLeaveCallback -= PlayerLeaveUpdateLobbyUI;
        }
    }

    private void HandleGameUIPollForInitialGameData()
    {
        if (!isUILoaded)
            if (GameManager.Instance.GetNumberOfCategory() <= 0 || GameManager.Instance.GetNumberOfQuestionTier() <= 0 || GameManager.Instance.GetTitle() == "")
            {
                Debug.Log("Data not fully updated yet");
                GameManager.Instance.LoadTitleAndCategory();
            }
            else
            {
                DisplayTitle();
                DisplayQuestionGrid();
                DisplayCategory();
                isUILoaded = true;
                loadingScreen.gameObject.SetActive(false);
                gameUI.gameObject.SetActive(true);
            }
    }

    private void HandleGameUIPollForQuestionData()
    {
        if (isQuestionDataRequired)
        {
            if (GameManager.Instance.GetQuestion() == "" || GameManager.Instance.GetAnswer() == "")
            {
                return;
            }
            else
            {
                RevealQuestion();
                isQuestionDataRequired = false;
            }
        }
    }

    private void SetupLobbyUI()
    {
        if (GameManager.Instance.isOnlineGame)
        {
            OnlineLobbyUISetup();
        }
        else
        {
            OfflineLobbyUISetup();
        }
    }

    private void OnlineLobbyUISetup()
    {
        GameManager.Instance.onPlayerJoinCallback += PlayerJoinUpdateLobbyUI;
        GameManager.Instance.onPlayerLeaveCallback += PlayerLeaveUpdateLobbyUI;
    }

    private void OfflineLobbyUISetup()
    {
        for (int i = 0; i < GameManager.Instance.numberOfOfflinePlayer; i++)
        {
            OfflinePlayerUI offlinePlayerUI = Instantiate(offlinePlayerUIPrefab, lobbyUI).GetComponent<OfflinePlayerUI>();
            offlinePlayerUI.InitializeUI("Player " + i);
        }
    }

    private void AssignButton()
    {
        returnToGridButton.onClick.AddListener(OnReturnButtonClick);
        toggleAnswerButton.onClick.AddListener(OnToggleAnswerButtonClick);
        setReadyButton.onClick.AddListener(OnQuestionReadyButtonClick);
        leaveGameButton.onClick.AddListener(OnLeaveGameButtonClick);
    }

    public void DisplayTitle()
    {
        gameTitle.text = GameManager.Instance.GetTitle();
    }

    public void DisplayCategory()
    {
        categoryBarCanvasGroup = categoryBar.GetComponent<CanvasGroup>();
        col = GameManager.Instance.GetNumberOfCategory();
        for (int i = 0; i < col; i++)
        {
            RectTransform categoryCell = Instantiate(categoryCellPrefab, categoryBar);
            categoryCell.GetComponentInChildren<TextMeshProUGUI>().text = GameManager.Instance.GetCategory(i);
        }
    }

    public void DisplayQuestionGrid()
    {
        col = GameManager.Instance.GetNumberOfCategory();
        row = GameManager.Instance.GetNumberOfQuestionTier();

        questionGrid.GetComponent<FlexibleGridLayout>().SetSize(row, col);
        questionGridCanvasGroup = questionGrid.GetComponent<CanvasGroup>();
        questionContentUI.gameObject.SetActive(false);
        questionUIs = new List<RectTransform>();
        for (int i = 0; i < row; i++)
            for (int j = 0; j < col; j++)
            {
                RectTransform questionButton = Instantiate(questionUIPrefab, questionGrid);
                questionButton.GetComponentInChildren<TextMeshProUGUI>().text = ((i + 1) * 100).ToString();
                int index = j + i * col;
                questionButton.GetComponent<Button>().onClick.AddListener(() => OnQuestionButtonClick(index));
                questionUIs.Add(questionButton);
            }
    }

    private void OnQuestionButtonClick(int index)
    {
        GameManager.Instance.SelectQuestion(index);
    }

    private void OnReturnButtonClick()
    {
        GameManager.Instance.DeselectQuestion();
    }

    private void OnToggleAnswerButtonClick()
    {
        GameManager.Instance.ToggleQuestionAnswered();
    }

    private void OnQuestionReadyButtonClick()
    {
        GameManager.Instance.SetQuestionReady();
    }

    private async void OnLeaveGameButtonClick()
    {
        if (await LobbyManager.Instance.LeaveGame())
            SceneManager.LoadScene("MenuScene");
    }

    public void RevealQuestion()
    {
        questionContentUI.gameObject.SetActive(true);
        questionContentCanvasGroup.GetComponentInChildren<TextMeshProUGUI>().text = GameManager.Instance.GetQuestion();
        questionContentCanvasGroup.alpha = 1;
        answerContentCanvasGroup.alpha = 0;
        StartCoroutine(PlayQuestionAnimation());
        GameManager.Instance.SetQuestionAnswered(IsQuestionAnswered());
    }

    public void ReturnToGrid()
    {
        questionContentUI.gameObject.SetActive(false);
        questionGridCanvasGroup.alpha = 1.0f;
        categoryBarCanvasGroup.alpha = 1.0f;
    }

    public void ToggleAnswer()
    {
        if (GameManager.Instance.IsQuestionAnswered())
            RevealAnswer();
        else
            HideAnswer();
    }

    private void RevealAnswer()
    {
        answerContentCanvasGroup.GetComponentInChildren<TextMeshProUGUI>().text = GameManager.Instance.GetAnswer();
        questionUIs[GameManager.Instance.GetQuestionIndex()].GetComponentInChildren<TextMeshProUGUI>().color = answeredQuestionColor;
        StartCoroutine(PlayAnswerAnimation());
        toggleAnswerButton.GetComponentInChildren<TextMeshProUGUI>().text = "Hide";
    }

    private void HideAnswer()
    {
        questionUIs[GameManager.Instance.GetQuestionIndex()].GetComponentInChildren<TextMeshProUGUI>().color = newQuestionColor;
        questionContentCanvasGroup.alpha = 1;
        answerContentCanvasGroup.alpha = 0;
        toggleAnswerButton.GetComponentInChildren<TextMeshProUGUI>().text = "Show";
    }

    private bool IsQuestionAnswered()
    {
        return questionUIs[GameManager.Instance.GetQuestionIndex()].GetComponentInChildren<TextMeshProUGUI>().color == answeredQuestionColor;
    }

    private void PlayerJoinUpdateLobbyUI(int index)
    {
        PlayerNetwork newPlayer = GameManager.Instance.playerList[index];
        RectTransform playerUI = Instantiate(playerUIPrefab, lobbyUI);
        playerUIList.Add(playerUI);
        playerUI.Find("Name").GetComponent<TextMeshProUGUI>().text = newPlayer.playerName.Value.ToString();
        newPlayer.onPointChangeCallback += (int newPoint) => playerUI.Find("Score").GetComponentInChildren<TextMeshProUGUI>().text = newPoint.ToString();
        newPlayer.onNameUpdateCallback += (string name) => playerUI.Find("Name").GetComponentInChildren<TextMeshProUGUI>().text = name;
        playerUI.Find("Button/GainPointButton").GetComponent<Button>().onClick.AddListener(() => newPlayer.GainPoint(GameManager.Instance.questionValue));
        playerUI.Find("Button/LosePointButton").GetComponent<Button>().onClick.AddListener(() => newPlayer.LosePoint(GameManager.Instance.questionValue));
    }
    
    private void PlayerLeaveUpdateLobbyUI(int index)
    {
        Destroy(playerUIList[index].gameObject);
    }

    public void PlayerReadyUI(int index)
    {
        playerUIList[index].GetComponent<Image>().color = playerReadyColor;
    }

    public void UnreadyPlayerUI(int index)
    {

        playerUIList[index].GetComponent<Image>().color = playerColor;
    }

    public void ToggleQuestionReadyUI()
    {
        if (GameManager.Instance.IsQuestionReady())
            setReadyButton.GetComponent<Image>().color = questionReadyColor;
        else
            setReadyButton.GetComponent<Image>().color = questionNotReadyColor;
    }

    IEnumerator PlayAnswerAnimation()
    {
        float animationTime = 0.5f;
        float time = 0f;
        float interval = 0.01f;
        while (time < animationTime)
        {
            float alphaQuestion = Mathf.Lerp(1, 0, time / animationTime);
            float alphaAnswer = Mathf.Lerp(0, 1, time / animationTime);
            questionContentCanvasGroup.alpha = alphaQuestion;
            answerContentCanvasGroup.alpha = alphaAnswer;
            yield return new WaitForSeconds(interval);
            time += interval;
        }

        yield return null;
    }

    IEnumerator PlayQuestionAnimation()
    {
        float animationTime = 0.5f;
        float time = 0f;
        float interval = 0.01f;
        while (time < animationTime)
        {
            Vector2 scale = Vector2.Lerp(Vector2.zero, Vector2.one, time / animationTime);
            float alpha = Mathf.Lerp(1, 0, time / animationTime);
            questionContentUI.localScale = scale;
            questionGridCanvasGroup.alpha = alpha;
            categoryBarCanvasGroup.alpha = alpha;
            yield return new WaitForSeconds(interval);
            time += interval;
        }

        yield return null;
    }
}