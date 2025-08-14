using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Project.Scripts.Car
{
    public class CarInputController : CarNetComponent, INetworkRunnerCallbacks
    {
        public struct NetworkInputData : INetworkInput
        {
            public const uint ButtonAccelerate = 1 << 0;
            public const uint ButtonReverse = 1 << 1;
            public const uint ButtonRespawn = 1 << 2;
            public const uint ButtonLookbehind = 1 << 3;

            public uint Buttons;
            public uint OneShots;

            private float steer;
            public float Steer
            {
                get => steer;
                set => steer = value;
            }

            private float acceleration;
            public float Acceleration
            {
                get => acceleration;
                set => acceleration = value;
            }

            private float reverse;
            public float Reverse
            {
                get => reverse;
                set => reverse = value;
            }

            public bool IsUp(uint button) => IsDown(button) == false;
            public bool IsDown(uint button) => (Buttons & button) == button;

            public bool IsDownThisFrame(uint button) => (OneShots & button) == button;

            public bool IsAccelerate => IsDown(ButtonAccelerate);
            public bool IsReverse => IsDown(ButtonReverse);
            public bool IsRespawnPressed => IsDown(ButtonRespawn);
            public bool IsDriftPressedThisFrame => IsDownThisFrame(ButtonRespawn);
        }

        public Gamepad gamepad;

        [SerializeField] private InputAction accelerate;
        [SerializeField] private InputAction reverse;
        [SerializeField] private InputAction respawn;
        [SerializeField] private InputAction steer;
        [SerializeField] private InputAction lookBehind;
        [SerializeField] private InputAction pause;

        private bool driftPressed;
        public bool IsLookBehindPressed => ReadBool(lookBehind);

        public override void Spawned()
        {
            base.Spawned();
            Runner.AddCallbacks(this);

            accelerate = accelerate.Clone();
            reverse = reverse.Clone();
            respawn = respawn.Clone();
            steer = steer.Clone();
            lookBehind = lookBehind.Clone();
            pause = pause.Clone();

            accelerate.Enable();
            reverse.Enable();
            respawn.Enable();
            steer.Enable();
            lookBehind.Enable();
            pause.Enable();

            respawn.started += DriftPressed;
            pause.started += PausePressed;
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);
            DisposeInputs();
            Runner.RemoveCallbacks(this);
        }

        private void OnDestroy() =>
            DisposeInputs();

        private void DisposeInputs()
        {
            accelerate.Dispose();
            reverse.Dispose();
            respawn.Dispose();
            steer.Dispose();
            lookBehind.Dispose();
            pause.Dispose();
        }

        private void DriftPressed(InputAction.CallbackContext ctx) => driftPressed = true;

        private void PausePressed(InputAction.CallbackContext ctx)
        {
            //(Car.Controller.CanDrive)InterfaceManager.Instance.OpenPauseMenu();
        }

        private static bool ReadBool(InputAction action) => action.ReadValue<float>() != 0;
        private static float ReadFloat(InputAction action) => action.ReadValue<float>();

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            gamepad = Gamepad.current;
            NetworkInputData userInput = new();
            if (ReadBool(accelerate)) userInput.Buttons |= NetworkInputData.ButtonAccelerate;
            if (ReadBool(reverse)) userInput.Buttons |= NetworkInputData.ButtonReverse;
            if (ReadBool(respawn)) userInput.Buttons |= NetworkInputData.ButtonRespawn;
            if (ReadBool(lookBehind)) userInput.Buttons |= NetworkInputData.ButtonLookbehind;
            if (driftPressed) userInput.OneShots |= NetworkInputData.ButtonRespawn;

            userInput.Steer = ReadFloat(steer);
            userInput.Acceleration = ReadFloat(accelerate);
            userInput.Reverse = ReadFloat(accelerate);
            input.Set(userInput);
            driftPressed = false;
        }

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
        public void OnConnectedToServer(NetworkRunner runner) { }
        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
        public void OnSceneLoadDone(NetworkRunner runner) { }
        public void OnSceneLoadStart(NetworkRunner runner) { }
    }
}