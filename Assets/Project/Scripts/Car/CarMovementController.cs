using Fusion;
using UnityEngine;

namespace Assets.Project.Scripts.Car
{
    public sealed class CarMovementController : CarNetComponent
    {
        [Header("Components")]
        [SerializeField]
        private WheelGroundConnector[] wheelGroundPlacer;
        [SerializeField]
        public Rigidbody rigBody;

        public float Horizontal;
        public float Vertical;

        [Header("Car Settings")]
        [SerializeField]
        public float wheelRotateSpeed;
        [SerializeField]
        public float wheelSteeringAngle;
        [SerializeField]
        public float wheelAcceleration;
        [SerializeField]
        public float wheelMaxSpeed;
        [SerializeField]
        public float BreakPower;

        public float CurrentSpeed = 0;

        [Networked]
        public CarInputController.NetworkInputData Inputs { get; set; }
        [Networked]
        public RoomPlayer RoomUser { get; set; }

        [Networked]
        public bool CanControll { get; set; } = false;

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();
            MoveWheels();
        }

        private void MoveWheels()
        {
            if (!CanControll) return;

            if (GetInput(out CarInputController.NetworkInputData input))
            {
                Inputs = input;
                Horizontal = input.Steer;
                Vertical = input.Acceleration;
            }

            for (int i = 0; i < wheelGroundPlacer.Length; i++)
            {
                wheelGroundPlacer[i].SteeringAngle = Mathf.LerpAngle(wheelGroundPlacer[i].SteeringAngle, 0, Runner.DeltaTime * wheelRotateSpeed);
                wheelGroundPlacer[i].WheelCollider.motorTorque = -Mathf.Lerp(wheelGroundPlacer[i].WheelCollider.motorTorque, 0, Runner.DeltaTime * wheelAcceleration);


                if (Vertical > 0.1)
                    wheelGroundPlacer[i].WheelCollider.motorTorque = Mathf.Lerp(wheelGroundPlacer[i].WheelCollider.motorTorque, wheelMaxSpeed, Runner.DeltaTime * wheelAcceleration);

                if (Vertical < -0.1)
                {
                    wheelGroundPlacer[i].WheelCollider.motorTorque = -Mathf.Lerp(wheelGroundPlacer[i].WheelCollider.motorTorque, wheelMaxSpeed, Runner.DeltaTime * wheelAcceleration * BreakPower);
                    rigBody.linearDamping = 0.3f;
                }
                else rigBody.linearDamping = 0;

                if (Horizontal > 0.1)
                    wheelGroundPlacer[i].SteeringAngle = Mathf.LerpAngle(wheelGroundPlacer[i].SteeringAngle, wheelSteeringAngle, Runner.DeltaTime * wheelRotateSpeed);

                if (Horizontal < -0.1)
                    wheelGroundPlacer[i].SteeringAngle = Mathf.LerpAngle(wheelGroundPlacer[i].SteeringAngle, -wheelSteeringAngle, Runner.DeltaTime * wheelRotateSpeed);
            }

            Vector3 vel = rigBody.linearVelocity;
            CurrentSpeed = rigBody.linearVelocity.magnitude * 2.23693629f;
        }

        public void ResetController()
        {
            rigBody.linearVelocity = Vector3.zero;
            rigBody.linearDamping = 0;
            Vertical = 0f;
            Horizontal = 0f;

            for (int i = 0; i < wheelGroundPlacer.Length; i++)
            {
                wheelGroundPlacer[i].SteeringAngle = 0f;
                wheelGroundPlacer[i].WheelCollider.motorTorque = 0f;
            }
        }

        public override void OnRaceStart()
        {
            base.OnRaceStart();
            if (Object.HasInputAuthority)
            {
                AudioManager.PlayMusic(Track.Current.music);
                CanControll = true;
                for (int i = 0; i < CarEntity.Cars.Count; i++)
                {
                    CarEntity.Cars[i].CarControllerHandler.CanControll = true;
                    CarEntity.Cars[i].CarAudio.СanPlayAudio = true;
                }
            }
        }
    }
}
