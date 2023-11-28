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
    [SerializeField] private TMP_InputField HeightInput;
    [SerializeField] private TMP_Text IdText;

    // Data
    private int ObjectId;
    private Vector2 Position;
    private float Height;
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

    public string GetObjectType()
    {
        return ObjectType;
    }

    public Vector2 GetPosition()
    {
        return Position;
    }
    
    public float GetHeight()
    {
        return Height;
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

    public void UpdateHeight(string height)
    {
        if (Int32.TryParse(height, out int heightValue))
        {
            Height = heightValue;

            // TODO: sort object visuals based on highest to lowest
        }
    }

    public void UpdateObjectType(int index)
    {
        ObjectType = ObjectTypeDropdown.options[index].text;
    }
}
