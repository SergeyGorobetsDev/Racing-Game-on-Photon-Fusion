using UnityEngine;

namespace Assets.Project.Scripts.Car
{
    [CreateAssetMenu(fileName = "Car-config", menuName = "Racing / Car Module / New Car Config", order = 1)]
    public sealed class CarConfig : ScriptableObject
    {
        //Можно создать поля с ценой и т.д в конфигурационном файле. (В ТЗ к тестовому не указана надобность в этом)

        [SerializeField]
        private string name;
        [SerializeField]
        private string id;
        [SerializeField]
        private Sprite icon;

        [Header("Stats")]
        [SerializeField]
        private float wheelRotateSpeed;
        [SerializeField]
        private float wheelSteeringAngle;
        [SerializeField]
        private float wheelAcceleration;
        [SerializeField]
        private float wheelMaxSpeed;
        [SerializeField]
        private float breakPower;
        [Header("Prefab")]
        [SerializeField]
        private CarEntity carEntity;

        [Space()]
        [Header("Customization")]
        [Header("Car Body")]
        [SerializeField]
        private BodyConfig[] bodyConfigs;
        [Header("Car Wheels")]
        [SerializeField]
        private WheelConfig[] wheelConfig;
        [Header("Car Spoilers")]
        [SerializeField]
        private SpoilerConfig[] spoilerConfig;

        public string Name => name;
        public string Id => id;
        public Sprite Icon => icon;
        public float WheelRotateSpeed => wheelRotateSpeed;
        public float WheelSteeringAngle => wheelRotateSpeed;
        public float WheelAcceleration => wheelRotateSpeed;
        public float WheelMaxSpeed => wheelRotateSpeed;
        public float BreakPower => wheelRotateSpeed;
        public CarEntity CarEntity => carEntity;
        public BodyConfig[] BodyConfigs => bodyConfigs;
        public WheelConfig[] WheelConfig => wheelConfig;
        public SpoilerConfig[] SpoilerConfig => spoilerConfig;
    }
}