using Assets.Project.Scripts.Car;
using Fusion;
using UnityEngine;

public class Track : NetworkBehaviour, ICameraController
{
    public static Track Current { get; private set; }

    [Networked] public TickTimer StartRaceTimer { get; set; }

    public CameraTrack[] introTracks;
    public Checkpoint[] checkpoints;
    public Transform[] spawnpoints;
    public FinishLine finishLine;

    public TrackDefinition definition;
    public TrackStartSequence sequence;

    public string music = "";
    public float introSpeed = 0.5f;

    private int currentIntroTrack;
    private float introIntervalProgress;

    [SerializeField]
    private Camera sceneCamera;

    public override void Spawned()
    {
        base.Spawned();
        Current = this;
        InitCheckpoints();
        AudioManager.StopMusic();
        GameManager.SetTrack(this);
        GameManager.Instance.camera = sceneCamera;
        StartIntro();

        if (RoomPlayer.Local.IsLeader)
            StartRaceTimer = TickTimer.CreateFromSeconds(Runner, sequence.duration + 4f);
        sequence.StartSequence();
    }

    private void OnDestroy() =>
        GameManager.SetTrack(null);

    public void SpawnPlayer(NetworkRunner runner, RoomPlayer player)
    {
        int index = RoomPlayer.Players.IndexOf(player);
        Transform point = spawnpoints[index];
        int prefabId = player.CarId;
        CarEntity carPrefab = ResourceManager.Instance.CarConfigs[prefabId].CarEntity;
        CarEntity spawned = runner.Spawn(
            carPrefab,
            point.position,
            point.rotation,
            player.Object.InputAuthority
        );
        spawned.transform.name = $"Car ({player.Username})";
        player.GameState = RoomPlayer.EGameState.GameCutscene;
        player.Car = spawned.CarControllerHandler;
        spawned.CarControllerHandler.RoomUser = player;
    }

    private void InitCheckpoints()
    {
        for (int i = 0; i < checkpoints.Length; i++)
            checkpoints[i].index = i;
    }

    public bool ControlCamera(Camera cam)
    {
        Debug.Log($"Control Camera = {cam is null}");

        cam.transform.SetPositionAndRotation(Vector3.Lerp(
            introTracks[currentIntroTrack].startPoint.position,
            introTracks[currentIntroTrack].endPoint.position,
            introIntervalProgress), Quaternion.Slerp(
            introTracks[currentIntroTrack].startPoint.rotation,
            introTracks[currentIntroTrack].endPoint.rotation,
            introIntervalProgress));
        introIntervalProgress += Time.deltaTime * introSpeed;

        if (introIntervalProgress > 1)
        {
            introIntervalProgress -= 1;
            currentIntroTrack++;
            if (currentIntroTrack == introTracks.Length)
            {
                currentIntroTrack = 0;
                introIntervalProgress = 0;
                return false;
            }
        }

        return true;
    }

    public void StartIntro()
    {
        currentIntroTrack = 0;
        introIntervalProgress = 0;
        AudioManager.PlayMusic("track01");
        GameManager.GetCameraControl(this);
    }
}