using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using QFSW.QC;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Threading.Tasks;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Mono.CSharp;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance;

    public const string PlayerNameField = "PlayerName";
    public const string RelayCodeField = "RelayCode";

    public delegate void OnLobbyCreated();
    public OnLobbyCreated onLobbyUpdatedCallback;
    private float heartBeatTimer;
    private float maxTimer = 15.0f;
    public Lobby myLobby
    { get; private set; }
    public bool isHost
    { get { return myLobby != null && myLobby.HostId == AuthenticationService.Instance.PlayerId; } }
    public int playerIndexInLobby
    {get; private set;}

    private float lobbyUpdateTimer;
    private float lobbyUpdateTimeMax = 1.1f;
    private bool joinedGame = false;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Player logged in with id: " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void Update()
    {
        HandleLobbyHeartbeatPing();
        HandleLobbyPollForUpdates();
    }

    public string GetPlayerName()
    {
        return myLobby.Players[playerIndexInLobby].Data[PlayerNameField].Value;
    }

    [Command]
    public async Task<bool> CreateLobby(string lobbyName, string playerName)
    {
        try
        {
            int maxPlayer = 8;
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                Player = new Player
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        { PlayerNameField, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
                    }
                },

                Data = new Dictionary<string, DataObject>
                {
                    { RelayCodeField, new DataObject(DataObject.VisibilityOptions.Member, "0") }
                }
            };
            myLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayer, options);
            if (onLobbyUpdatedCallback != null)
                onLobbyUpdatedCallback();

            Debug.Log("Lobby created " + myLobby.Name + ": " + myLobby.MaxPlayers);
            return true;
        }catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return false;
        }
    }

    [Command]
    public async void ListLobby()
    {
        try
        {
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync();

            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log(lobby.Name + ": " + lobby.MaxPlayers);
            }
        }catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async Task<bool> JoinLobby(string playerName, string lobbyCode)
    {
        try
        {
            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions
            {
                Player = new Player
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        { PlayerNameField, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) },
                    }
                }
            };

            myLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode.ToUpper(), options);
            playerIndexInLobby = myLobby.Players.Count - 1;

            if (onLobbyUpdatedCallback != null)
                onLobbyUpdatedCallback();
            Debug.Log("Lobby joined " + myLobby.Name + ": " + myLobby.MaxPlayers);
            return true;

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return false;
        }
    }

    [Command]
    public async void KickPlayer(string playerId)
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(myLobby.Id, playerId);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async Task<bool> LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(myLobby.Id, AuthenticationService.Instance.PlayerId);
            myLobby = null;
            return true;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return false;
        }
    }

    private async void HandleLobbyPollForUpdates()
    {
        if (myLobby != null)
        {
            lobbyUpdateTimer -= Time.deltaTime;
            if (lobbyUpdateTimer < 0)
            {
                lobbyUpdateTimer = lobbyUpdateTimeMax;

                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(myLobby.Id);

                if (lobby.Players == null)
                    myLobby = null;
                else
                myLobby = lobby;

                if (onLobbyUpdatedCallback != null)
                    onLobbyUpdatedCallback();

                if (!isHost && myLobby.Data[RelayCodeField].Value != "0" && !joinedGame)
                {
                    JoinRelay(myLobby.Data[RelayCodeField].Value);
                    GameManager.Instance.StartOnlineGame();
                }
            }
        }
    }

    private async void HandleLobbyHeartbeatPing()
    {
        if (isHost)
        {
            heartBeatTimer -= Time.deltaTime;
            if (heartBeatTimer <= 0)
            {
                heartBeatTimer = maxTimer;
                await LobbyService.Instance.SendHeartbeatPingAsync(myLobby.Id);
            }
        }
    }

    public async void StartGame()
    {
        if (isHost)
        {
            GameManager.Instance.StartOnlineGame();
            string joinCode = await CreateRelay();

            await LobbyService.Instance.UpdateLobbyAsync(myLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { RelayCodeField, new DataObject(DataObject.VisibilityOptions.Member, joinCode)}
                }
            });

            Debug.Log("Join code: " + joinCode);
        }
        
    }

    private async Task<string> CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(7);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();

            return joinCode;

        }
        catch(RelayServiceException e)
        {
            Debug.Log(e);
            return null;
        }
    }

    [Command]
    private async void JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();
            joinedGame = true;

        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async Task<bool> LeaveGame()
    {
        if (await LeaveLobby())
        {
            NetworkManager.Singleton.Shutdown();
            joinedGame = false;
            return true;
        }else
            return false;
        
    }

}
