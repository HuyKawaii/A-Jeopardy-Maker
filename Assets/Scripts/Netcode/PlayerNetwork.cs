using QFSW.QC.Actions;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    private NetworkVariable<int> point = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public delegate void OnPointChange(int newPoint);
    public OnPointChange onPointChangeCallback;
    public delegate void OnNameUpdate(string name);
    public OnNameUpdate onNameUpdateCallback;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner) 
            playerName.Value = LobbyManager.Instance.GetPlayerName();

        if (IsOwnedByServer)
            GameManager.Instance.HostInstantiateOnlineGame();
        else
            GameManager.Instance.PlayerJoin(this);

        playerName.OnValueChanged += (FixedString32Bytes oldValue, FixedString32Bytes newValue) =>
        {
            if (onNameUpdateCallback != null)
                onNameUpdateCallback(newValue.ToString());
        };

        point.OnValueChanged += (int previousValue, int newValue) =>
        {
            if (onPointChangeCallback != null)
                onPointChangeCallback(point.Value);
            Debug.Log("New point: " + point.Value);
        };

        Debug.Log("Network spawned");
    }

    public override void OnNetworkDespawn()
    {
        GameManager.Instance.PlayerLeave(this);
    }

    void Update()
    {
        if (!IsOwner || !GameManager.Instance.isOnlineGame)
            return;

        if (Input.GetKeyUp(KeyCode.Space))
            GameManager.Instance.PlayerReady(this);
    }

    [ServerRpc]
    public void TestServerRpc(ServerRpcParams serverRpcParams)
    {
        Debug.Log("This is test server rpc: senderip: " + serverRpcParams.Receive.SenderClientId);
    }

    [ClientRpc]
    public void TestClientRpc(ClientRpcParams clientRpcParams)
    {
        Debug.Log("This is client server rpc: senderip: " + clientRpcParams.Send.TargetClientIds);
    }

    public void GainPoint(int point)
    {
        this.point.Value += point;
        Debug.Log("Gain point, new point: " + this.point.Value);
    }

    public void LosePoint(int point)
    {
        this.point.Value -= point;
        Debug.Log("Lose point");
    }
}
