using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;

public class ToggleObject : MonoBehaviour
{
    private bool IsActive = false;
    [SerializeField] private GameObject ObjectToEnable;
    [SerializeField] private float DistanceToDisable;
    private GameObject Player;

    public void ToggleObjectEnabled()
    {
        IsActive = !IsActive;
        ObjectToEnable.SetActive(IsActive);
    }

    private void Start()
    {
        Player = FindObjectOfType<XROrigin>().gameObject;
    }

    private void Update()
    {
        if(Vector3.Distance(Player.transform.position, transform.position) > DistanceToDisable)
        {
            IsActive = false;
            ObjectToEnable.SetActive(false);
        }
    }
}
