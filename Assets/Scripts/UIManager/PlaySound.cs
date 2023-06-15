using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public AudioSource soundPlayer;
    public AudioClip selectedSoundClip;
    public AudioClip pointSoundClip;

    public void PlaySelectedSound()
    {
        soundPlayer.PlayOneShot(selectedSoundClip);
    }

    public void PlayPointSound()
    {
        soundPlayer.PlayOneShot(pointSoundClip);

    }

}
