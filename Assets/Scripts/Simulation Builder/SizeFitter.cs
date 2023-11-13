using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeFitter : MonoBehaviour
{
    // NOTE: THIS SHOULD ONLY BE FALSE IF THE GAME OBJECT IS ALREADY CENTERED
    //       IF THE COMPONENTS ARE NOT BALANCED, THE BOX WILL NOT FIT
    [SerializeField] private bool Recenter = true;
    [SerializeField] private bool ShowDebug = false;

    public void Start()
    {
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

        if (Recenter)
        {
            float centerX = (maxX + minX) / 2f;
            float centerY = (maxY + minY) / 2f;

            if (ShowDebug) Debug.Log($"Center: ({centerX}, {centerY})");

            foreach (RectTransform child in children)
            {
                string result = $"{child.name} Old: {child.localPosition}";
                child.localPosition -= new Vector3(centerX, centerY);
                result += $" New: {child.localPosition}";
                if (ShowDebug) Debug.Log(result);
            }
        }

        GetComponent<RectTransform>().sizeDelta = new Vector2(maxX - minX, maxY - minY);
    }
}