using Fusion;
using Fusion.Addons.Physics;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Project.Scripts.Car
{
    public sealed class CarEntity : CarNetComponent, IAfterSpawned
    {
        //Не лучший вариант делать статичные Ивенты, нужен менеджер управляющий Созданием и Удалением машин из сцены,
        //для быстроты выполнения ТЗ сделал через статику.
        public static event Action<CarEntity> OnCarSpawned;
        public static event Action<CarEntity> OnCarDespawned;

        [Header("Config")]
        [SerializeField]
        private CarConfig carConfig;

        private bool despawned;

        [Space]
        [Header("Components")]
        [SerializeField]
        private CarMovementController carControllerHandler;
        [SerializeField]
        private NetworkRigidbody3D networkRigidbody;
        [SerializeField]
        private CarInputController carInputController;
        [SerializeField]
        private CarCameraController carCameraController;
        [SerializeField]
        private CarLapController carLapController;
        [SerializeField]
        private GameUI hud;
        [SerializeField]
        private CarPartsHolder carPartsHolder;
        [SerializeField]
        private CarAudio carAudio;
        public CarMovementController CarControllerHandler => carControllerHandler;
        public NetworkRigidbody3D NetworkRigidbody => networkRigidbody;
        public CarInputController CarInputController => carInputController;
        public CarCameraController CarCameraController => carCameraController;
        public CarLapController CarLapController => carLapController;
        public CarPartsHolder CarPartsHolder => carPartsHolder;
        public CarAudio CarAudio => carAudio;
        public GameUI Hud => hud;

        public static readonly List<CarEntity> Cars = new List<CarEntity>();

        private void Awake()
        {
            SetComponents();
        }

        public override void Spawned()
        {
            base.Spawned();

            if (Object.HasInputAuthority)
            {
                hud = Instantiate(ResourceManager.Instance.hudPrefab);
                Hud.Init(this);

                Instantiate(ResourceManager.Instance.nicknameCanvasPrefab);
            }

            Cars.Add(this);
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);
            Cars.Remove(this);
            despawned = true;
            OnCarDespawned?.Invoke(this);
        }

        public override void OnRaceStart()
        {
            base.OnRaceStart();
            carControllerHandler.OnRaceStart();
            carLapController.OnRaceStart();
            carAudio.OnRaceStart();
        }

        private void OnDestroy()
        {
            Cars.Remove(this);
            if (!despawned)
                OnCarDespawned?.Invoke(this);
        }

        private void SetComponents()
        {
            if (TryGetComponent(out CarMovementController carMovementController))
            {
                this.carControllerHandler = carMovementController;
                this.carControllerHandler.Initialize(this);
            }

            if (TryGetComponent(out CarInputController carInputController))
            {
                this.carInputController = carInputController;
                this.carInputController.Initialize(this);
            }

            if (TryGetComponent(out CarCameraController carCameraController))
            {
                this.carCameraController = carCameraController;
                this.carCameraController.Initialize(this);
            }

            if (TryGetComponent(out CarLapController carLapController))
            {
                this.carLapController = carLapController;
                this.carLapController.Initialize(this);
            }

            if (TryGetComponent(out CarPartsHolder сarPartsHolder))
            {
                this.carPartsHolder = сarPartsHolder;
                this.carPartsHolder.Initialize(this);
            }

            if (TryGetComponent(out CarAudio carAudio))
            {
                this.carAudio = carAudio;
                this.carAudio.Initialize(this);
            }

            if (TryGetComponent(out NetworkRigidbody3D networkRigidbody))
                this.networkRigidbody = networkRigidbody;
        }

        public void AfterSpawned()
        {
            carPartsHolder.ChangeVisibility();
        }
    }
}