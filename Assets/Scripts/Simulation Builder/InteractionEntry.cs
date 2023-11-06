using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Reflection;

public class InteractionEntry : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown ObjectDropdown, InteractionDropdown;

    [SerializeField] private string ObjectId;
    [SerializeField] private string InteractionType;

    public void Start()
    {
        ObjectId = ObjectDropdown.options[0].text;
        InteractionType = InteractionDropdown.options[0].text;
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

    private void SelectObjectId(int index)
    {
        ObjectId = ObjectDropdown.options[index].text;
    }

    public void SelectInteractionType(int index)
    {
        InteractionType = InteractionDropdown.options[index].text;
    }
}
