using Fusion;
using UnityEngine;

namespace Assets.Project.Scripts.Car
{
    public class CarNetComponent : NetworkBehaviour
    {
        [SerializeField]
        private CarEntity carEntity;

        public CarEntity CarEntity => carEntity;

        public virtual void Initialize(CarEntity carEntity) =>
            this.carEntity = carEntity;

        public virtual void OnRaceStart() { }
        public virtual void OnLapCompleted(int lap, bool isFinish) { }
    }
}