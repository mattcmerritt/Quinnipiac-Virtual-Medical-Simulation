using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildCollisionDetector : MonoBehaviour
{
    [SerializeField] private ProximityInteraction ProximityInteraction;

    // Events that manage collision detection with hands
    public event Action<GameObject> OnHandEnter;
    public event Action<GameObject> OnHandExit;

    // For prebuilt scenes
    private void Start()
    {
        if (ProximityInteraction != null)
        {
            Configure(ProximityInteraction);
        }
    }

    public void Configure(ProximityInteraction proximityInteraction)
    {
        ProximityInteraction = proximityInteraction;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"<color=red>CHILD ENTERED:</color>\nTarget: {gameObject}\nObject: {other.gameObject}");
        if (other.gameObject.CompareTag("Player"))
        {
            OnHandEnter?.Invoke(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"<color=red>CHILD EXITED:</color>\nTarget: {gameObject}\nObject: {other.gameObject}");
        if (other.gameObject.CompareTag("Player"))
        {
            OnHandExit?.Invoke(other.gameObject);
        }
    }
}
