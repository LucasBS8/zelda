using UnityEngine;
using System.Collections.Generic;

public class Playsound : MonoBehaviour
{
    public AudioSource audioSource;
    public List<AudioClip> audioClips = new List<AudioClip>();

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
    }

   
    public void Play(int sound)
    {
        Play(sound, 1f); // volume padrão = 100%
    }

   
    public void Play(int sound, float volume)
    {
        Play(sound, volume, 1f, 1f); 
    }

    
    public void Play(int sound, float volume, float minPitch, float maxPitch)
    {
        if (sound >= 0 && sound < audioClips.Count && audioClips[sound] != null)
        {
            float randomPitch = Random.Range(minPitch, maxPitch);
            audioSource.pitch = randomPitch;
            audioSource.PlayOneShot(audioClips[sound], volume);
        }
        else
        {
            Debug.LogWarning("Som inválido ou não atribuído na lista de áudio!");
        }
    }
}
