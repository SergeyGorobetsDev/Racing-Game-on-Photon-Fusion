using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegionUI : MonoBehaviour
{
    private readonly string[] options = new string[] { "us", "eu", "asia" };

    [SerializeField]
    private Dropdown dropdown;

    private void Awake()
    {
        if (dropdown == null)
            if (TryGetComponent(out Dropdown dropdown))
                this.dropdown = dropdown;

        dropdown.AddOptions(new List<string>(options));
        dropdown.onValueChanged.AddListener((index) =>
        {
            string region = dropdown.options[index].text;
            Fusion.Photon.Realtime.PhotonAppSettings.Global.AppSettings.FixedRegion = region;
        });

        string curRegion = Fusion.Photon.Realtime.PhotonAppSettings.Global.AppSettings.FixedRegion;
        int curIndex = dropdown.options.FindIndex((op) => op.text == curRegion);
        dropdown.value = curIndex != -1 ? curIndex : 0;
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    private void RegionChanged(int id)
    {

    }
}