using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatisticManager : MonoBehaviour
{
    // [System.Serializable]
    // public struct Statistic
    // {
    //     public string TaskName;
    //     public float Duration;
    //     public float Order;
    //     public float Accuracy;
    // }

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
        Statistic newStatistic = new Statistic(trackable.GetTaskName(), trackable.GetDuration(), Statistics.Count + 1, trackable.GetAccuracy());

        // Replacing statistic if it was already present
        bool replacedInList = false;
        for (int i = 0; i < Statistics.Count; i++)
        {
            if (Statistics[i].task_name == newStatistic.task_name)
            {
                Statistics[i] = new Statistic(Statistics[i].task_name, newStatistic.duration, Statistics[i].order, newStatistic.accuracy);
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

    public List<Statistic> GetStatistics()
    {
        return Statistics;
    }
}

[System.Serializable]
public class Statistic
{
    public string task_name { get; set; }
    public float duration { get; set; }
    public float order { get; set; }
    public float accuracy { get; set; }

    public Statistic(string task_name, float duration, float order, float accuracy)
    {
        this.task_name = task_name;
        this.duration = duration;
        this.order = order;
        this.accuracy = accuracy;
    }
}