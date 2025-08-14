using Assets.Project.Scripts.Car;
using Fusion;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomPlayer : NetworkBehaviour
{
    public enum EGameState
    {
        Lobby,
        GameCutscene,
        GameReady
    }

    public static readonly List<RoomPlayer> Players = new List<RoomPlayer>();

    public static Action<RoomPlayer> PlayerJoined;
    public static Action<RoomPlayer> PlayerLeft;
    public static Action<RoomPlayer> PlayerChanged;
    public static RoomPlayer Local;

    [Networked]
    public NetworkBool IsReady { get; set; }
    [Networked]
    public NetworkString<_32> Username { get; set; }
    [Networked]
    public NetworkBool HasFinished { get; set; }
    [Networked]
    public CarMovementController Car { get; set; }
    [Networked]
    public EGameState GameState { get; set; }
    [Networked]
    public int CarId { get; set; }
    [Networked]
    public int CarBodyId { get; set; }
    [Networked]
    public int CarWheelID { get; set; }
    [Networked]
    public int CarSpolerID { get; set; }

    public bool IsLeader => Object != null && Object.IsValid && Object.HasStateAuthority;

    private ChangeDetector changeDetector;

    public override void Spawned()
    {
        base.Spawned();

        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

        if (Object.HasInputAuthority)
        {
            Local = this;

            PlayerChanged?.Invoke(this);
            RPC_SetPlayerStats(SaveManager.Instance.GetUserData().Username, SaveManager.Instance.GetUserData().ActiveCarId);
            var carStats = SaveManager.Instance.GetUserData().CarsModificationData[SaveManager.Instance.GetUserData().ActiveCarId];
            RPC_SetPlayerCarStats(carStats.BodyId, carStats.WheelId, carStats.SpoilerId);
        }

        Players.Add(this);
        PlayerJoined?.Invoke(this);

        DontDestroyOnLoad(gameObject);
    }

    public override void Render()
    {
        foreach (var change in changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(IsReady):
                case nameof(Username):
                    OnStateChanged(this);
                    break;
            }
        }
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RPC_SetPlayerStats(NetworkString<_32> username, int kartId)
    {
        Username = username;
        CarId = kartId;
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RPC_SetPlayerCarStats(int carBodyId, int carWheelId, int carSpoilerId)
    {
        CarBodyId = carBodyId;
        CarWheelID = carWheelId;
        CarSpolerID = carSpoilerId;
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    public void RPC_SetCarId(int id)
    {
        CarId = id;
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    public void RPC_SetCarBodyId(int id)
    {
        CarBodyId = id;
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    public void RPC_SetCarWheelId(int id)
    {
        CarWheelID = id;
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    public void RPC_SetCarSpoilerId(int id)
    {
        CarSpolerID = id;
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    public void RPC_ChangeReadyState(NetworkBool state)
    {
        Debug.Log($"Setting {Object.Name} ready state to {state}");
        IsReady = state;
    }

    private void OnDisable()
    {
        PlayerLeft?.Invoke(this);
        Players.Remove(this);
    }

    private static void OnStateChanged(RoomPlayer changed) => PlayerChanged?.Invoke(changed);

    public static void RemovePlayer(NetworkRunner runner, PlayerRef p)
    {
        var roomPlayer = Players.FirstOrDefault(x => x.Object.InputAuthority == p);
        if (roomPlayer != null)
        {
            if (roomPlayer.Car != null)
                runner.Despawn(roomPlayer.Car.Object);

            Players.Remove(roomPlayer);
            runner.Despawn(roomPlayer.Object);
        }
    }
}