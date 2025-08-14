using UnityEngine;

namespace Assets.Project.Scripts.Car
{
    public sealed class CarCameraController : CarNetComponent, ICameraController
    {
        [Header("Components")]
        [SerializeField]
        public Transform rig;
        [SerializeField]
        public Transform camNode;
        [SerializeField]
        public Transform forwardVP;
        [SerializeField]
        public Transform backwardVP;
        [SerializeField]
        public Transform finishVP;

        [Space]
        [Header("Settings")]
        [SerializeField]
        public float lerpFactorVP = 3f;
        [SerializeField]
        public float lerpFactorFOV = 1.5f;
        [SerializeField]
        public float normalFOV = 60;
        [SerializeField]
        public float finishFOV = 45;
        [SerializeField]
        public bool useFinishVP;

        private float currentFOV = 60;
        private Transform viewpoint;
        private bool shouldLerpCamera = true;
        private bool lastFrameLookBehind;

        public override void OnLapCompleted(int lap, bool isFinish)
        {
            base.OnLapCompleted(lap, isFinish);
            if (isFinish)
                useFinishVP = true;
        }

        public override void Render()
        {
            base.Render();

            if (Object.HasInputAuthority && shouldLerpCamera && !GameManager.IsCameraControlled)
            {
                rig.rotation = transform.rotation;
                GameManager.GetCameraControl(this);
            }
        }

        public bool ControlCamera(Camera cam)
        {
            if (this.Equals(null))
                return false;

            viewpoint = GetViewpoint();

            if (shouldLerpCamera)
                ControlCameraLerp(cam);
            else
                ControlCameraDriving(cam);

            return true;
        }

        private void ControlCameraDriving(Camera cam)
        {
            bool lookBehindThisFrame = lastFrameLookBehind != CarEntity.CarInputController.IsLookBehindPressed;
            rig.localEulerAngles = viewpoint.localEulerAngles;
            lastFrameLookBehind = CarEntity.CarInputController.IsLookBehindPressed;

            camNode.localPosition = Vector3.Lerp(camNode.localPosition,
                                                 viewpoint.localPosition,
                                                 Time.deltaTime * lerpFactorVP);

            cam.transform.SetPositionAndRotation(camNode.position, Quaternion.LookRotation(camNode.forward, Vector3.up));
            SetFOV(cam);
        }

        private void ControlCameraLerp(Camera cam)
        {
            cam.transform.SetPositionAndRotation(Vector3.Lerp(cam.transform.position, camNode.position, Time.deltaTime * 2f),
                                                 Quaternion.Slerp(cam.transform.rotation, camNode.rotation, Time.deltaTime * 2f));

            if (Vector3.Distance(cam.transform.position, camNode.position) < 0.05f &&
                Vector3.Dot(cam.transform.forward, camNode.forward) > 0.95f)
                shouldLerpCamera = false;
        }

        private void SetFOV(Camera cam)
        {
            currentFOV = useFinishVP ? finishFOV : normalFOV;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, currentFOV, Time.deltaTime * lerpFactorFOV);
        }

        private Transform GetViewpoint()
        {
            if (CarEntity.CarControllerHandler == null) return null;
            if (useFinishVP) return finishVP;
            return forwardVP;
        }
    }
}