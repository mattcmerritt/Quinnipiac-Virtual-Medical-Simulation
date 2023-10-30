using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Prerequisite
{
    [SerializeField] private Trackable Trackable;
    [SerializeField] private float RequiredAccuracy = 1;
    [SerializeField] private float RequiredDuration = 0;
    [SerializeField] private float FailurePenalty = 0.25f;

    public bool CheckSatisfied()
    {
        return Trackable.GetAccuracy() > RequiredAccuracy && Trackable.GetDuration() > RequiredDuration;
    }
    
    public float GetPenalty()
    {
        return FailurePenalty;
    }
}
