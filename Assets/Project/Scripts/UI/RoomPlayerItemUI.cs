using UnityEngine;
using UnityEngine.UI;

public class RoomPlayerItemUI : MonoBehaviour
{
    public Text username;
    public Image ready;
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

        if (this.player != null)
        {
            RoomPlayer.PlayerChanged += UpdatePlayerData;
            username.text = player.Username.Value;
            ready.gameObject.SetActive(player.IsReady);
        }
    }

    private void UpdatePlayerData(RoomPlayer player)
    {
        if (player == this.player)
        {
            username.text = player.Username.Value;
            ready.gameObject.SetActive(player.IsReady);

            Debug.Log($"Update Player Data Username {player.Username.Value} | Is Ready {player.IsReady}");
        }
    }
}