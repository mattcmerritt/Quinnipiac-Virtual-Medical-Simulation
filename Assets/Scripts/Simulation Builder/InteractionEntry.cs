using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Reflection;
using UnityEngine.Events;
using UnityEngine.UI;

public class InteractionEntry : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown ObjectDropdown, InteractionDropdown;
    [SerializeField] private TMP_Text IdText;

    [SerializeField] private string ObjectId;
    [SerializeField] private string InteractionType;

    [SerializeField] private bool HasAccuracyRequirements;
    [SerializeField] private float AccuracyPenalty, DurationRequired;
    [SerializeField] private List<int> PrerequisiteList;

    [SerializeField] private List<TMP_Dropdown> PrerequisiteDropdowns;
    [SerializeField] private List<TMP_InputField> AccuracyFields;

    [SerializeField] private ContentAdder ContentAdder;

    // Data
    private int InteractionId;
    private List<string> SavedInteractionIds;

    // Data that is sepcific to certain classes of interaction
    // Audio interaction data
    private float VolumeIncreaseTimeInterval, VolumeIncreaseMagnitude, AcceptableVolumeThreshold, InitialVolume;
    private bool Loop;
    private string SelectedAudioSource;

    private void Start()
    {
        if (ObjectDropdown.options.Count > 0)
        {
            ObjectId = ObjectDropdown.options[0].text;
        }

        InteractionType = InteractionDropdown.options[0].text;
    }

    private void Update()
    {
        if (!string.IsNullOrEmpty(ObjectId) && ObjectDropdown.options.Count > 0)
        {
            ObjectId = ObjectDropdown.options[0].text;
        }
    }

    public void SetId(int id)
    {
        InteractionId = id;
        IdText.text = $"Interaction ID: {InteractionId}";
    }

    public int GetId()
    {
        return InteractionId;
    }

    public string GetObjectId()
    {
        return ObjectId;
    }

    public string GetInteractionType()
    {
        return InteractionType;
    }

    public List<int> GetPrerequisiteList()
    {
        return PrerequisiteList;
    }

    public float GetAccuracyPenalty()
    {
        return AccuracyPenalty;
    }

    public float GetDuration()
    {
        return DurationRequired;
    }

    public void SetObjectOptions(List<string> objectIds)
    {
        string previousId = ObjectId;

        ObjectDropdown.ClearOptions();
        ObjectDropdown.AddOptions(objectIds);

        int newIdIndex = objectIds.FindIndex((string id) => id == previousId);

        if (newIdIndex != -1)
        {
            ObjectDropdown.SetValueWithoutNotify(newIdIndex);
        }
        else
        {
            ObjectDropdown.SetValueWithoutNotify(0);
        }
    }

    public void SelectObjectId(int index)
    {
        ObjectId = ObjectDropdown.options[index].text;
    }

    public void SelectInteractionType(int index)
    {
        InteractionType = InteractionDropdown.options[index].text;

        // Adding accuracy information
        if (AccuracyFields.Count != 0)
        {
            // TODO: remove and rescale elements, not disable
            foreach (TMP_InputField accuracyField in AccuracyFields)
            {
                accuracyField.enabled = false;
            }
            
        }
        else
        {
            // TODO: put this in a function or a separate adder, but not here
            if (InteractionType == "Proximity Task" && !HasAccuracyRequirements)
            {
                // Mark that the new fields have been added
                HasAccuracyRequirements = true;

                // Data to load in the new fields
                List<string> namesRow1 = new List<string> { "Duration Label", "Duration Input" };
                List<string> namesRow2 = new List<string> { "Accuracy Label", "Accuracy Input" };
                List<string> prefabsRow1 = new List<string> { "Text", "Integer Input" };
                List<string> prefabsRow2 = new List<string> { "Text", "Decimal Input" };
                List<float> positions = new List<float> { -60, 0 };

                // adding and setting up listeners for the duration input
                List<GameObject> row1Items = ContentAdder.AddLayerOfElements(prefabsRow1, positions, namesRow1);
                row1Items[0].GetComponent<TMP_Text>().text = "Duration:";
                AccuracyFields.Add(row1Items[1].GetComponent<TMP_InputField>());

                // adding the listener to the new duration text field to track the value input in this class
                row1Items[1].GetComponent<TMP_InputField>().onValueChanged.AddListener((string duration) => {
                    if (Int32.TryParse(duration, out int durationValue))
                    {
                        DurationRequired = durationValue;
                    }
                });

                // resizing and scaling elements to fit
                SimulationBuilderUI simBuilder = FindObjectOfType<SimulationBuilderUI>();
                simBuilder.UpdateListSizes();

                // adding and setting up listeners for the accuracy penalty input
                List<GameObject> row2Items = ContentAdder.AddLayerOfElements(prefabsRow2, positions, namesRow2);
                row2Items[0].GetComponent<TMP_Text>().text = "Accuracy:";
                AccuracyFields.Add(row2Items[1].GetComponent<TMP_InputField>());

                // adding the listener to the new accuracy text field to track the value input in this class
                row2Items[1].GetComponent<TMP_InputField>().onValueChanged.AddListener((string accuracy) => {
                    if (float.TryParse(accuracy, out float accuracyValue))
                    {
                        AccuracyPenalty = accuracyValue;
                    }
                });

                // final resizing and scaling elements to fit
                simBuilder.UpdateListSizes();
            }
            else if (InteractionType == "Audio Task" && !HasAccuracyRequirements)
            {
                // Mark that the new fields have been added
                HasAccuracyRequirements = true;

                // Data to load in the new fields
                // TODO: find a way to possibly compress this into some sort of structure
                //      would be ideal if we could make a tuple
                //      likely possible with structs
                List<List<string>> nameRows = new List<List<string>>
                {
                    new List<string> { "Time Interval Label", "Time Interval Input" },
                    new List<string> { "Magnitude Increase Label", "Magnitude Increase Input" },
                    new List<string> { "Acceptable Threshold Label", "Acceptable Threshold Input" },
                    new List<string> { "Loop Toggle"},
                    new List<string> { "Initial Volume Label", "Initial Volume Input" },
                    new List<string> { "Audio Source Label", "Audio Source Dropdown" }
                };

                List<List<string>> prefabRows = new List<List<string>>
                {
                    new List<string> { "Text", "Decimal Input" },
                    new List<string> { "Text", "Decimal Input" },
                    new List<string> { "Text", "Decimal Input" },
                    new List<string> { "Toggle" },
                    new List<string> { "Text", "Decimal Input" },
                    new List<string> { "Text", "Dropdown" }
                };

                List<List<float>> positions = new List<List<float>>
                {
                    new List<float> { -60, 10 },
                    new List<float> { -60, 10 },
                    new List<float> { -60, 10 },
                    new List<float> { -40 },
                    new List<float> { -60, 10 },
                    new List<float> { -60, 50 }
                };

                List<List<string>> rowItemData = new List<List<string>>
                {
                    new List<string> { "Interval:" },
                    new List<string> { "Magnitude:" },
                    new List<string> { "Threshold:" },
                    new List<string> { "Loop:" },
                    new List<string> { "Initial Vol:" },
                    new List<string> { "Audio:" },
                };

                // TODO: find a way to merge all three event listener lists into a single list
                //      one solution could be to design a lot of objects to set up inheritance
                //      would allow for leveraging generics
                List<List<UnityAction<string>>> stringEvents = new List<List<UnityAction<string>>>
                {
                    new List<UnityAction<string>> { (string input) => { if (Int32.TryParse(input, out int value)) VolumeIncreaseTimeInterval = value; } },
                    new List<UnityAction<string>> { (string input) => { if (Int32.TryParse(input, out int value)) VolumeIncreaseMagnitude = value; } },
                    new List<UnityAction<string>> { (string input) => { if (Int32.TryParse(input, out int value)) AcceptableVolumeThreshold = value; } },
                    null,
                    new List<UnityAction<string>> { (string input) => { if (Int32.TryParse(input, out int value)) InitialVolume = value; } },
                    null,
                };

                // constructing the interaction's new fields
                SimulationBuilderUI simBuilder = FindObjectOfType<SimulationBuilderUI>();
                for (int row = 0; row < nameRows.Count; row++)
                {
                    // adding content
                    List<GameObject> rowItems = ContentAdder.AddLayerOfElements(prefabRows[row], positions[row], nameRows[row]);

                    // configuring labels to read with proper text
                    if (prefabRows[row][0] == "Text")
                    {
                        rowItems[0].GetComponent<TMP_Text>().text = rowItemData[row][0];
                    }
                    // update toggle text and add listener
                    else if (prefabRows[row][0] == "Toggle")
                    {
                        rowItems[0].GetComponentInChildren<TMP_Text>().text = rowItemData[row][0];
                        // TODO: when generics and objects are implemented, make this its own unity action rather than being hard coded
                        rowItems[0].GetComponent<Toggle>().onValueChanged.AddListener((bool value) => Loop = value);
                    }

                    // adding the listener to the new text field to track the value input in this class
                    if (prefabRows[row].Count == 1)
                    {
                        continue; // continue to next row
                    }
                    else if (prefabRows[row][1].Contains("Input"))
                    {
                        rowItems[1].GetComponent<TMP_InputField>().onValueChanged.AddListener(stringEvents[row][0]);
                    }
                    // preparing dropdown options
                    // TODO: when generics and objects are implemented, make this its own unity action rather than being hard coded
                    else if (prefabRows[row][1] == "Dropdown")
                    {
                        TMP_Dropdown drop = rowItems[1].GetComponent<TMP_Dropdown>();
                        drop.ClearOptions();
                        drop.AddOptions(new List<string> { "BackgroundTalking" });
                        drop.onValueChanged.AddListener((int index) => SelectedAudioSource = drop.options[drop.value].text);
                    }

                    // resizing and scaling elements to fit
                    simBuilder.UpdateListSizes();
                }
            }
        }
    }

    public void SetInteractionOptions(List<string> interactionIds)
    {
        // Save options for future prerequisites
        SavedInteractionIds = interactionIds;

        foreach (TMP_Dropdown prerequisiteDropdown in PrerequisiteDropdowns)
        {
            string previousId = prerequisiteDropdown.options[prerequisiteDropdown.value].text;

            prerequisiteDropdown.ClearOptions();
            prerequisiteDropdown.AddOptions(interactionIds);

            int newIdIndex = interactionIds.FindIndex((string id) => id == previousId);

            if (newIdIndex != -1)
            {
                prerequisiteDropdown.SetValueWithoutNotify(newIdIndex);
            }
            else
            {
                prerequisiteDropdown.SetValueWithoutNotify(0);
            }
        }
    }

    public void UpdatePrerequisiteList()
    {
        // regenerating the entire list of prerequisite ids
        List<int> newPrerequisiteList = new List<int>();
        foreach (TMP_Dropdown d in PrerequisiteDropdowns)
        {
            Int32.TryParse(d.options[d.value].text, out int prereqNumber);
            newPrerequisiteList.Add(prereqNumber);
        }
        PrerequisiteList = newPrerequisiteList;
    }

    public void AddPrerequisite(TMP_Dropdown dropdown)
    {
        if (SavedInteractionIds != null)
        {
            dropdown.ClearOptions();
            dropdown.AddOptions(SavedInteractionIds);
        }
        PrerequisiteDropdowns.Add(dropdown);

        // adding a listener to the dropdown
        dropdown.onValueChanged.AddListener((int index) => UpdatePrerequisiteList());
        // updating the list of prerequisites to include this new one
        UpdatePrerequisiteList();

        SimulationBuilderUI simBuilder = FindObjectOfType<SimulationBuilderUI>();
        simBuilder.UpdateListSizes();
    }
}
