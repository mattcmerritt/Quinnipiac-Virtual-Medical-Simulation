using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MovablePickup : Trackable
{
    [SerializeField] private bool Selected;
    [SerializeField] private GameObject Target;

    private void Update()
    {
        if (Selected)
        {
            Duration += Time.deltaTime;
        }
    }

    // Debug method for testing
    public void Notify(string content)
    {
        Debug.Log(gameObject.name + ": " + content);
    }

    public void Select(SelectEnterEventArgs selectEnterEventArgs)
    {
        Debug.Log($"<color=blue>SELECTION OCCURRED:</color>\nInteractable: {selectEnterEventArgs.interactableObject}\nInteractor: {selectEnterEventArgs.interactorObject}");
        // as soon as the player first picks up the object, start tracking the time
        if (selectEnterEventArgs.interactorObject.transform.CompareTag("Player"))
        {
            Selected = true;
        }
        // otherwise if the object is selected by a non-player object (likely a socket), check if target
        // if target, task is completed and statistic is issued
        else
        {
            if (selectEnterEventArgs.interactorObject.transform.gameObject == Target)
            {
                Selected = false;
                Accuracy = 1.0f;
                CompleteStatistic();
            }
        }
    }

    public void Deselect(SelectExitEventArgs selectExitEventArgs)
    {
        Debug.Log($"<color=blue>DESELECTION OCCURRED:</color>\nInteractable: {selectExitEventArgs.interactableObject}\nInteractor: {selectExitEventArgs.interactorObject}");
    }
}
