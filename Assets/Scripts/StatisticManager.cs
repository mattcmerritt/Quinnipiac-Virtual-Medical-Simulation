using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatisticManager : MonoBehaviour
{
    [System.Serializable]
    public struct Statistic
    {
        public string TaskName;
        public float Duration;
        public float Order;
        public float Accuracy;
    }

    public static event Action<Statistic> OnStatisticAdded;
    [SerializeField] private List<Statistic> Statistics;

    // Singleton reference
    public static StatisticManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Statistics = new List<Statistic>();
    }

    public void SaveNewStatistic(Trackable trackable)
    {
        Statistic newStatistic = new Statistic
        {
            TaskName = trackable.GetTaskName(),
            Duration = trackable.GetDuration(),
            Order = Statistics.Count + 1,
            Accuracy = trackable.GetAccuracy()
        };

        Statistics.Add(newStatistic);
        OnStatisticAdded?.Invoke(newStatistic);
    }
}
