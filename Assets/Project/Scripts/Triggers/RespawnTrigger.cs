using Assets.Project.Scripts.Car;
using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
    public GameObject effect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.TryGetComponent(out CarEntity car))
        {
            if (car.Object.HasInputAuthority)
            {
                if (effect != null)
                    Instantiate(effect, car.transform.position, car.transform.rotation);

                if (car.Object.HasStateAuthority)
                    car.CarLapController.ResetToCheckpoint();
            }
        }
    }
}