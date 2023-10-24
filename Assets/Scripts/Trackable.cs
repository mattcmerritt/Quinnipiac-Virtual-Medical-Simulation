using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Trackable : MonoBehaviour
{
    [SerializeField] protected string TaskName;
    [SerializeField] protected float Duration;
    [SerializeField] protected float Accuracy;

    private static event Action<Trackable> OnStatisticCompleted;

    protected void Start()
    {
        OnStatisticCompleted += StatisticManager.Instance.SaveNewStatistic;
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
}
