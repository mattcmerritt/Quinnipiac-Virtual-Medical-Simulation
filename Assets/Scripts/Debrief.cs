using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class Debrief : MonoBehaviour
{
    private float AverageAccuracy;
    private float MaxAccuracy = float.MaxValue;
    private float MinAccuracy= float.MinValue;
    [SerializeField] private TMP_Text FinalMessage;

    public void Start()
    {
    }
    public void Update()
    {
        
    }
    public void generateDebrief(List<Statistic> stats)
    {
        string message;
        float AccuracySum = 0;
        int count = 0;
        foreach(var statistic in stats)
        {
            count++;
            AccuracySum += statistic.accuracy;
        }
        AverageAccuracy = AccuracySum / count;
        int NumTrackables = FindObjectsOfType<Trackable>().Length;
        //Scoring System
        if(AverageAccuracy >= 0.90)
        {
            message = $"Your average accuracy of {AverageAccuracy} indicates that you have achieved mastery of the skills present in this simulation.";
        }else if(AverageAccuracy >= 0.80)
        {
            //Good Score
            message = $"Your average accuracy of {AverageAccuracy} indicates that you are proficient with the skills present in this simulation.";
        }
        else if(AverageAccuracy >= 0.70)
        {
            //Passing Score
            message = $"Your average accuracy of {AverageAccuracy} indicates that you are somewhat comfortable with the skills present in this simulation.";
        }
        else
        {
            //Faling Score
            message = $"Your score of {AverageAccuracy} indicates that you still need to work on the skills present in this simulation.";
        }
        if(Mathf.Abs(NumTrackables - stats.Count) != 0)
        {
            message += " You left no task incompleted";
        }
        else
        {
            message += $" You left {Mathf.Abs(NumTrackables - stats.Count)} tasks unfinished";
        }
        {

        }
        FinalMessage.text = message;
    }
}
