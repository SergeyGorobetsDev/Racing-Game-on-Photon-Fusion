using Managers;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour, IDisabledUI
{
    [SerializeField]
    public LobbyItemUI lobbyItemUI;
    [SerializeField]
    public Transform parent;
    [SerializeField]
    public Button readyUp;
    [SerializeField]
    public Button customizeButton;
    [SerializeField]
    public Text trackNameText;
    [SerializeField]
    public Text modeNameText;
    [SerializeField]
    public TextMeshProUGUI lobbyNameText;
    [SerializeField]
    public Dropdown trackNameDropdown;
    [SerializeField]
    public Dropdown gameTypeDropdown;
    [SerializeField]
    public Image trackIconImage;

    private static readonly Dictionary<RoomPlayer, LobbyItemUI> ListItems = new Dictionary<RoomPlayer, LobbyItemUI>();
    private static bool IsSubscribed;

    private void Awake()
    {
        trackNameDropdown.onValueChanged.AddListener(x =>
        {
            var gm = GameManager.Instance;
            if (gm != null) gm.TrackId = x;
        });
        gameTypeDropdown.onValueChanged.AddListener(x =>
        {
            var gm = GameManager.Instance;
            if (gm != null) gm.GameTypeId = x;
        });

        GameManager.OnLobbyDetailsUpdated += UpdateDetails;

        RoomPlayer.PlayerChanged += (player) =>
        {
            var isLeader = RoomPlayer.Local.IsLeader;
            trackNameDropdown.interactable = isLeader;
            gameTypeDropdown.interactable = isLeader;
            customizeButton.interactable = !RoomPlayer.Local.IsReady;
        };
    }

    void UpdateDetails(GameManager manager)
    {
        lobbyNameText.text = "Room Code: " + manager.LobbyName;
        trackNameText.text = manager.TrackName;
        modeNameText.text = manager.ModeName;

        var tracks = ResourceManager.Instance.tracks;
        var trackOptions = tracks.Select(x => x.TrackName).ToList();

        var gameTypes = ResourceManager.Instance.gameTypes;
        var gameTypeOptions = gameTypes.Select(x => x.modeName).ToList();

        trackNameDropdown.ClearOptions();
        trackNameDropdown.AddOptions(trackOptions);
        trackNameDropdown.value = GameManager.Instance.TrackId;

        trackIconImage.sprite = ResourceManager.Instance.tracks[GameManager.Instance.TrackId].TrackIcon;

        gameTypeDropdown.ClearOptions();
        gameTypeDropdown.AddOptions(gameTypeOptions);
        gameTypeDropdown.value = GameManager.Instance.GameTypeId;
    }

    public void Setup()
    {
        if (IsSubscribed) return;

        RoomPlayer.PlayerJoined += AddPlayer;
        RoomPlayer.PlayerLeft += RemovePlayer;
        RoomPlayer.PlayerChanged += EnsureAllPlayersReady;
        readyUp.onClick.AddListener(ReadyUpListener);
        IsSubscribed = true;
    }

    private void OnDestroy()
    {
        if (!IsSubscribed) return;

        RoomPlayer.PlayerJoined -= AddPlayer;
        RoomPlayer.PlayerLeft -= RemovePlayer;
        RoomPlayer.PlayerChanged -= EnsureAllPlayersReady;
        readyUp.onClick.RemoveListener(ReadyUpListener);
        IsSubscribed = false;
    }

    private void AddPlayer(RoomPlayer player)
    {
        if (ListItems.ContainsKey(player))
        {
            var toRemove = ListItems[player];
            Destroy(toRemove.gameObject);

            ListItems.Remove(player);
        }

        LobbyItemUI obj = Instantiate(lobbyItemUI, parent);
        obj.SetPlayer(player);

        ListItems.Add(player, obj);

        UpdateDetails(GameManager.Instance);
    }

    private void RemovePlayer(RoomPlayer player)
    {
        if (!ListItems.ContainsKey(player))
            return;

        var obj = ListItems[player];
        if (obj != null)
        {
            Destroy(obj.gameObject);
            ListItems.Remove(player);
        }
    }

    public void OnDestruction()
    {
    }

    private void ReadyUpListener()
    {
        var local = RoomPlayer.Local;
        if (local && local.Object && local.Object.IsValid)
        {
            local.RPC_ChangeReadyState(!local.IsReady);
        }
    }

    private void EnsureAllPlayersReady(RoomPlayer lobbyPlayer)
    {
        if (!RoomPlayer.Local.IsLeader)
            return;

        if (IsAllReady())
        {
            int scene = ResourceManager.Instance.tracks[GameManager.Instance.TrackId].BuildIndex;
            LevelManager.LoadTrack(scene);
        }
    }

    private static bool IsAllReady() => RoomPlayer.Players.Count > 0 && RoomPlayer.Players.All(player => player.IsReady);
}