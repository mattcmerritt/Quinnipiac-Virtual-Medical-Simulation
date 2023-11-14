using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Metadata;

public class SizeFitter : MonoBehaviour
{
    [SerializeField] private bool ShowDebug = false;
    [SerializeField] private bool UpdateRealtime = false;

    // Dimensions
    [SerializeField] private bool RescaleHorizontal, RescaleVertical;

    private void Start()
    {
        GenerateShape();
    }

    private void Update()
    {
        if (UpdateRealtime) GenerateShape();
    }

    public void GenerateShape()
    {
        UpdateBounds();
    }

    private void UpdateBounds()
    {
        // Getting size of bounding shape
        RectTransform children = transform.GetComponentInChildren<RectTransform>();

        float minX = Mathf.Infinity;
        float maxX = Mathf.NegativeInfinity;
        float minY = Mathf.Infinity;
        float maxY = Mathf.NegativeInfinity;

        foreach (RectTransform child in children)
        {
            float childMinX = child.localPosition.x - (child.sizeDelta.x / 2);
            float childMaxX = child.localPosition.x + (child.sizeDelta.x / 2);
            float childMinY = child.localPosition.y - (child.sizeDelta.y / 2);
            float childMaxY = child.localPosition.y + (child.sizeDelta.y / 2);

            if (ShowDebug) Debug.Log($"{child.name}: min: ({childMinX}, {childMinY}), max: ({childMaxX}, {childMaxY})");

            if (childMinX < minX)
            {
                minX = childMinX;
                if (ShowDebug) Debug.Log($"Min X changed to {minX}");
            } 
            if (childMaxX > maxX)
            {
                maxX = childMaxX;
                if (ShowDebug) Debug.Log($"Max X changed to {maxX}");
            }
            if (childMinY < minY)
            {
                minY = childMinY;
                if (ShowDebug) Debug.Log($"Min Y changed to {minY}");
            }
            if (childMaxY > maxY)
            {
                maxY = childMaxY;
                if (ShowDebug) Debug.Log($"Max Y changed to {maxY}");
            }
        }

        if (ShowDebug) Debug.Log($"Final: min: ({minX}, {minY}), max: ({maxX}, {maxY})");

        RectTransform parentTransform = GetComponent<RectTransform>();
        parentTransform.sizeDelta = new Vector2(RescaleHorizontal ? maxX - minX : parentTransform.sizeDelta.x, RescaleVertical ? maxY - minY : parentTransform.sizeDelta.y);

        // Centering
        float centerX = (maxX + minX) / 2f;
        float centerY = (maxY + minY) / 2f;

        if (ShowDebug) Debug.Log($"Center: ({centerX}, {centerY})");

        foreach (RectTransform child in children)
        {
            string result = $"{child.name} Old: {child.localPosition}";
            child.localPosition -= new Vector3(RescaleHorizontal ? centerX : 0, RescaleVertical ? centerY : 0);
            result += $" New: {child.localPosition}";
            if (ShowDebug) Debug.Log(result);
        }
    }

    public float GetLowest()
    {
        float lowest = 0;

        RectTransform children = transform.GetComponentInChildren<RectTransform>();

        foreach (RectTransform child in children)
        {
            float childMinY = child.localPosition.y - (child.sizeDelta.y / 2);

            // Ignore the separator
            if (child.name.Contains("Separator"))
            {
                continue;
            }

            if (ShowDebug) Debug.Log($"{child.name}: min: {childMinY}");

            if (childMinY < lowest)
            {
                lowest = childMinY;
                if (ShowDebug) Debug.Log($"Min Y changed to {lowest}");
            }
        }

        return lowest;
    }
}