using UnityEngine;

public class AudioDestroyer : MonoBehaviour
{
    [SerializeField]
    private AudioSource src;

    private void Awake() =>
        src = GetComponent<AudioSource>();

    private void OnEnable() =>
        src = GetComponent<AudioSource>();

    private void Update()
    {
        if (src == null) src = GetComponent<AudioSource>();

        if (src.timeSamples == src.clip.samples || src.isPlaying == false)
            Destroy(gameObject);
    }
}
