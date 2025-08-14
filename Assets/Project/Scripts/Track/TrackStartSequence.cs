using System;
using System.Collections;
using UnityEngine;

public class TrackStartSequence : MonoBehaviour
{
    public Action OnComplete;
    public float duration;
    public AudioClip sequenceAudio;

    private AudioSource audioSource;

    public void StartSequence()
    {
        StartCoroutine(PlaySequence());
    }

    public IEnumerator PlaySequence()
    {
        if (sequenceAudio != null)
        {
            if (audioSource == null)
                audioSource = new GameObject("Start Track Sequence Audio").AddComponent<AudioSource>();

            audioSource.clip = sequenceAudio;
            audioSource.Play();

            Destroy(audioSource.gameObject, audioSource.clip.length);
        }

        yield return new WaitForSeconds(duration);

        OnComplete?.Invoke();
    }
}
