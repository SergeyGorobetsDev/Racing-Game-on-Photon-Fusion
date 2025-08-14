using UnityEngine;
using UnityEngine.UI;

public class UIScreen : MonoBehaviour
{
    public bool isModal = false;

    [SerializeField]
    private UIScreen previousScreen = null;

    [SerializeField]
    private Selectable focusUIElement;
    [SerializeField]
    private bool shouldFocusOnStart;

    public static UIScreen activeScreen;

    public static void Focus(UIScreen screen)
    {
        if (screen == activeScreen)
            return;

        if (activeScreen)
            activeScreen.Defocus();
        screen.previousScreen = activeScreen;
        activeScreen = screen;
        screen.Focus();
    }

    public static void BackToInitial() =>
        activeScreen?.BackTo(null);

    public void FocusScreen(UIScreen screen) =>
        Focus(screen);

    private void Focus()
    {
        if (gameObject)
        {
            gameObject.SetActive(true);
            if (shouldFocusOnStart)
                focusUIElement.Select();
        }
    }

    private void Defocus()
    {
        if (gameObject)
            gameObject.SetActive(false);
    }

    public void Back()
    {
        if (previousScreen)
        {
            Defocus();
            activeScreen = previousScreen;
            activeScreen.Focus();
            previousScreen = null;
        }
    }

    public void BackTo(UIScreen screen)
    {
        while (activeScreen != null && activeScreen.previousScreen != null && activeScreen != screen)
            activeScreen.Back();
    }
}