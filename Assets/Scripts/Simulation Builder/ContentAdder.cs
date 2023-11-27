using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ContentAdder : MonoBehaviour
{
    [SerializeField] private RectTransform AddPrerequisiteButton;
    [SerializeField] private float PrerequisiteGap = 15;
    [SerializeField] private RectTransform Separator;
    [SerializeField] private float SeparatorGap = 35;
    [SerializeField] private float Lowest;

    [SerializeField] private SizeFitter Fitter;

    // Parallel arrays for building dictionary
    [SerializeField] private List<string> UIObjectKeys;
    [SerializeField] private List<GameObject> UIObjectGameObject;
    private Dictionary<string, GameObject> UIObjects;

    private void Start()
    {
        UIObjects = new Dictionary<string, GameObject>();
        for (int i = 0; i < UIObjectKeys.Count; i++)
        {
            UIObjects.Add(UIObjectKeys[i], UIObjectGameObject[i]);
        }
    }

    private void Update()
    {
        Lowest = Fitter.GetLowest();
    }

    public GameObject AddNewElement(string elementKey, float xPosition, string objectName)
    {
        Lowest = Fitter.GetLowest();

        UIObjects.TryGetValue(elementKey, out GameObject elementPrefab);

        GameObject element = Instantiate(elementPrefab, transform);
        element.transform.localScale = Vector3.one;
        element.gameObject.name = objectName;

        RectTransform elementTransform = element.GetComponent<RectTransform>();
        elementTransform.localPosition = new Vector3(xPosition, Lowest - elementTransform.sizeDelta.y / 2);

        Lowest = Fitter.GetLowest();

        AddPrerequisiteButton.transform.localPosition = new Vector3(AddPrerequisiteButton.transform.localPosition.x, Lowest - PrerequisiteGap);
        Separator.transform.localPosition = new Vector3(Separator.transform.localPosition.x, Lowest - SeparatorGap);
        Fitter.GenerateShape();

        return element;
    }

    public List<GameObject> AddLayerOfElements(List<string> elementKeys, List<float> xPositions, List<string> objectNames)
    {
        List<GameObject> items = new List<GameObject>();

        Lowest = Fitter.GetLowest();

        for (int i = 0; i < elementKeys.Count;i++)
        {
            UIObjects.TryGetValue(elementKeys[i], out GameObject elementPrefab);

            GameObject element = Instantiate(elementPrefab, transform);
            element.transform.localScale = Vector3.one;
            element.gameObject.name = objectNames[i];

            RectTransform elementTransform = element.GetComponent<RectTransform>();
            elementTransform.localPosition = new Vector3(xPositions[i], Lowest - elementTransform.sizeDelta.y / 2);

            items.Add(element);
        }

        Lowest = Fitter.GetLowest();

        AddPrerequisiteButton.transform.localPosition = new Vector3(AddPrerequisiteButton.transform.localPosition.x, Lowest - PrerequisiteGap);
        Separator.transform.localPosition = new Vector3(Separator.transform.localPosition.x, Lowest - SeparatorGap);
        Fitter.GenerateShape();

        return items;
    }
}
