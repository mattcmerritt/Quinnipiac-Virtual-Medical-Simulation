using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectEntry : MonoBehaviour
{
    // Text elements
    [SerializeField] private TMP_Dropdown ObjectTypeDropdown;
    [SerializeField] private TMP_InputField XInput, YInput;
    [SerializeField] private TMP_Text IdText;

    // Data
    private int ObjectId;
    private Vector2 Position;
    private string ObjectType;

    // Map visual components
    [SerializeField] private float MapWidth = 350, MapHeight = 350;
    [SerializeField] private RectTransform ObjectVisual;

    private void Start()
    {
        ObjectType = ObjectTypeDropdown.options[0].text;
    }

    public void SetId(int id)
    {
        ObjectId = id;
        IdText.text = $"Object ID: {ObjectId}";
    }

    public int GetId()
    {
        return ObjectId;
    }

    public void AttachObjectVisual(RectTransform objectVisual)
    {
        ObjectVisual = objectVisual;
    }

    public void UpdateXPosition(string x)
    {
        if (Int32.TryParse(x, out int xValue))
        {
            Position.x = Mathf.Clamp(xValue, -MapWidth / 2, MapWidth / 2);
            XInput.text = $"{Mathf.Clamp(xValue, -MapWidth / 2, MapWidth / 2)}";

            ObjectVisual.anchoredPosition = Position;
        }
    }

    public void UpdateYPosition(string y)
    {
        if (Int32.TryParse(y, out int yValue))
        {
            Position.y = Mathf.Clamp(yValue, -MapHeight / 2, MapHeight / 2);
            YInput.text = $"{Mathf.Clamp(yValue, -MapHeight / 2, MapHeight / 2)}";

            ObjectVisual.anchoredPosition = Position;
        }
    }

    public void UpdateObjectType(int index)
    {
        ObjectType = ObjectTypeDropdown.options[index].text;
    }
}
