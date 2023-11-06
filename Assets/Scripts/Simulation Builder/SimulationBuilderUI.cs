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
        Instantiate(ObjectEntryPrefab, ObjectScrollWindow);
    }

    public void AddEvent()
    {
        Instantiate(EventEntryPrefab, EventScrollWindow);
    }
}
