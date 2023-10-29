using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MovablePickup : Trackable
{
    [SerializeField] private string TargetName;
    [SerializeField] private GameObject Target;

    protected new void Start()
    {
        base.Start();

        if (TargetName != null)
        {
            Target = GameObject.Find(TargetName);
        }
    }

    public void Select(SelectEnterEventArgs selectEnterEventArgs)
    {
        Debug.Log($"<color=blue>SELECTION OCCURRED:</color>\nInteractable: {selectEnterEventArgs.interactableObject}\nInteractor: {selectEnterEventArgs.interactorObject}");
        // as soon as the player first picks up the object, start tracking the time
        if (selectEnterEventArgs.interactorObject.transform.CompareTag("Player"))
        {
            Activate();
        }
        // otherwise if the object is selected by a non-player object (likely a socket), check if target
        // if target, task is completed and statistic is issued
        else
        {
            if (selectEnterEventArgs.interactorObject.transform.gameObject == Target)
            {
                Deactivate(1);
                CompleteStatistic();
            }
        }
    }

    public void Deselect(SelectExitEventArgs selectExitEventArgs)
    {
        Debug.Log($"<color=blue>DESELECTION OCCURRED:</color>\nInteractable: {selectExitEventArgs.interactableObject}\nInteractor: {selectExitEventArgs.interactorObject}");
    }
}
