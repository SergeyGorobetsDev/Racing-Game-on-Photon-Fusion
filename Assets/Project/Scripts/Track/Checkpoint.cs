using Assets.Project.Scripts.Car;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int index = -1;

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.parent.TryGetComponent(out CarLapController lapController))
            lapController.ProcessCheckpoint(this);
    }
}