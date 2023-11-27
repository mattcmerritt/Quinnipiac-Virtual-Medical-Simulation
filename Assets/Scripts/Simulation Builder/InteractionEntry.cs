using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Reflection;

public class InteractionEntry : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown ObjectDropdown, InteractionDropdown;
    [SerializeField] private TMP_Text IdText;

    [SerializeField] private string ObjectId;
    [SerializeField] private string InteractionType;

    [SerializeField] private List<TMP_Dropdown> PrerequisiteDropdowns;
    [SerializeField] private List<TMP_InputField> AccuracyFields;

    [SerializeField] private ContentAdder ContentAdder;

    // Data
    private int InteractionId;
    private List<string> SavedInteractionIds;

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
            if (InteractionType == "Proximity Task")
            {
                List<string> namesRow1 = new List<string> { "Duration Label", "Duration Input" };
                List<string> namesRow2 = new List<string> { "Accuracy Label", "Accuracy Input" };
                List<string> prefabsRow1 = new List<string> { "Text", "Integer Input" };
                List<string> prefabsRow2 = new List<string> { "Text", "Decimal Input" };
                List<float> positions = new List<float> { -60, 0 };

                List<GameObject> row1Items = ContentAdder.AddLayerOfElements(prefabsRow1, positions, namesRow1);

                row1Items[0].GetComponent<TMP_Text>().text = "Duration:";
                AccuracyFields.Add(row1Items[1].GetComponent<TMP_InputField>());

                SimulationBuilderUI simBuilder = FindObjectOfType<SimulationBuilderUI>();
                simBuilder.UpdateListSizes();

                List<GameObject> row2Items = ContentAdder.AddLayerOfElements(prefabsRow2, positions, namesRow2);
                row2Items[0].GetComponent<TMP_Text>().text = "Accuracy:";
                AccuracyFields.Add(row2Items[1].GetComponent<TMP_InputField>());

                simBuilder.UpdateListSizes();
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

    public void AddPrerequisite(TMP_Dropdown dropdown)
    {
        if (SavedInteractionIds != null)
        {
            dropdown.ClearOptions();
            dropdown.AddOptions(SavedInteractionIds);
        }
        PrerequisiteDropdowns.Add(dropdown);

        SimulationBuilderUI simBuilder = FindObjectOfType<SimulationBuilderUI>();
        simBuilder.UpdateListSizes();
    }
}
