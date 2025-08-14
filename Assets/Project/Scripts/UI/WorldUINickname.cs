using Assets.Project.Scripts.Car;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class WorldUINickname : MonoBehaviour
{
    [SerializeField]
    public Text worldNicknameText;
    [SerializeField]
    public Vector3 offset;

    [HideInInspector]
    public Transform target;

    private CarEntity car;

    public void SetCarEntity(CarEntity car)
    {
        this.car = car;
        target = car.NetworkRigidbody.InterpolationTarget;
    }

    private void Update()
    {
        if (car == null) return;

        RoomPlayer lobbyUser = car.CarControllerHandler.RoomUser;
        if (lobbyUser != null)
            worldNicknameText.text = lobbyUser.Username.Value;
    }

    private void LateUpdate()
    {
        if (target)
        {
            transform.position = target.position + offset;
            transform.rotation = Camera.main.transform.rotation;
        }
        else StartCoroutine(WaitAndDestroy());
    }

    private IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(3);
        if (target != null && !target.Equals(null))
            yield return null;
        else Destroy(gameObject);
    }
}
