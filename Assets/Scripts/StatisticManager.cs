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

    [SerializeField] private List<Statistic> Statistics;

    // Singleton reference
    public static StatisticManager Instance;

    private void Start()
    {
        Instance = this;
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
    }
}
