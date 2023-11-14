using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentAdder : MonoBehaviour
{
    [SerializeField] private RectTransform Separator;
    [SerializeField] private float SeparatorGap = 5;
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

    public void AddNewElement(string elementKey, float xPosition, string objectName)
    {
        UIObjects.TryGetValue(elementKey, out GameObject elementPrefab);

        GameObject element = Instantiate(elementPrefab, transform);
        element.transform.localScale = Vector3.one;
        element.gameObject.name = objectName;

        RectTransform elementTransform = element.GetComponent<RectTransform>();
        elementTransform.localPosition = new Vector3(xPosition, Lowest - elementTransform.sizeDelta.y / 2);

        Lowest = Fitter.GetLowest();

        Separator.transform.localPosition = new Vector3(Separator.transform.localPosition.x, Lowest - SeparatorGap);
        Fitter.GenerateShape();
    }
}
