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

    // private struct StatisticKey
    // {
    //     public string StatisticName;
    //     public float Order;
    // }

    public static event Action<Statistic> OnStatisticAdded;
    [SerializeField] private List<Statistic> Statistics;
    // [SerializeField] private Dictionary<StatisticKey, Statistic> OrderedStatistics;

    // Singleton reference
    public static StatisticManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Statistics = new List<Statistic>();
        // OrderedStatistics = new Dictionary<StatisticKey, Statistic>();
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

        // Replacing statistic if it was already present
        bool replacedInList = false;
        for (int i = 0; i < Statistics.Count; i++)
        {
            if (Statistics[i].TaskName == newStatistic.TaskName)
            {
                Statistics[i] = new Statistic
                {
                    TaskName = Statistics[i].TaskName,
                    Duration = newStatistic.Duration,
                    Order = Statistics[i].Order,
                    Accuracy = newStatistic.Accuracy
                };
                replacedInList = true;
            }
        }

        if (!replacedInList)
        {
            Statistics.Add(newStatistic);
        }

        // OrderedStatistics.Add(new StatisticKey
        // {
        //     StatisticName = newStatistic.TaskName,
        //     Order = newStatistic.Order
        // }, newStatistic);

        OnStatisticAdded?.Invoke(newStatistic);
    }
}
