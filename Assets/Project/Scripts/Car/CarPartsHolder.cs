using Fusion;
using UnityEngine;

namespace Assets.Project.Scripts.Car
{
    public sealed class CarPartsHolder : CarNetComponent
    {
        [SerializeField]
        private Transform[] bodyParts;
        [SerializeField]
        private Transform[] wheelsLeftFrontParts;
        [SerializeField]
        private Transform[] wheelsRightFrontParts;
        [SerializeField]
        private Transform[] wheelsLeftBackParts;
        [SerializeField]
        private Transform[] wheelsRightBackParts;
        [SerializeField]
        private Transform[] spoilerParts;

        [Networked]
        public int CarBodyId { get; set; }
        [Networked]
        public int CarWheelID { get; set; }
        [Networked]
        public int CarSpolerID { get; set; }

        public void SetData(int bodyId, int wheelId, int spoilerId)
        {
            if (Object.HasStateAuthority)
            {
                CarBodyId = bodyId;
                CarWheelID = wheelId;
                CarSpolerID = spoilerId;
                ChangeVisibility();
            }
        }

        public void ChangeVisibility()
        {
            for (int i = 0; i < bodyParts.Length; i++)
                bodyParts[i].gameObject.SetActive(i == CarBodyId);

            for (int i = 0; i < wheelsLeftFrontParts.Length; i++)
                wheelsLeftFrontParts[i].gameObject.SetActive(i == CarWheelID);
            for (int i = 0; i < wheelsRightFrontParts.Length; i++)
                wheelsRightFrontParts[i].gameObject.SetActive(i == CarWheelID);
            for (int i = 0; i < wheelsLeftBackParts.Length; i++)
                wheelsLeftBackParts[i].gameObject.SetActive(i == CarWheelID);
            for (int i = 0; i < wheelsRightBackParts.Length; i++)
                wheelsRightBackParts[i].gameObject.SetActive(i == CarWheelID);

            for (int i = 0; i < spoilerParts.Length; i++)
                spoilerParts[i].gameObject.SetActive(i == CarSpolerID);
        }
    }
}