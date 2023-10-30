using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ClickInteraction : Trackable
{
    // Data to manually generate interaction script components
    // [SerializeField] private string ClickableObjectName;
    // [SerializeField] private GameObject ClickableObject;

    protected new void Start()
    {
        base.Start();

        // if (ClickableObjectName != null)
        // {
        //     ClickableObject = GameObject.Find(ClickableObjectName);

        //     // Adding the XR components
        //     XRSimpleInteractable interactable = ClickableObject.AddComponent<XRSimpleInteractable>();
        //     interactable.selectEntered.AddListener(Select);
        //     interactable.selectExited.AddListener(Deselect);
        // }
    }

    public void Select(SelectEnterEventArgs selectEnterEventArgs)
    {
        Debug.Log($"<color=blue>SELECTION OCCURRED:</color>\nInteractable: {selectEnterEventArgs.interactableObject}\nInteractor: {selectEnterEventArgs.interactorObject}");
        // as soon as the player first picks up the object, start tracking the time
        if (selectEnterEventArgs.interactorObject.transform.CompareTag("Player"))
        {
            Activate();
        }
    }

    public void Deselect(SelectExitEventArgs selectExitEventArgs)
    {
        Debug.Log($"<color=blue>DESELECTION OCCURRED:</color>\nInteractable: {selectExitEventArgs.interactableObject}\nInteractor: {selectExitEventArgs.interactorObject}");
        // as soon as the player first picks up the object, start tracking the time
        if (selectExitEventArgs.interactorObject.transform.CompareTag("Player"))
        {
            Deactivate(1);
            CompleteStatistic();
        }
    }
}
