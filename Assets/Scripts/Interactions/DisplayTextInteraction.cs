using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.CoreUtils;
using TMPro;

public class DisplayTextInteraction : Trackable
{
    private GameObject Player;
    [SerializeField] private static Canvas DisplayTextCanvas;
    [SerializeField] private TMP_Text TextBox;
    [SerializeField] private DisplayText TextToDisplay;
    [SerializeField] private bool InitiallyActive;

    // Steps attached to this event as prerequesite missions
    [SerializeField] private List<Prerequisite> PrerequisiteSteps;

    protected new void Start()
    {
        base.Start();

        // if it is initially active, or if there are zero prerequisites, set the interaction as active so timer starts
        if (InitiallyActive || PrerequisiteSteps.Count < 1)
        {
            Activate();
        }

        Player = FindObjectOfType<XROrigin>().gameObject;
        DisplayTextCanvas = Player.GetComponentInChildren<Canvas>();
        TextBox = Player.GetComponentInChildren<TMP_Text>();
        DisplayTextCanvas.enabled = false;
    }

    protected new void Update()
    {
        base.Update();

        if(!Started)
        {
            bool currentCheck = true;

            foreach (Prerequisite prereq in PrerequisiteSteps)
            {
                if (!prereq.CheckSatisfied())
                {
                    currentCheck = false;
                }
            }

            if (currentCheck)
            {
                Activate();
            }
        }
    }

    public void DisplayText() 
    {
        TextBox.text = TextToDisplay.text;
        DisplayTextCanvas.enabled = true;
        Deactivate(1); // TODO: make some sort of time-based calculation rather than 1
        CompleteStatistic();
    }

    public static void CloseText()
    {
        DisplayTextCanvas.enabled = false;
    }
}
