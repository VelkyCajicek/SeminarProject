using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioSource vfxSource;
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }
    public void SetVFXVolume(float volume)
    {
        vfxSource.volume = volume;
    }

}
