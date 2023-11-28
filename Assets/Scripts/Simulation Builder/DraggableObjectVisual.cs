using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableObjectVisual : MonoBehaviour, IDragHandler, IPointerDownHandler, IEndDragHandler
{
    private ObjectEntry LinkedObjectEntry;
    private Vector3 OffsetFromClickPoint;

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, eventData.position, eventData.pressEventCamera, out globalMousePos))
        {
            transform.position = globalMousePos + OffsetFromClickPoint;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector3 mousePosInRect;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, eventData.position, eventData.pressEventCamera, out mousePosInRect);
        OffsetFromClickPoint = transform.position - mousePosInRect;
        // Debug.Log("offset set: " + OffsetFromClickPoint);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Debug.Log(transform.localPosition);
        LinkedObjectEntry.UpdateXYPosition((int)transform.localPosition.x, (int)transform.localPosition.y);
    }

    public void SetLinkedObjectEntry(ObjectEntry linkedObject)
    {
        LinkedObjectEntry = linkedObject;
    }
}
