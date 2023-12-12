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

    public void SetObjectToToggle(GameObject obj)
    {
        ObjectToEnable = obj;
        Start();
    }

    public void ToggleObjectEnabled()
    {
        IsActive = !IsActive;
        ObjectToEnable.SetActive(IsActive);
    }

    private void Start()
    {
        if(ObjectToEnable != null) 
        {
            Player = FindObjectOfType<XROrigin>().gameObject;
            IsActive = false;
            ObjectToEnable.SetActive(false);
        }
    }

    private void Update()
    {
        if(ObjectToEnable != null) 
        {
            if(Vector3.Distance(Player.transform.position, transform.position) > DistanceToDisable)
            {
                IsActive = false;
                ObjectToEnable.SetActive(false);
            }
        }
    }
}
