using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Scripting.Python;
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

    private void UpdateUI(StatisticManager.Statistic statistic)
    {
        task.text = "Task: " + statistic.TaskName;
        duration.text = "Duration: " + statistic.Duration.ToString();
        accuracy.text = "Accuracy: " + statistic.Accuracy.ToString();
        PythonRunner.RunFile($"{Application.dataPath}/Python/test.py");

    }
}
