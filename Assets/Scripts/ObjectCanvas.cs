using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCanvas : MonoBehaviour
{
    GameObject ViewTarget;

    void OnEnable()
    {
        ViewTarget = Camera.main.gameObject;
    }

    void Update()
    {
        transform.LookAt(ViewTarget.transform);
        transform.Rotate(new Vector3(0, 180f, 0));
    }
}
