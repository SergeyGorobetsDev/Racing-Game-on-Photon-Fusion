using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField]
    public string mixerParameter;
    [SerializeField]
    public string mixerGroup;
    [SerializeField]
    private float lastVal;

    private Slider slider;

    private void OnEnable()
    {
        if (TryGetComponent(out Slider slider))
        {
            this.slider = slider;
            lastVal = slider.value = PlayerPrefs.GetFloat(mixerParameter, 0.75f);
            slider.onValueChanged.AddListener(ChangeValue);
        }
    }

    private void OnDisable() =>
        slider.onValueChanged.RemoveListener(ChangeValue);

    private void ChangeValue(float value)
    {
        if (Mathf.Round(value * 10) != Mathf.Round(lastVal * 10))
        {
            AudioManager.Play("hoverUI", mixerGroup);
            lastVal = value;
        }
    }
}
