using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.CoreUtils;
using TMPro;

public class DisplayTextInteraction : MonoBehaviour
{
    private GameObject Player;
    [SerializeField] private static Canvas DisplayTextCanvas;
    [SerializeField] private TMP_Text TextBox;
    [SerializeField] private DisplayText TextToDisplay;

    private void Start()
    {
        Player = FindObjectOfType<XROrigin>().gameObject;
        DisplayTextCanvas = Player.GetComponentInChildren<Canvas>();
        TextBox = Player.GetComponentInChildren<TMP_Text>();
        DisplayTextCanvas.enabled = false;
    }

    public void DisplayText() 
    {
        TextBox.text = TextToDisplay.Text;
        DisplayTextCanvas.enabled = true;
    }

    public static void CloseText()
    {
        DisplayTextCanvas.enabled = false;
    }
}
