
using UnityEngine;

public class AnimationSound : MonoBehaviour
{
    public AudioSource audioSource;

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
            
    }
}
