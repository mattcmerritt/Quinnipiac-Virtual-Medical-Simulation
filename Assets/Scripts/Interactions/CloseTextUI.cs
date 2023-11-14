using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CloseTextUI : MonoBehaviour
{
    private Canvas Canvas;

    private void Start()
    {
        Canvas = GetComponent<Canvas>();
        Canvas.enabled = false;
    }

    public void OnTrigger()
    {
        Canvas.enabled = false;
    }
}
