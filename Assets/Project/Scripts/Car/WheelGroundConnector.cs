using Fusion;
using UnityEngine;

namespace Assets.Project.Scripts.Car
{
    public sealed class WheelGroundConnector : NetworkBehaviour
    {
        [Header("Components")]
        [SerializeField]
        private GameObject wheelBase;
        [SerializeField]
        private GameObject wheelVisual;
        [SerializeField]
        private WheelCollider wheelCol;

        private RaycastHit hit;

        [Networked]
        public bool Steerable { get; set; }
        [Networked]
        public float SteeringAngle { get; set; }

        public WheelCollider WheelCollider => wheelCol;

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();
            AlignToCollider();
        }

        private void AlignToCollider()
        {
            wheelVisual.transform.position = Physics.Raycast(wheelCol.transform.position, -wheelCol.transform.up, out hit, wheelCol.suspensionDistance + wheelCol.radius)
                                                            ? hit.point + wheelCol.transform.up * wheelCol.radius
                                                            : wheelCol.transform.position - (wheelCol.transform.up * wheelCol.suspensionDistance);

            if (Steerable)
                wheelCol.steerAngle = SteeringAngle;

            wheelVisual.transform.eulerAngles = new Vector3(wheelBase.transform.eulerAngles.x, wheelBase.transform.eulerAngles.y + wheelCol.steerAngle, wheelBase.transform.eulerAngles.z);
            wheelVisual.transform.Rotate(wheelCol.rpm / 60 * 360 * Time.deltaTime, 0, 0);
        }
    }
}
