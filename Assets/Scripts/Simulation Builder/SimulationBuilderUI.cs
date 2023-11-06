using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulationBuilderUI : MonoBehaviour
{
    // UI Components and Elements
    [SerializeField] private Button ObjectTabButton, EventTabButton;
    [SerializeField] private GameObject ObjectPanel, EventPanel;
    [SerializeField] private Transform ObjectScrollWindow, EventScrollWindow;

    // Prefabs to instantiate entries in the lists
    [SerializeField] private GameObject ObjectEntryPrefab, EventEntryPrefab;

    // Data for the map visualization
    [SerializeField] private Transform Map;
    [SerializeField] private GameObject ObjectPrefab;

    // Added data
    [SerializeField] private List<ObjectEntry> ObjectEntries;
    [SerializeField] private List<string> ObjectIds;
    [SerializeField] private List<InteractionEntry> InteractionEntries;

    private void Start()
    {
        ObjectEntries = new List<ObjectEntry>();
        ObjectIds = new List<string>();
        InteractionEntries = new List<InteractionEntry>();
    }

    public void SwitchToObjectsTab()
    {
        ObjectTabButton.interactable = false;
        EventTabButton.interactable = true;

        ObjectPanel.SetActive(true);
        EventPanel.SetActive(false);
    }

    public void SwitchToEventsTab()
    {
        ObjectTabButton.interactable = true;
        EventTabButton.interactable = false;

        ObjectPanel.SetActive(false);
        EventPanel.SetActive(true);
    }

    public void AddObject()
    {
        GameObject objectEntry = Instantiate(ObjectEntryPrefab, ObjectScrollWindow);
        GameObject objectVisual = Instantiate(ObjectPrefab, Map);

        ObjectEntry objectEntryScript = objectEntry.GetComponent<ObjectEntry>();
        RectTransform visualTransform = objectVisual.GetComponent<RectTransform>();

        objectEntryScript.SetId(ObjectEntries.Count);
        objectEntryScript.AttachObjectVisual(visualTransform);

        ObjectIds.Add($"{ObjectEntries.Count}");
        ObjectEntries.Add(objectEntryScript);

        // updating all of the options for the interactions
        foreach (InteractionEntry interaction in InteractionEntries)
        {
            interaction.SetObjectOptions(ObjectIds);
        }
    }

    public void AddEvent()
    {
        GameObject interactionEntry = Instantiate(EventEntryPrefab, EventScrollWindow);

        InteractionEntry interactionEntryScript = interactionEntry.GetComponent<InteractionEntry>();

        interactionEntryScript.SetObjectOptions(ObjectIds);

        InteractionEntries.Add(interactionEntryScript);
    }
}
