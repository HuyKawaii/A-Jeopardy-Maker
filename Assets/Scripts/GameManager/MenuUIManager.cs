using Mono.CSharp.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUIManager : MonoBehaviour
{
    [Header("Main menu")]
    [SerializeField]
    private RectTransform mainMenu;
    [SerializeField]
    private RectTransform mainMenuOptions;
    [SerializeField]
    private Button playOnlineButton;
    [SerializeField]
    private Button playOfflineButton;
    [SerializeField]
    private Button createGameButton;

    [Header("Online menu")]
    [SerializeField]
    private RectTransform onlineMenuOptions;
    [SerializeField]
    private Button createLobbyButton;
    [SerializeField]
    private Button joinLobbyButton;
    [SerializeField]
    private Button backButton;

    [Header("Create lobby menu")]
    [SerializeField]
    private RectTransform createLobbyMenu;
    [SerializeField]
    private TMP_InputField createPlayerNameInput;
    [SerializeField]
    private TMP_InputField createLobbyNameInput;
    [SerializeField]
    private Button confirmCreateLobbyButton;
    [SerializeField]
    private Button cancelCreateLobbyButton;
    [SerializeField]
    private CanvasGroup createLobbyAlert;

    [Header("Join lobby menu")]
    [SerializeField]
    private RectTransform joinLobbyMenu;
    [SerializeField]
    private TMP_InputField joinPlayerNameInput;
    [SerializeField]
    private TMP_InputField joinLobbyCodeInput;
    [SerializeField]
    private Button confirmJoinLobbyButton;
    [SerializeField]
    private Button cancelJoinLobbyButton;
    [SerializeField]
    private CanvasGroup joinLobbyAlert;

    [Header("Loading screen")]
    [SerializeField]
    private RectTransform loadingScreen;

    [Header("Lobby menu")]
    [SerializeField]
    private TextMeshProUGUI lobbyName;
    [SerializeField]
    private RectTransform lobbyMenu;
    [SerializeField]
    private Button leaveLobbyButton;
    [SerializeField] 
    private RectTransform playerGrid;
    [SerializeField]
    private RectTransform playerLobbyCellPrefab;
    [SerializeField]
    private TextMeshProUGUI lobbyCodeText;
    [SerializeField]
    private TextMeshProUGUI playerCount;
    [SerializeField]
    private Button startOnlineGameButton;

    [Header("Offline game menu")]
    [SerializeField]
    private RectTransform offlineGameMenu;
    [SerializeField]
    private Button returnFromOfflineMenuButton;
    [SerializeField]
    private Button upButton;
    [SerializeField]
    private Button downButton;
    [SerializeField]
    private TMP_InputField numberOfPlayersInputField;
    [SerializeField]
    private Button startOfflineGameButton;

    [Header("Create game menu")]
    [SerializeField]
    private RectTransform createGameMenu;
    [SerializeField]
    private Button createNewGameButton;
    [SerializeField]
    private Button editExistingGameButton;
    [SerializeField]
    private Button backFromCreateGameMenuButton;

    [Header("Choose game menu")]
    [SerializeField]
    private RectTransform chooseGameMenu;
    [SerializeField]
    private Button editButton;
    [SerializeField]
    private Button chooseGameBackButton;

    [Header("How to play menu")]
    [SerializeField]
    private Button howToPlayButton;
    [SerializeField]
    private RectTransform howToPlayPanel;
    [SerializeField]
    private Button closeHowToPlayPanelButton;

    [Header("")]
    [SerializeField]
    private RectTransform kickFromLobbyText;

    private Color baseInputFieldColor = new Color(0.15f, 0.08f, 0.08f);
    private Color warningInputFieldColor = Color.red;
    private float inputWarningTimer;
    private float inputWarningTimerMax = 0.5f;
    private float kickFromLobbyTimer;
    private float kickedFromLobbyTimerMax = 1.0f;
    private float createJoinAlertTimer;
    private float createJoinAlertTimeMax = 2.0f;
    private bool lobbyInputWarning;
    private bool playerNameInputWarning;
    private void Start()
    {
        AssignButtons();
        InitilizeMenu();

        LobbyManager.Instance.onLobbyUpdatedCallback += UpdateLobbyUI;
        GameManager.Instance.onNumberOfPlayerChangeCallback += UpdateNumberOfPlayerInput;

        numberOfPlayersInputField.onEndEdit.AddListener(OnNumberOfPlayersInputFieldEdit);
    }

    private void Update()
    {
        HandleInputFieldWarning();
        HandleCreateJoinLobbyFailAlert();
        HandleKickedFromLobbyWarning();
    }

    private void OnDisable()
    {
        LobbyManager.Instance.onLobbyUpdatedCallback -= UpdateLobbyUI;
        GameManager.Instance.onNumberOfPlayerChangeCallback -= UpdateNumberOfPlayerInput;
    }

    private void AssignButtons()
    {
        playOnlineButton.onClick.AddListener(OnPlayOnlineButtonClick);
        playOfflineButton.onClick.AddListener(OnPlayOfflineButtonClick);
        createGameButton.onClick.AddListener(OnCreateGameButtonClick);
        createLobbyButton.onClick.AddListener(OnCreateLobbyButtonClick);
        joinLobbyButton.onClick.AddListener(OnJoinLobbyButtonClick);
        cancelCreateLobbyButton.onClick.AddListener(OnCancelCreateLobbyButtonClick);
        confirmCreateLobbyButton.onClick.AddListener(OnConfirmCreateLobbyButtonClick);
        cancelJoinLobbyButton.onClick.AddListener(OnCancelJoinLobbyButtonClick);
        confirmJoinLobbyButton.onClick.AddListener(OnConfirmJoinLobbyButtonClick);
        leaveLobbyButton.onClick.AddListener(OnLeaveLobbyButtonClick);
        startOnlineGameButton.onClick.AddListener(OnStartOnlineGameButtonClick);
        backButton.onClick.AddListener(OnBackButtonClick);
        returnFromOfflineMenuButton.onClick.AddListener(OnReturnFromOfflineMenuButtonClick);
        upButton.onClick.AddListener(OnUpButtonClick);
        downButton.onClick.AddListener(OnDownButtonClick);
        startOfflineGameButton.onClick.AddListener(OnStartOfflineGameButtonClick);
        howToPlayButton.onClick.AddListener(OnHowToPlayButtonClick);
        closeHowToPlayPanelButton.onClick.AddListener(OnCloseHowToPlayPanelButtonClick);
        createNewGameButton.onClick.AddListener(OnCreateNewGameButtonClick);
        backFromCreateGameMenuButton.onClick.AddListener(OnBackFromCreateGameMenuButtonClick);
        editExistingGameButton.onClick.AddListener(OnEditExistingGameButtonClick);
        editButton.onClick.AddListener(OnEditButtonClick);
        chooseGameBackButton.onClick.AddListener(OnChooseGameBackButtonClick);
    }

    private void InitilizeMenu()
    {
        mainMenuOptions.gameObject.SetActive(true);
        onlineMenuOptions.gameObject.SetActive(false);
        createLobbyMenu.gameObject.SetActive(false);
        lobbyMenu.gameObject.SetActive(false);
        loadingScreen.gameObject.SetActive(false);
        joinLobbyMenu.gameObject.SetActive(false);
        offlineGameMenu.gameObject.SetActive(false);
        chooseGameMenu.gameObject.SetActive(false);
        howToPlayPanel.gameObject.SetActive(false);
        createGameMenu.gameObject.SetActive(false);
    }

    private void OnChooseGameBackButtonClick()
    {
        chooseGameMenu.gameObject.SetActive(false);
        mainMenuOptions.gameObject.SetActive(true);
    }

    private void OnEditButtonClick()
    {
        SceneManager.LoadScene("CreateScene");
    }

    private void OnCreateGameButtonClick()
    {
        createGameMenu.gameObject.SetActive(true);
        mainMenuOptions.gameObject.SetActive(false);
    }

    private void OnEditExistingGameButtonClick()
    {
        ShowFullChooseGameMenu();
        createGameMenu.gameObject.SetActive(false);
    }

    private void OnBackFromCreateGameMenuButtonClick()
    {
        createGameMenu.gameObject.SetActive(false);
        mainMenuOptions.gameObject.SetActive(true);
    }

    private void OnHowToPlayButtonClick()
    {
        howToPlayPanel.gameObject.SetActive(true);
    }

    private void OnCloseHowToPlayPanelButtonClick()
    {
        howToPlayPanel.gameObject.SetActive(false);
    }

    private void OnBackButtonClick()
    {
        mainMenuOptions.gameObject.SetActive(true);
        onlineMenuOptions.gameObject.SetActive(false);
    }

    private void OnPlayOnlineButtonClick()
    {
        onlineMenuOptions.gameObject.SetActive(true);
        mainMenuOptions.gameObject.SetActive(false);
    }

    private void OnPlayOfflineButtonClick()
    {
        offlineGameMenu.gameObject.SetActive(true);
        ShowPartialChooseGameMenu();
        mainMenu.gameObject.SetActive(false);
    }

    private void OnReturnFromOfflineMenuButtonClick()
    {
        offlineGameMenu.gameObject.SetActive(false);
        chooseGameMenu.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(true);
    }

    private void OnCreateNewGameButtonClick()
    {
        GameManager.Instance.SetGameData(null);
        SceneManager.LoadScene("CreateScene");
    }

    private void OnCreateLobbyButtonClick()
    {
        createLobbyMenu.gameObject.SetActive(true);
        onlineMenuOptions.gameObject.SetActive(false);
    }

    private void OnJoinLobbyButtonClick()
    {
        joinLobbyMenu.gameObject.SetActive(true);
        onlineMenuOptions.gameObject.SetActive(false);
    }

    private void OnCancelCreateLobbyButtonClick()
    {
        createLobbyMenu.gameObject.SetActive(false);
        mainMenuOptions.gameObject.SetActive(true);
    }

    private bool CheckCreateLobbyInputField()
    {
        if (createLobbyNameInput.text == "")
        {
            lobbyInputWarning = true;
            inputWarningTimer = inputWarningTimerMax;
        }
        if (createPlayerNameInput.text == "")
        {
            playerNameInputWarning = true;
            inputWarningTimer = inputWarningTimerMax;
        }

        if (lobbyInputWarning | playerNameInputWarning)
            return false;

        return true;
    }

    private async void OnConfirmCreateLobbyButtonClick()
    {
        if (!CheckCreateLobbyInputField())
            return;

        createLobbyMenu.gameObject.SetActive(false);
        loadingScreen.gameObject.SetActive(true);

        if (!await LobbyManager.Instance.CreateLobby(createLobbyNameInput.text, createPlayerNameInput.text))
        {
            loadingScreen.gameObject.SetActive(false);
            createLobbyMenu.gameObject.SetActive(true);
            createJoinAlertTimer = createJoinAlertTimeMax;
        }
        
    }

    private void OnCancelJoinLobbyButtonClick()
    {
        joinLobbyMenu.gameObject.SetActive(false);
        mainMenuOptions.gameObject.SetActive(true);
    }

    private bool CheckJoinLobbyInputField()
    {
        if (joinLobbyCodeInput.text == "")
        {
            lobbyInputWarning = true;
            inputWarningTimer = inputWarningTimerMax;
        }
        if (joinPlayerNameInput.text == "")
        {
            playerNameInputWarning = true;
            inputWarningTimer = inputWarningTimerMax;
        }

        if (lobbyInputWarning | playerNameInputWarning)
            return false;

        return true;
    }

    private async void OnConfirmJoinLobbyButtonClick()
    {
        if (!CheckJoinLobbyInputField())
            return;

        joinLobbyMenu.gameObject.SetActive(false);
        loadingScreen.gameObject.SetActive(true);

        if (!await LobbyManager.Instance.JoinLobby(joinPlayerNameInput.text, joinLobbyCodeInput.text))
        {
            joinLobbyMenu.gameObject.SetActive(true);
            loadingScreen.gameObject.SetActive(false);
            createJoinAlertTimer = createJoinAlertTimeMax;
        }
        
    }

    private async void OnLeaveLobbyButtonClick()
    {
        if (await LobbyManager.Instance.LeaveLobby())
        {
            mainMenuOptions.gameObject.SetActive(true);
            lobbyMenu.gameObject.SetActive(false);
            chooseGameMenu.gameObject.SetActive(false);
        }
       
    }

    private void OnUpButtonClick()
    {
        GameManager.Instance.SetNumberOfPlayer(GameManager.Instance.numberOfOfflinePlayer + 1);
    }

    private void OnDownButtonClick()
    {
        GameManager.Instance.SetNumberOfPlayer(GameManager.Instance.numberOfOfflinePlayer - 1);
    }

    private void OnNumberOfPlayersInputFieldEdit(string input)
    {
        GameManager.Instance.SetNumberOfPlayer(Convert.ToInt32(input));
    }

    private void ShowPartialChooseGameMenu()
    {
        chooseGameMenu.gameObject.SetActive(true);
        editButton.gameObject.SetActive(false);
        chooseGameBackButton.gameObject.SetActive(false);
    }

    private void ShowFullChooseGameMenu()
    {
        chooseGameMenu.gameObject.SetActive(true);
        editButton.gameObject.SetActive(true);
        chooseGameBackButton.gameObject.SetActive(true);
    }

    private void UpdateLobbyUI()
    {
        if (LobbyManager.Instance.myLobby != null)
        {
            
            loadingScreen.gameObject.SetActive(false);
            lobbyMenu.gameObject.SetActive(true);

            if (LobbyManager.Instance.isHost)
            {
                ShowPartialChooseGameMenu();
                startOnlineGameButton.gameObject.SetActive(true);
            }
            else
            {
                startOnlineGameButton.gameObject.SetActive(false);
            }

            RectTransform[] playerCellList = playerGrid.GetComponentsInChildren<RectTransform>();

            lobbyName.text = LobbyManager.Instance.myLobby.Name;

            for (int i = 1; i < playerCellList.Length; i++)
            {
                Destroy(playerCellList[i].gameObject);
            }

            Lobby thisLobby = LobbyManager.Instance.myLobby;
            foreach (Player player in thisLobby.Players)
            {
                string playerId = player.Id;

                RectTransform playerLobbyCell = Instantiate(playerLobbyCellPrefab, playerGrid);
                playerLobbyCell.Find("Name").GetComponent<TextMeshProUGUI>().text = player.Data[LobbyManager.PlayerNameField].Value;

                if (thisLobby.HostId == playerId)
                    playerLobbyCell.Find("HostIcon").gameObject.SetActive(true);

                if (LobbyManager.Instance.isHost)
                {
                    if (thisLobby.HostId != playerId)
                    {
                        playerLobbyCell.Find("KickButton").GetComponent<Button>().onClick.AddListener(() => LobbyManager.Instance.KickPlayer(playerId));
                        playerLobbyCell.Find("KickButton").gameObject.SetActive(true);
                    }
                }
            }

            lobbyCodeText.text = "Code: " + thisLobby.LobbyCode;
            playerCount.text = thisLobby.Players.Count + "/8";
        }
        else       
        {
            mainMenuOptions.gameObject.SetActive(true);
            lobbyMenu.gameObject.SetActive(false);
            kickFromLobbyTimer = kickedFromLobbyTimerMax;
            return;
        }
    }

    private void HandleInputFieldWarning()
    {
        if (inputWarningTimer > 0)
        {
            inputWarningTimer -= Time.deltaTime;
            Color inputFieldColor = Color.Lerp(baseInputFieldColor, warningInputFieldColor, Mathf.Clamp01(inputWarningTimer / inputWarningTimerMax));
            if (lobbyInputWarning)
            {
                createLobbyNameInput.GetComponent<Image>().color = inputFieldColor;
                joinLobbyCodeInput.GetComponent<Image>().color = inputFieldColor;
            }
            if (playerNameInputWarning)
            {
                createPlayerNameInput.GetComponent<Image>().color = inputFieldColor;
                joinPlayerNameInput.GetComponent <Image>().color = inputFieldColor;
            }
        }
        else
        {
            lobbyInputWarning = false;
            playerNameInputWarning = false;
        }
    }

    private void HandleCreateJoinLobbyFailAlert()
    {
        if (createJoinAlertTimer > 0)
        {
            createJoinAlertTimer -= Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, createJoinAlertTimer / createJoinAlertTimeMax);
            createLobbyAlert.alpha = alpha;
            joinLobbyAlert.alpha = alpha;
        }
    }

    private void HandleKickedFromLobbyWarning()
    {
        if (kickFromLobbyTimer > 0)
        {
            kickFromLobbyText.gameObject.SetActive(true);
            kickFromLobbyTimer -= Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, kickFromLobbyTimer / kickedFromLobbyTimerMax);
            kickFromLobbyText.GetComponent<CanvasGroup>().alpha = alpha;
        }
        else
        {
            kickFromLobbyText.gameObject.SetActive(false);
        }
    }

    private void OnStartOnlineGameButtonClick()
    {
        if (GameManager.Instance.IsGameReady())
            LobbyManager.Instance.StartGame();
        else
            SelectGameUI.Instance.FlickerChooseGameText();
    }

    private void OnStartOfflineGameButtonClick()
    {
        if (GameManager.Instance.IsGameReady())
            GameManager.Instance.StartOfflineGame();
        else
            SelectGameUI.Instance.FlickerChooseGameText();
    }

    private void DeactiveMainMenu()
    {
        mainMenu.gameObject.SetActive(false);
        onlineMenuOptions.gameObject.SetActive(false);
    }

    private void UpdateNumberOfPlayerInput()
    {
        numberOfPlayersInputField.text = GameManager.Instance.numberOfOfflinePlayer.ToString();
    }
}
