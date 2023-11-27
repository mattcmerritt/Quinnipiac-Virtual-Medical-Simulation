using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: need to design some sort of "OngoingTrackable" that doesn't end until the simulation ends
// TODO: need to add Deactivate and CompleteStatistic calls
public class RampingAudioInteractions : Trackable
{
    private AudioSource Noise;
    private float IncreaseTimeInterval = 3f;
    private float RemainingTimeInInterval;
    private float VolumeIncreaseInterval = 0.1f;
    private bool Muted;
    [SerializeField] private float AcceptableVolumeThreshold = 0.7f;
    [SerializeField] private float TimeAboveThreshold;
    
    protected new void Start()
    {
        base.Start();
        Activate();
        TimeAboveThreshold = 0;

        Noise = GetComponent<AudioSource>();
        Muted = false;
    }

    protected new void Update()
    {
        base.Update();

        if(!Muted)
        {
            RemainingTimeInInterval -= Time.deltaTime;
            if(RemainingTimeInInterval <= 0f)
            {
                Noise.volume += VolumeIncreaseInterval;
                RemainingTimeInInterval = IncreaseTimeInterval;
            }

            if(Noise.volume > AcceptableVolumeThreshold)
            {
                TimeAboveThreshold += Time.deltaTime;
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
