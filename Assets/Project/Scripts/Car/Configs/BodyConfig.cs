using UnityEngine;

namespace Assets.Project.Scripts.Car
{
    [CreateAssetMenu(fileName = "Body-config", menuName = "Racing / Car Module / New Car Body Config", order = 1)]
    public sealed class BodyConfig : ScriptableObject
    {
        //Можно создать поля с ценой и т.д в конфигурационном файле. (В ТЗ к тестовому не указана надобность в этом)
        [SerializeField]
        private Sprite icon;
        [SerializeField]
        private GameObject bodyModel;

        public Sprite Icon => icon;
        public GameObject BodyModel => bodyModel;

    }
}