using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Trackable : MonoBehaviour
{
    // Statistic tracking information
    [SerializeField] protected string TaskName;
    [SerializeField] protected float Duration;
    [SerializeField] protected float Accuracy;

    // In-game tracking data
    [SerializeField] protected bool Started;

    private static event Action<Trackable> OnStatisticCompleted;

    // Data for identifying this interaction
    [SerializeField] private int InteractionId;

    protected void Start()
    {
        OnStatisticCompleted += StatisticManager.Instance.SaveNewStatistic;
    }

    protected void Update()
    {
        if (Started)
        {
            Duration += Time.deltaTime;
        }
    }

    public void Activate()
    {
        Started = true;
    }

    public void Deactivate(float accuracy)
    {
        Accuracy = accuracy;
        Started = false;
    }

    public string GetTaskName()
    {
        return TaskName;
    }

    public float GetDuration()
    {
        return Duration;
    }

    public float GetAccuracy()
    {
        return Accuracy;
    }

    public void ResetStatistics()
    {
        Duration = 0;
        Accuracy = 0;
    }

    public void CompleteStatistic()
    {
        OnStatisticCompleted?.Invoke(this);
    }

    public void SetInteractionId(int interactionId)
    {
        InteractionId = interactionId;
    }

    public int GetInteractionId()
    {
        return InteractionId;
    }
}
