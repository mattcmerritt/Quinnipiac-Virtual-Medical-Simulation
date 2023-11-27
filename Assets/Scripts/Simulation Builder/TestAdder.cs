using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TestAdder : MonoBehaviour
{
    [SerializeField] private List<string> GameObjectNames, PrefabNames;
    [SerializeField] private List<float> XPositions;

    [SerializeField] private List<ContentAdder> Adders;
    [SerializeField] private Button Button;

    private void Start()
    {
        Button.onClick.AddListener(() =>
        {
            foreach (ContentAdder adder in Adders)
            {
                adder.AddLayerOfElements(PrefabNames, XPositions, GameObjectNames);
            }
        });
    }
}
