using Fusion;
using Fusion.Addons.Physics;
using System;
using UnityEngine;

namespace Assets.Project.Scripts.Car
{
    public class CarLapController : CarNetComponent
    {
        public static event Action<CarLapController> OnRaceCompleted;

        [Networked]
        public int Lap { get; set; } = 1;
        [Networked, Capacity(5)]
        public NetworkArray<int> LapTicks { get; }
        [Networked]
        public int StartRaceTick { get; set; }
        [Networked]
        public int EndRaceTick { get; set; }
        [Networked]
        private int CheckpointIndex { get; set; } = -1;

        public event Action<int, int> OnLapChanged;
        public bool HasFinished => EndRaceTick != 0;

        private CarMovementController Controller => CarEntity.CarControllerHandler;
        private GameUI Hud => CarEntity.Hud;
        private NetworkRigidbody3D nrb;
        private ChangeDetector changeDetector;

        private void Awake() =>
            nrb = GetComponent<NetworkRigidbody3D>();

        public override void Spawned()
        {
            base.Spawned();
            changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
            Lap = 1;
            var maxLaps = GameManager.Instance.GameType.lapCount;
            this.OnLapChanged?.Invoke(Lap, maxLaps);
        }

        public override void Render()
        {
            foreach (var change in changeDetector.DetectChanges(this))
            {
                switch (change)
                {
                    case nameof(Lap):
                        OnLapChangedCallback(this);
                        break;
                    case nameof(CheckpointIndex):
                        CheckpointIndexChanged(this);
                        break;
                }
            }
        }

        public override void OnRaceStart()
        {
            base.OnRaceStart();
            StartRaceTick = Runner.Tick;
        }

        public override void OnLapCompleted(int lap, bool isFinish)
        {
            base.OnLapCompleted(lap, isFinish);

            if (isFinish)
            {
                if (Object.HasInputAuthority)
                {
                    AudioManager.Play("raceFinishedSFX", AudioManager.MixerTarget.SFX);
                    Hud.ShowEndRaceScreen();
                }

                CarEntity.CarControllerHandler.RoomUser.HasFinished = true;
                EndRaceTick = Runner.Tick;
            }
            else
            {
                if (Object.HasInputAuthority)
                    AudioManager.Play("newLapSFX", AudioManager.MixerTarget.SFX);
            }

            OnRaceCompleted?.Invoke(this);
        }

        public void ResetToCheckpoint()
        {
            var tgt = CheckpointIndex == -1
                ? GameManager.CurrentTrack.finishLine.transform
                : GameManager.CurrentTrack.checkpoints[CheckpointIndex].transform;

            nrb.Teleport(tgt.position, tgt.rotation);

            Controller.ResetController();
        }

        private static void OnLapChangedCallback(CarLapController changed)
        {
            var maxLaps = GameManager.Instance.GameType.lapCount;
            var behaviours = changed.GetComponentsInChildren<CarNetComponent>();

            var isFinish = changed.Lap - 1 == maxLaps;

            foreach (var b in behaviours)
                b.OnLapCompleted(changed.Lap, isFinish);

            changed.OnLapChanged?.Invoke(changed.Lap, maxLaps);
        }

        private static void CheckpointIndexChanged(CarLapController changed)
        {
            var nObject = changed.Object;

            if (!nObject.HasInputAuthority) return;

            if (changed.CheckpointIndex != -1)
                AudioManager.Play("errorSFX", AudioManager.MixerTarget.SFX);
        }

        public void ProcessCheckpoint(Checkpoint checkpoint)
        {
            if (CheckpointIndex == checkpoint.index - 1)
                CheckpointIndex++;
#if UNITY_EDITOR
            Debug.Log($"Process Checkpoint ID : {checkpoint.index}");
#endif
        }

        public void ProcessFinishLine(FinishLine finishLine)
        {
            Checkpoint[] checkpoints = GameManager.CurrentTrack.checkpoints;

            if (CheckpointIndex == checkpoints.Length - 1)
            {
                if (Lap == 0) return;
                LapTicks.Set(Lap - 1, Runner.Tick);
                Lap++;
                CheckpointIndex = -1;
            }

#if UNITY_EDITOR
            Debug.Log($"Process Finish Line : {finishLine.name}");
#endif
        }

        public float GetTotalRaceTime()
        {
            if (!Runner.IsRunning || StartRaceTick == 0)
                return 0f;

            var endTick = EndRaceTick == 0 ? Runner.Tick.Raw : EndRaceTick;
            return TickHelper.TickToSeconds(Runner, endTick - StartRaceTick);
        }
    }
}