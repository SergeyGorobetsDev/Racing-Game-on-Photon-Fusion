using UnityEngine;
using UnityEngine.UI;

public class NetworkButton : MonoBehaviour
{
    public bool interactableWhileConnected = false;
    private Button btn;

    private void Awake() =>
        btn = GetComponent<Button>();

    private void OnEnable()
    {
        btn.interactable = interactableWhileConnected
            ? GameLauncher.ConnectionStatus == ConnectionStatus.Connected
            : GameLauncher.ConnectionStatus != ConnectionStatus.Connected;
    }
}
