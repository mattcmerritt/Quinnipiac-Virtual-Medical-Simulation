using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RampingAudioInteraction : Trackable
{
    // data points set in builder/scene
    [SerializeField] private float VolumeIncreaseTimeInterval;
    [SerializeField] private float VolumeIncreaseMagnitude;
    [SerializeField] private float AcceptableVolumeThreshold;
    [SerializeField] private bool Loop;
    [SerializeField] private float InitialVolume;
    [SerializeField] private AudioSourceDetails AudioDetails;

    // internals for use during runtime
    private AudioSource Noise;
    private float RemainingTimeInInterval;
    private bool Muted;
    private float TimeAboveThreshold;

    // necessary components for attaching the interaction UI
    private GameObject InteractionUI;
    [SerializeField] public GameObject InteractionUIPrefab;
    [SerializeField] public GameObject InteractionUIButtonPrefab;
    
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

        // spawn child objects
        InteractionUI = Instantiate(InteractionUIPrefab, transform);
        GameObject ButtonHolder = InteractionUI.GetComponentInChildren<VerticalLayoutGroup>().gameObject;
        
        GameObject QuietButtonObject = Instantiate(InteractionUIButtonPrefab, ButtonHolder.transform);
        Button QuietButton = QuietButtonObject.GetComponent<Button>();
        QuietButton.onClick.AddListener(() => {
            QuietDown();
            InteractionUI.SetActive(false);
        });
        QuietButton.GetComponentInChildren<TMP_Text>().text = "Quiet down";

        GameObject MuteButtonObject = Instantiate(InteractionUIButtonPrefab, ButtonHolder.transform);
        Button MuteButton = MuteButtonObject.GetComponent<Button>();
        MuteButton.onClick.AddListener(() => {
            ToggleMute();
            InteractionUI.SetActive(false);
        });
        MuteButton.GetComponentInChildren<TMP_Text>().text = "Mute";

        // set up ToggleObject script to handle UI despawning at range
        ToggleObject ToggleScript = GetComponent<ToggleObject>();
        ToggleScript.SetObjectToToggle(InteractionUI);
        
        Activate();
        TimeAboveThreshold = 0;

        Noise = gameObject.AddComponent<AudioSource>();
        Noise.clip = AudioDetails.audio_clip;
        Noise.loop = Loop;
        Noise.volume = InitialVolume;

        Muted = false;

        Noise.Play();
    }

    protected new void Update()
    {
        base.Update();

        if(!Muted)
        {
            RemainingTimeInInterval -= Time.deltaTime;
            if(RemainingTimeInInterval <= 0f)
            {
                Noise.volume += VolumeIncreaseMagnitude;
                RemainingTimeInInterval = VolumeIncreaseTimeInterval;
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

        float score = PercentTimeBelowThreshold;
        foreach (Prerequisite prerequisite in PrerequisiteSteps)
        {
            if (!prerequisite.CheckSatisfied())
            {
                score -= prerequisite.GetPenalty();
            }
        }

        Deactivate(score);
        CompleteStatistic();
    }
}
