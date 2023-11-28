using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsUI : MonoBehaviour
{
    [SerializeField] private TMP_Text task;
    [SerializeField] private TMP_Text duration;
    [SerializeField] private TMP_Text accuracy;
    // Start is called before the first frame update
    void Start()
    {
        StatisticManager.OnStatisticAdded += UpdateUI;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateUI(Statistic statistic)
    {
        task.text = "Task: " + statistic.task_name;
        duration.text = "Duration: " + statistic.duration.ToString();
        accuracy.text = "Accuracy: " + statistic.accuracy.ToString();

    }
}
