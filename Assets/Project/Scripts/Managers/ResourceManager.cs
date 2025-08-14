using Assets.Project.Scripts.Car;
using FusionExamples.Utility;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public GameUI hudPrefab;
    public NicknameUI nicknameCanvasPrefab;
    public GameType[] gameTypes;
    public TrackDefinition[] tracks;
    public CarConfig[] CarConfigs;

    public static ResourceManager Instance => Singleton<ResourceManager>.Instance;

    private void Awake() =>
        DontDestroyOnLoad(gameObject);
}