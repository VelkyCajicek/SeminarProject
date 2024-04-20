using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public AudioSource audioSource;
    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }
}
