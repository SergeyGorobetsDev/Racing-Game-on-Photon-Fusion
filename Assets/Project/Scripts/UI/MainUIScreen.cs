using UnityEngine;

public class MainUIScreen : MonoBehaviour
{
    private void Awake() =>
        UIScreen.Focus(GetComponent<UIScreen>());
}
