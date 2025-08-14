using Fusion;
using Fusion.Addons.Physics;
using Fusion.Sockets;
using FusionExamples.FusionHelpers;
using Managers;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ConnectionStatus
{
    Disconnected = 0,
    Connecting = 1,
    Failed = 2,
    Connected = 3
}

[RequireComponent(typeof(LevelManager))]
public class GameLauncher : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField]
    private GameManager gameManagerPrefab;
    [SerializeField]
    private RoomPlayer roomPlayerPrefab;
    [SerializeField]
    private DisconnectUI disconnectUI;

    public static ConnectionStatus ConnectionStatus = ConnectionStatus.Disconnected;

    private GameMode gameMode;
    private NetworkRunner runner;
    private FusionObjectPoolRoot pool;
    private LevelManager levelManager;

    private void Start()
    {
        Application.runInBackground = true;
        Application.targetFrameRate = (int)Math.Round(Screen.currentResolution.refreshRateRatio.value);
        QualitySettings.vSyncCount = 1;

        levelManager = GetComponent<LevelManager>();

        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene(LevelManager.LOBBY_SCENE);
        AudioManager.PlayMusic("menu");
    }

    public void SetCreateLobby() => gameMode = GameMode.Host;
    public void SetJoinLobby() => gameMode = GameMode.Client;

    public void JoinOrCreateLobby()
    {
        SetConnectionStatus(ConnectionStatus.Connecting);

        if (runner != null)
            LeaveSession();

        GameObject go = new GameObject("Session");
        DontDestroyOnLoad(go);

        runner = go.AddComponent<NetworkRunner>();
        var sim3D = go.AddComponent<RunnerSimulatePhysics3D>();
        sim3D.ClientPhysicsSimulation = ClientPhysicsSimulation.SimulateAlways;

        runner.ProvideInput = gameMode != GameMode.Server;
        runner.AddCallbacks(this);

        pool = go.AddComponent<FusionObjectPoolRoot>();
#if UNITY_EDITOR
        Debug.Log($"Created gameobject {go.name} - starting game");
#endif

        runner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            SessionName = gameMode == GameMode.Host ? ServerInfo.LobbyName : ClientInfo.LobbyName,
            ObjectProvider = pool,
            SceneManager = levelManager,
            PlayerCount = ServerInfo.MaxUsers,
            EnableClientSessionCreation = false
        });
    }

    private void SetConnectionStatus(ConnectionStatus status)
    {
#if UNITY_EDITOR
        Debug.Log($"Setting connection status to {status}");
#endif
        ConnectionStatus = status;

        if (!Application.isPlaying)
            return;

        if (status == ConnectionStatus.Disconnected || status == ConnectionStatus.Failed)
        {
            SceneManager.LoadScene(LevelManager.LOBBY_SCENE);
            UIScreen.BackToInitial();
        }
    }

    public void LeaveSession()
    {
        if (runner != null) runner.Shutdown();
        else SetConnectionStatus(ConnectionStatus.Disconnected);
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
#if UNITY_EDITOR
        Debug.Log("Connected to server");
#endif
        SetConnectionStatus(ConnectionStatus.Connected);
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
#if UNITY_EDITOR
        Debug.Log("Disconnected from server");
#endif
        LeaveSession();
        SetConnectionStatus(ConnectionStatus.Disconnected);
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        if (runner.TryGetSceneInfo(out var scene) && scene.SceneCount > 0)
        {
            Debug.LogWarning($"Refused connection requested by {request.RemoteAddress}");
            request.Refuse();
        }
        else
            request.Accept();
    }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
#if UNITY_EDITOR
        Debug.Log($"Connect failed {reason}");
#endif
        LeaveSession();
        SetConnectionStatus(ConnectionStatus.Failed);
        (string status, string message) = ConnectFailedReasonToHuman(reason);
        disconnectUI.ShowMessage(status, message);
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
#if UNITY_EDITOR
        Debug.Log($"Player {player} Joined!");
#endif
        if (runner.IsServer)
        {
            if (gameMode == GameMode.Host)
                runner.Spawn(gameManagerPrefab, Vector3.zero, Quaternion.identity);
            var roomPlayer = runner.Spawn(roomPlayerPrefab, Vector3.zero, Quaternion.identity, player);
            roomPlayer.GameState = RoomPlayer.EGameState.Lobby;
        }
        SetConnectionStatus(ConnectionStatus.Connected);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
#if UNITY_EDITOR
        Debug.Log($"{player.PlayerId} disconnected.");
#endif

        RoomPlayer.RemovePlayer(runner, player);

        SetConnectionStatus(ConnectionStatus);
    }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
#if UNITY_EDITOR
        Debug.Log($"OnShutdown {shutdownReason}");
#endif

        SetConnectionStatus(ConnectionStatus.Disconnected);

        (string status, string message) = ShutdownReasonToHuman(shutdownReason);
        disconnectUI.ShowMessage(status, message);

        RoomPlayer.Players.Clear();

        if (this.runner)
            Destroy(this.runner.gameObject);

        pool.ClearPools();
        pool = null;

        this.runner = null;
    }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }

    private static (string, string) ShutdownReasonToHuman(ShutdownReason reason)
    {
        switch (reason)
        {
            case ShutdownReason.Ok:
                return (null, null);
            case ShutdownReason.Error:
                return ("Error", "Shutdown was caused by some internal error");
            case ShutdownReason.IncompatibleConfiguration:
                return ("Incompatible Config", "Mismatching type between client Server Mode and Shared Mode");
            case ShutdownReason.ServerInRoom:
                return ("Room name in use", "There's a room with that name! Please try a different name or wait a while.");
            case ShutdownReason.DisconnectedByPluginLogic:
                return ("Disconnected By Plugin Logic", "You were kicked, the room may have been closed");
            case ShutdownReason.GameClosed:
                return ("Game Closed", "The session cannot be joined, the game is closed");
            case ShutdownReason.GameNotFound:
                return ("Game Not Found", "This room does not exist");
            case ShutdownReason.MaxCcuReached:
                return ("Max Players", "The Max CCU has been reached, please try again later");
            case ShutdownReason.InvalidRegion:
                return ("Invalid Region", "The currently selected region is invalid");
            case ShutdownReason.GameIdAlreadyExists:
                return ("ID already exists", "A room with this name has already been created");
            case ShutdownReason.GameIsFull:
                return ("Game is full", "This lobby is full!");
            case ShutdownReason.InvalidAuthentication:
                return ("Invalid Authentication", "The Authentication values are invalid");
            case ShutdownReason.CustomAuthenticationFailed:
                return ("Authentication Failed", "Custom authentication has failed");
            case ShutdownReason.AuthenticationTicketExpired:
                return ("Authentication Expired", "The authentication ticket has expired");
            case ShutdownReason.PhotonCloudTimeout:
                return ("Cloud Timeout", "Connection with the Photon Cloud has timed out");
            default:
                Debug.LogWarning($"Unknown ShutdownReason {reason}");
                return ("Unknown Shutdown Reason", $"{(int)reason}");
        }
    }

    private static (string, string) ConnectFailedReasonToHuman(NetConnectFailedReason reason)
    {
        switch (reason)
        {
            case NetConnectFailedReason.Timeout:
                return ("Timed Out", "");
            case NetConnectFailedReason.ServerRefused:
                return ("Connection Refused", "The lobby may be currently in-game");
            case NetConnectFailedReason.ServerFull:
                return ("Server Full", "");
            default:
                Debug.LogWarning($"Unknown NetConnectFailedReason {reason}");
                return ("Unknown Connection Failure", $"{(int)reason}");
        }
    }
}