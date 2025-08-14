using Assets.Project.Scripts.Car;
using UnityEngine;

public sealed class CarCustomizationUI : MonoBehaviour
{
    [SerializeField]
    private SlotUI[] carSlots;
    [SerializeField]
    private SlotUI[] carBodySlots;
    [SerializeField]
    private SlotUI[] carWheelsSlots;
    [SerializeField]
    private SlotUI[] carSpoilersSlots;

    private int selectedCarId = -1;
    [SerializeField]
    private CarConfig[] carConfigs;

    private void Start()
    {
        carConfigs = ResourceManager.Instance.CarConfigs;

        for (int i = 0; i < carSlots.Length; i++)
            carSlots[i].SetData(carConfigs[i].Icon);

        SelectCar(SaveManager.Instance.GetUserData().ActiveCarId < 0 ? 0 : SaveManager.Instance.GetUserData().ActiveCarId);
    }

    private void OnEnable()
    {
        for (int i = 0; i < carSlots.Length; i++)
            carSlots[i].SlotSelected += SelectCar;

        for (int i = 0; i < carBodySlots.Length; i++)
            carBodySlots[i].SlotSelected += SelectBody;

        for (int i = 0; i < carWheelsSlots.Length; i++)
            carWheelsSlots[i].SlotSelected += SelectWheels;

        for (int i = 0; i < carSpoilersSlots.Length; i++)
            carSpoilersSlots[i].SlotSelected += SelectSpoiler;
    }


    private void OnDisable()
    {
        for (int i = 0; i < carSlots.Length; i++)
            carSlots[i].SlotSelected -= SelectCar;

        for (int i = 0; i < carBodySlots.Length; i++)
            carBodySlots[i].SlotSelected -= SelectBody;

        for (int i = 0; i < carWheelsSlots.Length; i++)
            carWheelsSlots[i].SlotSelected -= SelectWheels;

        for (int i = 0; i < carSpoilersSlots.Length; i++)
            carSpoilersSlots[i].SlotSelected -= SelectSpoiler;
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
            UpdateSlotsData();
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
        if (id < 0) return;

        if (SpotlightGroup.Search("Car-spoilers", out SpotlightGroup spotlight))
        {
            spotlight.FocusIndex(id);
            SaveManager.Instance.GetUserData().CarsModificationData[selectedCarId].SpoilerId = id;
        }
    }

    private void UpdateSlotsData()
    {
        CarConfig carConfig = ResourceManager.Instance.CarConfigs[selectedCarId];

        for (int i = 0; i < carBodySlots.Length; i++)
        {
            carBodySlots[i].ChangeVisibleState(carConfig.BodyConfigs[i] != null);
            if (carConfig.BodyConfigs[i] != null)
                carBodySlots[i].SetData(carConfig.BodyConfigs[i].Icon);
        }

        for (int i = 0; i < carWheelsSlots.Length; i++)
        {
            carWheelsSlots[i].ChangeVisibleState(carConfig.WheelConfig[i] != null);
            if (carConfig.WheelConfig[i] != null)
                carWheelsSlots[i].SetData(carConfig.WheelConfig[i].Icon);

        }

        for (int i = 0; i < carSpoilersSlots.Length; i++)
        {
            carSpoilersSlots[i].ChangeVisibleState(i <= carConfig.SpoilerConfig.Length - 1);
            if (i <= carConfig.SpoilerConfig.Length - 1)
                carSpoilersSlots[i].SetData(carConfig.SpoilerConfig[i].Icon);
        }
    }
}