using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFadeInOut : MonoBehaviour
{
    private AudioSource source;
    private void Start()
    {
        source = GetComponent<AudioSource>();
        source.volume = 0f;
        StartCoroutine(Fade(true, source, 5f, 1f));
        StartCoroutine(Fade(false, source, 5f, 0f));
    }
    public IEnumerator Fade(bool fadeIn, AudioSource source, float duration, float targetVolume)
    {
        if (!fadeIn)
        {
            double lengthOfSource = (double)source.clip.samples / source.clip.frequency;
            yield return new WaitForSecondsRealtime((float)(lengthOfSource - duration));
        }
        float time = 0f;
        float starVol = source.volume;
        while (time < duration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(starVol, targetVolume, time / duration);
            yield return null;
        }
        yield break;
    }
}
