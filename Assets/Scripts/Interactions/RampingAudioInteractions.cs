using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampingAudioInteractions : MonoBehaviour
{
    private AudioSource Noise;
    private float IncreaseTimeInterval = 3f;
    private float RemainingTimeInInterval;
    private float VolumeIncreaseInterval = 0.1f;
    private bool Muted;
    
    private void Start()
    {
        Noise = GetComponent<AudioSource>();
        Muted = false;
    }

    private void Update()
    {
        if(!Muted)
        {
            RemainingTimeInInterval -= Time.deltaTime;
            if(RemainingTimeInInterval <= 0f)
            {
                Noise.volume += VolumeIncreaseInterval;
                RemainingTimeInInterval = IncreaseTimeInterval;
            }
        }   
    }

    public void QuietDown()
    {
        Noise.volume = 0f;
    }

    public void ToggleMute()
    {
        Muted = !Muted;
        Noise.volume = 0f;
    }
}
