using FusionExamples.Utility;
using UnityEngine;

public class InterfaceManager : MonoBehaviour
{
    [SerializeField]
    private ProfileSetupUI profileSetup;

    [SerializeField]
    public UIScreen mainMenu;
    [SerializeField]
    public UIScreen pauseMenu;
    [SerializeField]
    public UIScreen lobbyMenu;

    public static InterfaceManager Instance => Singleton<InterfaceManager>.Instance;

    private void Start() =>
        profileSetup.AssertProfileSetup();

    public void OpenPauseMenu()
    {
        if (UIScreen.activeScreen != pauseMenu)
            UIScreen.Focus(pauseMenu);
    }

    public async void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public void SetVolumeMaster(float value) => AudioManager.SetVolumeMaster(value);
    public void SetVolumeSFX(float value) => AudioManager.SetVolumeSFX(value);
    public void SetVolumeUI(float value) => AudioManager.SetVolumeUI(value);
    public void SetVolumeMusic(float value) => AudioManager.SetVolumeMusic(value);
}