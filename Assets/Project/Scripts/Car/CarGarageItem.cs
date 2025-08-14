using UnityEngine;

namespace Assets.Project.Scripts.Car
{
    public class CarGarageItem : MonoBehaviour
    {
        [Header("Car Body")]
        [SerializeField]
        private SpotlightGroup bodyGroup;

        [Header("Car Wheels")]
        [SerializeField]
        private SpotlightGroup wheelsFrontLeftGroup;
        [SerializeField]
        private SpotlightGroup wheelsFrontRightGroup;
        [SerializeField]
        private SpotlightGroup wheelsBackLeftGroup;
        [SerializeField]
        private SpotlightGroup wheelsBackRightGroup;

        [Header("Car Spoilers")]
        [SerializeField]
        private SpotlightGroup spoilersGroup;

        public SpotlightGroup BodyGroup => bodyGroup;
        public SpotlightGroup WheelsFrontLeftGroup => wheelsFrontLeftGroup;
        public SpotlightGroup WheelsFrontRightGroup => wheelsFrontRightGroup;
        public SpotlightGroup WheelsBackLeftGroup => wheelsBackLeftGroup;
        public SpotlightGroup WheelsBackRightGroup => wheelsBackRightGroup;
        public SpotlightGroup SpoilersGroup => spoilersGroup;
    }
}