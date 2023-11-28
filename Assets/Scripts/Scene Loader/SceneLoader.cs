using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text.Json;
using MongoDB.Driver;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;

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

    // UI for selecting scene
    [SerializeField] private Transform ContentWindow;
    [SerializeField] private GameObject SceneButtonPrefab;
    [SerializeField] private GameObject LoadingUI;

    // The database connection must be created immediately
    private void Awake()
    {
        // TODO: diagnose issue that is preventing secrets from being found unless selected in editor
        Secret[] secrets = Resources.FindObjectsOfTypeAll<Secret>();
        // Debug.Log(secrets.Length);
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
        List<Scene> scenesFromDatabase = await GetAllSimulationsAsync();

        // Some garbage collection to remove scenes with no name from dropdown
        foreach (Scene s in scenesFromDatabase)
        {
            if (!string.IsNullOrWhiteSpace(s.Name))
            {
                // Debug.Log(s.Name);
                Scenes.Add(s);
            }
        }

        // creating the buttons in the UI
        foreach (Scene s in Scenes)
        {
            GameObject newButtonObject = Instantiate(SceneButtonPrefab, ContentWindow);
            newButtonObject.GetComponent<Button>().onClick.AddListener(() => 
            {
                SelectScene(s);
                LoadingUI.SetActive(false);
            });
            newButtonObject.GetComponentInChildren<TMP_Text>().text = s.Name;
        }
    }

    public async Task<List<Scene>> GetAllSimulationsAsync()
    {
        var filter = Builders<Scene>.Filter.Empty;
        return await sceneCollection.Find(filter).ToListAsync();
    }

    public void SelectScene(Scene s)
    {
        // TODO: load objects 
        Debug.Log($"<color=blue>LOADER:</color> Attempting to load scene {s.Name}, has {s.Objects.Count} objects with a total of {s.Interactions.Count} interactions.");
    }
}
