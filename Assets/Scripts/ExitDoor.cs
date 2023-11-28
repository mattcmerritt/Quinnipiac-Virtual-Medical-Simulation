using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    public static event Action OnExit;

    private void OnEnable()
    {
        OnExit += PushAllStatisticsAtEnd;
    }

    public void ExitScene()
    {
        OnExit?.Invoke();
    }

    public void PushAllStatisticsAtEnd()
    {
        Debug.Log("Simulation ended.");
        // TODO: mark all running Trackables as completed so they are gathered in the statistics at the end

        List<Statistic> statistics = StatisticManager.Instance.GetStatistics();
        // TODO: push statistics to database
    }
}
