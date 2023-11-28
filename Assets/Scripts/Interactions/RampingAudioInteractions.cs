using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: need to design some sort of "OngoingTrackable" that doesn't end until the simulation ends
// TODO: need to add Deactivate and CompleteStatistic calls
public class RampingAudioInteractions : Trackable
{
    // data points set in builder/scene
    [SerializeField] private float IncreaseTimeInterval;
    [SerializeField] private float VolumeIncreaseInterval;
    [SerializeField] private float AcceptableVolumeThreshold;
    [SerializeField] private bool Loop;
    [SerializeField] private float InitialVolume;
    [SerializeField] private AudioSourceDetails AudioDetails;

    // internals for use during runtime
    private AudioSource Noise;
    private float RemainingTimeInInterval;
    private bool Muted;
    private float TimeAboveThreshold;
    
    private void OnEnable()
    {
        ExitDoor.OnExit += FinalizeStatistics;
    }
    
    private void OnDisable()
    {
        ExitDoor.OnExit -= FinalizeStatistics;
    }

    protected new void Start()
    {
        base.Start();
        Activate();
        TimeAboveThreshold = 0;

        Noise = gameObject.AddComponent<AudioSource>();
        Noise.clip = AudioDetails.audio_clip;
        Noise.loop = Loop;
        Noise.volume = InitialVolume;

        // Noise = GetComponent<AudioSource>();
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

    public void FinalizeStatistics()
    {
        float PercentTimeBelowThreshold = (Duration - TimeAboveThreshold) / Duration;
        Deactivate(PercentTimeBelowThreshold);
        CompleteStatistic();

        Debug.Log("Volume Accuracy: " + Accuracy); // TODO: remove later
    }
}
