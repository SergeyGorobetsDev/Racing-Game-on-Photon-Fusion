using Assets.Project.Scripts.Car;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField]
    public CanvasGroup fader;
    [SerializeField]
    public Animator introAnimator;
    [SerializeField]
    public Animator countdownAnimator;
    [SerializeField]
    public GameObject lapCountContainer;
    [SerializeField]
    public EndRaceUI endRaceScreen;
    [SerializeField]
    public TMP_Text lapCount;
    [SerializeField]
    public TMP_Text raceTimeText;
    [SerializeField]
    public Text[] lapTimeTexts;
    [SerializeField]
    public Text introTrackNameText;
    [SerializeField]
    public Button continueEndButton;
    [SerializeField]
    private bool startedCountdown;
    [SerializeField]
    private TMP_Text carSpeedText;


    public CarEntity ActiveCar { get; private set; }
    private CarMovementController KartController => ActiveCar.CarControllerHandler;

    public void Init(CarEntity carEntity)
    {
        ActiveCar = carEntity;

        IGameUIComponent[] uis = GetComponentsInChildren<IGameUIComponent>(true);
        foreach (var ui in uis)
            ui.Init(carEntity);

        carEntity.CarLapController.OnLapChanged += SetLapCount;

        Track track = Track.Current;
        if (track != null)
            introTrackNameText.text = track.definition.TrackName;
        else Debug.LogWarning($"You need to initialize the GameUI on a track for track-specific values to be updated!");

        continueEndButton.gameObject.SetActive(ActiveCar.Object.HasStateAuthority);
    }

    private void OnDestroy() =>
        ActiveCar.CarLapController.OnLapChanged -= SetLapCount;

    public void FinishCountdown() =>
        ActiveCar.OnRaceStart();

    public void HideIntro() =>
        introAnimator.SetTrigger("Exit");

    private void FadeIn() =>
        StartCoroutine(FadeInRoutine());

    private IEnumerator FadeInRoutine()
    {
        float t = 1;
        while (t > 0)
        {
            fader.alpha = 1 - t;
            t -= Time.deltaTime;
            yield return null;
        }
    }

    private void Update()
    {
        if (!ActiveCar || !ActiveCar.CarLapController.Object || !ActiveCar.CarLapController.Object.IsValid)
            return;

        if (!startedCountdown && Track.Current != null && Track.Current.StartRaceTimer.IsRunning)
        {
            var remainingTime = Track.Current.StartRaceTimer.RemainingTime(ActiveCar.Runner);
            if (remainingTime != null && remainingTime <= 3.0f)
            {
                startedCountdown = true;
                HideIntro();
                FadeIn();
                countdownAnimator.SetTrigger("StartCountdown");
            }
        }

        if (ActiveCar.CarLapController.enabled)
        {
            UpdateLapTimes();
            carSpeedText.text = ActiveCar.CarControllerHandler.CurrentSpeed.ToString("0");
        }
    }

    private void UpdateLapTimes()
    {
        if (!ActiveCar.CarLapController.Object || !ActiveCar.CarLapController.Object.IsValid)
            return;

        //Fusion.NetworkArray<int> lapTimes = ActiveCar.CarLapController.LapTicks;
        //for (var i = 0; i < Mathf.Min(lapTimes.Length, lapTimeTexts.Length); i++)
        //{
        //    int lapTicks = lapTimes.Get(i);

        //    if (lapTicks == 0)
        //        lapTimeTexts[i].text = "";
        //    else
        //    {
        //        int previousTicks = i == 0
        //            ? ActiveCar.CarLapController.StartRaceTick
        //            : lapTimes.Get(i - 1);

        //        int deltaTicks = lapTicks - previousTicks;
        //        float time = TickHelper.TickToSeconds(ActiveCar.Runner, deltaTicks);
        //        SetLapTimeText(time, i);
        //    }
        //}

        SetRaceTimeText(ActiveCar.CarLapController.GetTotalRaceTime());
    }

    private void SetLapCount(int lap, int maxLaps)
    {
        var text = $"{(lap > maxLaps ? maxLaps : lap)}/{maxLaps}";
        lapCount.text = text;
    }

    public void SetRaceTimeText(float time) =>
        raceTimeText.text = $"{(int)(time / 60):00}:{time % 60:00.000}";

    public void SetLapTimeText(float time, int index) =>
        lapTimeTexts[index].text = $"<color=#FFC600>L{index + 1}</color> {(int)(time / 60):00}:{time % 60:00.000}";

    public void ShowEndRaceScreen() =>
        endRaceScreen.gameObject.SetActive(true);

    public void OpenPauseMenu() =>
        InterfaceManager.Instance.OpenPauseMenu();
}