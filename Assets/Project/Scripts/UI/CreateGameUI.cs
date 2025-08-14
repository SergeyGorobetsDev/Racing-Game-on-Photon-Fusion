using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CreateGameUI : MonoBehaviour
{
    [SerializeField]
    public InputField lobbyName;
    [SerializeField]
    public Dropdown track;
    [SerializeField]
    public Dropdown gameMode;
    [SerializeField]
    public Slider playerCountSlider;
    [SerializeField]
    public Image trackImage;
    [SerializeField]
    public Text playerCountSliderText;
    [SerializeField]
    public Button confirmButton;

    private bool lobbyIsValid;

    private void Start()
    {
        playerCountSlider.SetValueWithoutNotify(8);
        SetPlayerCount();

        track.ClearOptions();
        track.AddOptions(ResourceManager.Instance.tracks.Select(x => x.TrackName).ToList());
        track.onValueChanged.AddListener(SetTrack);
        SetTrack(0);

        gameMode.ClearOptions();
        gameMode.AddOptions(ResourceManager.Instance.gameTypes.Select(x => x.modeName).ToList());
        gameMode.onValueChanged.AddListener(SetGameType);
        SetGameType(0);

        playerCountSlider.wholeNumbers = true;
        playerCountSlider.minValue = 1;
        playerCountSlider.maxValue = 8;
        playerCountSlider.value = 2;
        playerCountSlider.onValueChanged.AddListener(x => ServerInfo.MaxUsers = (int)x);

        lobbyName.onValueChanged.AddListener(x =>
        {
            ServerInfo.LobbyName = x;
            confirmButton.interactable = !string.IsNullOrEmpty(x);
        });
        lobbyName.text = ServerInfo.LobbyName = "Session" + Random.Range(0, 1000);

        ServerInfo.TrackId = track.value;
        ServerInfo.GameMode = gameMode.value;
        ServerInfo.MaxUsers = (int)playerCountSlider.value;
    }

    public void SetGameType(int gameType) =>
        ServerInfo.GameMode = gameType;

    public void SetTrack(int trackId)
    {
        ServerInfo.TrackId = trackId;
        trackImage.sprite = ResourceManager.Instance.tracks[trackId].TrackIcon;
    }

    public void SetPlayerCount()
    {
        playerCountSlider.value = ServerInfo.MaxUsers;
        playerCountSliderText.text = $"{ServerInfo.MaxUsers}";
    }

    public void ValidateLobby() =>
        lobbyIsValid = string.IsNullOrEmpty(ServerInfo.LobbyName) == false;

    public void TryFocusScreen(UIScreen screen)
    {
        if (lobbyIsValid)
            UIScreen.Focus(screen);
    }

    public void TryCreateLobby(GameLauncher launcher)
    {
        if (lobbyIsValid)
        {
            launcher.JoinOrCreateLobby();
            lobbyIsValid = false;
        }
    }
}