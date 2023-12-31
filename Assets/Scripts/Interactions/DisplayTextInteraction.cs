using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.CoreUtils;
using TMPro;
using UnityEngine.UI;
using System;

public class DisplayTextInteraction : Trackable
{
    // data points set in builder/scene
    [SerializeField] private DisplayText TextToDisplay;
    [SerializeField] private bool IsTaskInitiallyActive;

    // internals for use during runtime
    private GameObject Player;
    private static Canvas DisplayTextCanvas;
    private TMP_Text TextBox;

    // necessary components for attaching the interaction UI
    private GameObject InteractionUI;
    [SerializeField] public GameObject InteractionUIPrefab;
    [SerializeField] public GameObject InteractionUIButtonPrefab;

    protected new void Start()
    {
        base.Start();
        // TODO: should hook onto existing interface, if one exists
        //  this is so that multiple interactions can be put on the same object

        // spawn child objects
        InteractionUI = Instantiate(InteractionUIPrefab, transform);
        GameObject ButtonHolder = InteractionUI.GetComponentInChildren<VerticalLayoutGroup>().gameObject;
        
        GameObject TalkButtonObject = Instantiate(InteractionUIButtonPrefab, ButtonHolder.transform);
        Button TalkButton = TalkButtonObject.GetComponent<Button>();
        TalkButton.onClick.AddListener(() => {
            DisplayText();
            InteractionUI.SetActive(false);
        });
        TalkButton.GetComponentInChildren<TMP_Text>().text = "Talk";

        // set up ToggleObject script to handle UI despawning at range
        ToggleObject ToggleScript = GetComponent<ToggleObject>();
        ToggleScript.SetObjectToToggle(InteractionUI);

        // if it is initially active, or if there are zero prerequisites, set the interaction as active so timer starts
        if (IsTaskInitiallyActive || PrerequisiteSteps.Count < 1)
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

        float score = 1;
        foreach (Prerequisite prerequisite in PrerequisiteSteps)
        {
            if (!prerequisite.CheckSatisfied())
            {
                score -= prerequisite.GetPenalty();
            }
        }

        Deactivate(score);
        CompleteStatistic();
    }

    public static void CloseText()
    {
        DisplayTextCanvas.enabled = false;
    }

    public void AddDetails(string textName, bool initiallyActive)
    {
        // searching for the associated asset in the resources library
        // TODO: use larger, scalable resource library
        if (textName != "None")
        {
            DisplayText[] textAssets = Resources.FindObjectsOfTypeAll<DisplayText>();
            DisplayText textSource = Array.Find<DisplayText>(textAssets, (DisplayText asset) => asset.name == textName);
            TextToDisplay = textSource;
        }

        IsTaskInitiallyActive = initiallyActive;
    }

    public void AddUIPrefabs(GameObject interactionUIPrefab, GameObject interactionUIButtonPrefab)
    {
        InteractionUIPrefab = interactionUIPrefab;
        InteractionUIButtonPrefab = interactionUIButtonPrefab;
    }
}
