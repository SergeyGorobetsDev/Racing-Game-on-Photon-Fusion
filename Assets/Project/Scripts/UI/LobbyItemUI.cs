using UnityEngine;
using UnityEngine.UI;

public class LobbyItemUI : MonoBehaviour
{
    [SerializeField]
    public Text username;
    [SerializeField]
    public Image ready;
    [SerializeField]
    public Image leader;

    private RoomPlayer player;

    private void OnDestroy()
    {
        if (this.player != null)
            RoomPlayer.PlayerChanged -= UpdatePlayerData;
    }

    public void SetPlayer(RoomPlayer player)
    {
        this.player = player;

        if (player.Object != null && this.player == player)
        {
            username.text = player.Username.Value;
            ready.gameObject.SetActive(player.IsReady);
            RoomPlayer.PlayerChanged += UpdatePlayerData;
        }
    }

    private void UpdatePlayerData(RoomPlayer player)
    {
        if (player.Object != null && this.player == player)
        {
            username.text = player.Username.Value;
            ready.gameObject.SetActive(player.IsReady);
            Debug.Log($"Update Player Data Username {player.Username.Value} | Is Ready {player.IsReady}");
        }
    }
}