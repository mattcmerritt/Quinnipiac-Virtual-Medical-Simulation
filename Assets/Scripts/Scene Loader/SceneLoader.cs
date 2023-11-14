using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.Json;

public class SceneLoader : MonoBehaviour
{
    [TextArea]
    private string jsonScene;
    private GameObject TablePrefabA, TablePrefabB;

    private float MaxPositionDimension = 175, RealMaxPosition;

    private void Start()
    {
        // TODO: Can only handle simple objects
        // JsonUtility.FromJson<PlayerPrefs>(jsonScene);
    }
}
