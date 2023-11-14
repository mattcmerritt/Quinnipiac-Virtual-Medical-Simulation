using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrerequisiteAdder : MonoBehaviour
{
    private List<string> GameObjectNames = new List<string> { "Prerequisite Title", "Prerequisite Dropdown" };
    private List<string> PrefabNames = new List<string> { "Text", "Dropdown" };
    private List<float> XPositions = new List<float> { -80, 45 };

    [SerializeField] private ContentAdder Adder;
    [SerializeField] private InteractionEntry InteractionEntry;
    [SerializeField] private Button Button;

    private void Start()
    {
        Button.onClick.AddListener(() =>
        {
            List<GameObject> newItems = Adder.AddLayerOfElements(PrefabNames, XPositions, GameObjectNames);
            InteractionEntry.AddPrerequisite(newItems[1].GetComponent<TMP_Dropdown>());
        });
    }
}
