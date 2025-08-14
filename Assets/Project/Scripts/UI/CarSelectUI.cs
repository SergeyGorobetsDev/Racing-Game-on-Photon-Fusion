using UnityEngine;
using UnityEngine.UI;

public class CarSelectUI : MonoBehaviour
{
    [SerializeField]
    private SlotUI[] carSlots;
    [SerializeField]
    public Image speedStatBar;
    [SerializeField]
    public Image accelStatBar;
    [SerializeField]
    public Image turnStatBar;

    private int selectedCarId = -1;

    private void Awake()
    {
        SelectCar(SaveManager.Instance.GetUserData().ActiveCarId < 0 ? 0 : SaveManager.Instance.GetUserData().ActiveCarId);
        var carConfigs = ResourceManager.Instance.CarConfigs;

        for (int i = 0; i < carSlots.Length; i++)
            carSlots[i].SetData(carConfigs[i].Icon);
    }

    private void OnEnable()
    {
        for (int i = 0; i < carSlots.Length; i++)
            carSlots[i].SlotSelected += SelectCar;
    }

    private void OnDisable()
    {
        for (int i = 0; i < carSlots.Length; i++)
            carSlots[i].SlotSelected -= SelectCar;
    }

    public void SelectCar(int id)
    {
        selectedCarId = id;
        if (SpotlightGroup.Search("Cars Display", out SpotlightGroup spotlight))
        {
            spotlight.FocusIndex(id);
            SaveManager.Instance.GetUserData().CarsModificationData[id].CarId = id;
            SelectBody(SaveManager.Instance.GetUserData().CarsModificationData[id].BodyId < 0 ? 0 : SaveManager.Instance.GetUserData().CarsModificationData[id].BodyId);
            SelectWheels(SaveManager.Instance.GetUserData().CarsModificationData[id].WheelId < 0 ? 0 : SaveManager.Instance.GetUserData().CarsModificationData[id].WheelId);
            SelectSpoiler(SaveManager.Instance.GetUserData().CarsModificationData[id].SpoilerId < 0 ? -1 : SaveManager.Instance.GetUserData().CarsModificationData[id].SpoilerId);
            SaveManager.Instance.GetUserData().ActiveCarId = id;
        }

        if (RoomPlayer.Local != null)
        {
            RoomPlayer.Local.RPC_SetCarId(id);
            RoomPlayer.Local.RPC_SetCarBodyId(SaveManager.Instance.GetUserData().CarsModificationData[id].BodyId < 0 ? 0 : SaveManager.Instance.GetUserData().CarsModificationData[id].BodyId);
            RoomPlayer.Local.RPC_SetCarWheelId(SaveManager.Instance.GetUserData().CarsModificationData[id].WheelId < 0 ? 0 : SaveManager.Instance.GetUserData().CarsModificationData[id].WheelId);
            RoomPlayer.Local.RPC_SetCarSpoilerId(SaveManager.Instance.GetUserData().CarsModificationData[id].SpoilerId < 0 ? -1 : SaveManager.Instance.GetUserData().CarsModificationData[id].SpoilerId);
        }
    }

    public void SelectBody(int id)
    {
        if (SpotlightGroup.Search("Car-body", out SpotlightGroup spotlight))
        {
            spotlight.FocusIndex(id);
            SaveManager.Instance.GetUserData().CarsModificationData[selectedCarId].BodyId = id;
        }
    }

    public void SelectWheels(int id)
    {
        if (SpotlightGroup.Search("Car-wheel-left-front", out SpotlightGroup spotlight1))
            spotlight1.FocusIndex(id);

        if (SpotlightGroup.Search("Car-wheel-right-front", out SpotlightGroup spotlight2))
            spotlight2.FocusIndex(id);

        if (SpotlightGroup.Search("Car-wheel-left-back", out SpotlightGroup spotlight3))
            spotlight3.FocusIndex(id);

        if (SpotlightGroup.Search("Car-wheel-right-back", out SpotlightGroup spotlight4))
            spotlight4.FocusIndex(id);

        SaveManager.Instance.GetUserData().CarsModificationData[selectedCarId].WheelId = id;
    }

    public void SelectSpoiler(int id)
    {
        if (SpotlightGroup.Search("Car-spoilers", out SpotlightGroup spotlight))
        {
            spotlight.FocusIndex(id);
            SaveManager.Instance.GetUserData().CarsModificationData[selectedCarId].SpoilerId = id;
        }
    }
}