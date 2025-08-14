using System;
using UnityEngine;

namespace Assets.Project.Scripts.Car
{
    public enum WheelType : byte
    {
        None = 0,
        Left = 1,
        Right = 2,
    }

    [CreateAssetMenu(fileName = " Wheel-config", menuName = "Racing / Car Module / New Car Wheel Config", order = 1)]
    public sealed class WheelConfig : ScriptableObject
    {
        //Можно создать поля с ценой и т.д в конфигурационном файле. (В ТЗ к тестовому не указана надобность в этом)
        [SerializeField]
        private Sprite icon;

        [SerializeField]
        private WheelData[] wheelsModels;

        public Sprite Icon => icon;
        public WheelData[] WheelsModels => wheelsModels;
    }

    [Serializable]
    public sealed class WheelData
    {
        [field: SerializeField]
        public WheelType WheelType { get; set; }
        [field: SerializeField]
        public GameObject WheelModel { get; set; }
    }
}
