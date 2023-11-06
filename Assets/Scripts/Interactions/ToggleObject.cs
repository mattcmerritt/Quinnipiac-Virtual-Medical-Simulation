using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleObject : MonoBehaviour
{
    private bool IsActive = false;
    [SerializeField] private GameObject ObjectToEnable;

    public void ToggleObjectEnabled()
    {
        IsActive = !IsActive;
        ObjectToEnable.SetActive(IsActive);
        Debug.Log("called");
    }
}
