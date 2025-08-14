using Assets.Project.Scripts.Car;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NicknameUI : MonoBehaviour
{
    public WorldUINickname nicknamePrefab;

    private readonly Dictionary<CarEntity, WorldUINickname> kartNicknames = new(4);

    private void Awake()
    {
        EnsureAllTexts();

        CarEntity.OnCarSpawned += SpawnNicknameText;
        CarEntity.OnCarDespawned += DespawnNicknameText;
    }

    private void OnDestroy()
    {
        CarEntity.OnCarSpawned -= SpawnNicknameText;
        CarEntity.OnCarDespawned -= DespawnNicknameText;
    }

    private void EnsureAllTexts()
    {
        IEnumerable<CarEntity> cars = CarEntity.Cars;
        foreach (CarEntity car in cars.Where(car => !kartNicknames.ContainsKey(car)))
            SpawnNicknameText(car);
    }

    private void SpawnNicknameText(CarEntity car)
    {
        if (car.Object.IsValid && car.Object.HasInputAuthority)
            return;

        WorldUINickname uiNickname = Instantiate(nicknamePrefab, this.transform);
        uiNickname.SetCarEntity(car);
        kartNicknames.Add(car, uiNickname);
    }

    private void DespawnNicknameText(CarEntity car)
    {
        if (!kartNicknames.ContainsKey(car))
            return;

        WorldUINickname text = kartNicknames[car];
        Destroy(text.gameObject);
        kartNicknames.Remove(car);
    }
}
