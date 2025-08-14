using Fusion;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Project.Scripts.Car
{
    public class CarAudio : CarNetComponent
    {
        [SerializeField]
        private CarMovementController controller;

        [SerializeField]
        private AudioSource Engine;
        [SerializeField]
        private AudioSource EngineIdle;
        [SerializeField]
        private AudioSource GearChangeSound;

        [SerializeField]
        private float[] PitchOffsets = new float[6];
        [SerializeField]
        private float[] gearChangeSpeeds = { 30, 60, 90, 120, 150, 180 };
        [SerializeField]
        private float pitchStep = 30f;

        [Networked]
        public bool СanPlayAudio { get; set; } = false;

        private HashSet<int> playedGears = new();

        public override void Initialize(CarEntity carEntity)
        {
            base.Initialize(carEntity);
            controller = carEntity.CarControllerHandler;
        }

        public override void OnRaceStart()
        {
            base.OnRaceStart();
            if (Object.HasInputAuthority)
            {
                СanPlayAudio = true;
                Engine.Play();
                Engine.loop = true;
                EngineIdle.Play();
                EngineIdle.loop = true;
                GearChangeSound.Play();
                GearChangeSound.loop = true;
            }
        }

        private void Update()
        {
            if (!СanPlayAudio) return;
            EngineVolume();
            PitchControl();
            GearChange();
        }

        private void EngineVolume()
        {
            float verticalInput = Input.GetAxis("Vertical");
            if (verticalInput > 0.9f)
                Engine.volume = Mathf.Clamp01(Engine.volume + Time.deltaTime);
            else if (Engine.volume > 0.1f)
                Engine.volume = Mathf.Clamp01(Engine.volume - Time.deltaTime);
        }

        private void GearChange()
        {
            foreach (float targetSpeed in gearChangeSpeeds)
            {
                if (Mathf.Abs(controller.CurrentSpeed - targetSpeed) < 1f && !playedGears.Contains((int)targetSpeed))
                {
                    GearChangeSound.Play();
                    playedGears.Add((int)targetSpeed);
                }
            }
        }

        private void PitchControl()
        {
            int index = Mathf.FloorToInt(controller.CurrentSpeed / pitchStep);
            index = Mathf.Clamp(index, 0, PitchOffsets.Length - 1);
            Engine.pitch = controller.CurrentSpeed * PitchOffsets[index];
        }
    }
}