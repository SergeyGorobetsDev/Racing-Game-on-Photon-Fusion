using UnityEngine;
using UnityEngine.UI;

public class ProfileSetupUI : MonoBehaviour
{
    [SerializeField]
    public InputField nicknameInput;
    [SerializeField]
    public Button confirmButton;

    private void Start() =>
        Initialize();

    private void Initialize()
    {
        nicknameInput.text = SaveManager.Instance.GetUserData().Username;
    }

    private void OnEnable() =>
        nicknameInput.onValueChanged.AddListener(NicknameInputChanged);

    private void OnDisable() =>
        nicknameInput.onValueChanged.RemoveListener(NicknameInputChanged);

    public void AssertProfileSetup()
    {
        if (string.IsNullOrEmpty(SaveManager.Instance.GetUserData().Username))
        {
            UIScreen.Focus(GetComponent<UIScreen>());
        }
    }

    private void NicknameInputChanged(string nickname)
    {
        if (string.IsNullOrEmpty(nickname))
        {
            confirmButton.interactable = !string.IsNullOrEmpty(nickname);
            return;
        }

        SaveManager.Instance.GetUserData().Username = nickname;
        confirmButton.interactable = true;
    }
}