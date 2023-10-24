using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Trackable : MonoBehaviour
{
    [SerializeField] protected string TaskName;
    [SerializeField] protected float Duration;
    [SerializeField] protected float Order;
    [SerializeField] protected float Accuracy;
}
