using UnityEngine;

namespace Assets.Project.Scripts.Car
{
    [CreateAssetMenu(fileName = "Spoiler-config", menuName = "Racing / Car Module / New Car Spoiler Config", order = 1)]
    public sealed class SpoilerConfig : ScriptableObject
    {
        //Можно создать поля с ценой и т.д в конфигурационном файле. (В ТЗ к тестовому не указана надобность в этом)
        [SerializeField]
        private Sprite icon;

        [SerializeField]
        private GameObject spoilerModel;

        public Sprite Icon => icon;
        public GameObject SpoilerModel => spoilerModel;

    }
}
