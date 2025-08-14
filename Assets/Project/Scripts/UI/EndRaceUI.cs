using Assets.Project.Scripts.Car;
using Managers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EndRaceUI : MonoBehaviour, IGameUIComponent, IDisabledUI
{
    private const float DELAY = 4;

    [SerializeField]
    public PlayerResultItem resultItemPrefab;
    [SerializeField]
    public Button continueEndButton;
    [SerializeField]
    public GameObject resultsContainer;

    private CarEntity carEntity;

    public void Init(CarEntity entity)
    {
        carEntity = entity;
        continueEndButton.onClick.AddListener(() => LevelManager.LoadMenu());
    }

    public void Setup()
    {
        CarLapController.OnRaceCompleted += RedrawResultsList;
        CarEntity.OnCarSpawned += RedrawResultsList;
        CarEntity.OnCarDespawned += RedrawResultsList;
    }

    public void OnDestruction()
    {
        CarLapController.OnRaceCompleted -= RedrawResultsList;
        CarEntity.OnCarSpawned -= RedrawResultsList;
        CarEntity.OnCarDespawned -= RedrawResultsList;
    }

    public void RedrawResultsList(CarNetComponent updated)
    {
        var parent = resultsContainer.transform;
        ClearParent(parent);

        var players = GetFinishedPlayers();
        for (var i = 0; i < players.Count; i++)
        {
            var kart = players[i];

            Instantiate(resultItemPrefab, parent)
                .SetResult(kart.CarControllerHandler.RoomUser.Username.Value, kart.CarLapController.GetTotalRaceTime(), i + 1);
        }

        EnsureContinueButton(players);
    }

    private static List<CarEntity> GetFinishedPlayers() =>
        CarEntity.Cars.OrderBy(x => x.CarLapController.GetTotalRaceTime())
                      .Where(kart => kart.CarLapController.HasFinished)
                      .ToList();

    private void EnsureContinueButton(List<CarEntity> carEntities)
    {
        var allFinished = carEntities.Count == CarEntity.Cars.Count;
        if (RoomPlayer.Local.IsLeader)
            continueEndButton.gameObject.SetActive(allFinished);
    }

    private static void ClearParent(Transform parent)
    {
        var len = parent.childCount;
        for (var i = 0; i < len; i++)
            Destroy(parent.GetChild(i).gameObject);
    }
}