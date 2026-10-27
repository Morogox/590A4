
using UnityEngine;
using System.Collections;

public class AnimationSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioSource loopAudioSource;

    public DemonCoreAccident EventManager;

    public void PlaySound()
    {
        if (audioSource != null)
        {
            audioSource.Play();
            

        }
        if (EventManager != null)
        {
            EventManager.StartGlow();
        }

        StartCoroutine(PlayLoopAfterSound());
    }

    private IEnumerator PlayLoopAfterSound()
    {
        yield return new WaitForSeconds(
            Mathf.Max(0f, audioSource.clip.length - 2)
        );

        if (loopAudioSource != null)
            loopAudioSource.Play();
    }
}
