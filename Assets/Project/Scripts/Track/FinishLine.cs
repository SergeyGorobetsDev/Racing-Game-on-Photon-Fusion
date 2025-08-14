using Assets.Project.Scripts.Car;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    public bool debug;

    private void OnTriggerStay(Collider other)
    {
        Debug.Log($"Finish Line Collided with {other.gameObject.name}");

        if (other.transform.parent.TryGetComponent(out CarLapController lapController))
            lapController.ProcessFinishLine(this);
    }
}