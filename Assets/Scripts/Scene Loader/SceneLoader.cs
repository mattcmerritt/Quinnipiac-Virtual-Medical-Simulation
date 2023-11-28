using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text.Json;
using MongoDB.Driver;
using System.Threading.Tasks;
using TMPro;

public class SceneLoader : MonoBehaviour
{
    [TextArea]
    private string jsonScene;
    private GameObject TablePrefabA, TablePrefabB;

    private float MaxPositionDimension = 175, RealMaxPosition;

    // MongoDB (relies on Secrets)
    private string ConnectionString;
    private static IMongoCollection<Scene> sceneCollection;

    // Scene data from database
    private List<Scene> Scenes;
    private List<string> SceneNames;

    [SerializeField] private TMP_Dropdown SceneSelector;

    // The database connection must be created immediately
    private void Awake()
    {
        // TODO: diagnose issue that is preventing secrets from being found unless selected in editor
        Secret[] secrets = Resources.FindObjectsOfTypeAll<Secret>();
        Debug.Log(secrets.Length);
        Secret secret = Array.Find<Secret>(secrets, (Secret s) => s.name == "DatabaseConnection");
        ConnectionString = secret.Content;

        // Create MongoDB client
        var client = new MongoClient(ConnectionString);
        var database = client.GetDatabase("simulation");
        sceneCollection = database.GetCollection<Scene>("scene");
    }

    private async void Start()
    {
        // Create scene collection
        Scenes = new List<Scene>();
        SceneNames = new List<string>();
        List<Scene> scenesFromDatabase = await GetAllSimulationsAsync();

        // Some garbage collection to remove scenes with no name from dropdown
        foreach (Scene s in scenesFromDatabase)
        {
            if (!string.IsNullOrWhiteSpace(s.Name))
            {
                Debug.Log(s.Name);
                Scenes.Add(s);
                SceneNames.Add(s.Name);
            }
        }

        // Feeding the names back to the dropdown
        SceneSelector.ClearOptions();
        SceneSelector.AddOptions(SceneNames);
        // attach listener
        SceneSelector.onValueChanged.AddListener(SelectSceneFromDropdown);
    }

    public async Task<List<Scene>> GetAllSimulationsAsync()
    {
        var filter = Builders<Scene>.Filter.Empty;
        return await sceneCollection.Find(filter).ToListAsync();
    }

    public void SelectSceneFromDropdown(int index)
    {
        Scene selectedScene = Scenes[index];
        // TODO: load objects
        Debug.Log($"<color=blue>LOADER:</color> Attempting to load scene {selectedScene.Name}, has {selectedScene.Objects.Count} objects with a total of {selectedScene.Interactions.Count} interactions.");
    }
}
