using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.CoreUtils;
using TMPro;
using UnityEngine.UI;

public class DisplayTextInteraction : Trackable
{
    // data points set in builder/scene
    [SerializeField] private DisplayText TextToDisplay;
    [SerializeField] private bool IsTaskInitiallyActive;
    [SerializeField] private List<Prerequisite> PrerequisiteSteps;

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
}
