using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text.Json;
using MongoDB.Driver;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using MongoDB.Bson.Serialization.IdGenerators;

public class SceneLoader : MonoBehaviour
{
    // Room data and basics
    [SerializeField] private List<GameObject> RoomElementPrefabs;
    [SerializeField] private float MaxPositionDimension = 175, RealMaxPosition;

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

    // Query to retrieve all scenes on the database
    public async Task<List<Scene>> GetAllSimulationsAsync()
    {
        var filter = Builders<Scene>.Filter.Empty;
        return await sceneCollection.Find(filter).ToListAsync();
    }

    public void SelectScene(Scene s)
    {
        Debug.Log($"<color=blue>LOADER:</color> Attempting to load scene {s.Name}, has {s.Objects.Count} objects with a total of {s.Interactions.Count} interactions.");

        List<RoomObject> roomObjects = new List<RoomObject>();
        foreach (ObjectData obj in s.Objects)
        {
            // picking the proper prefab
            GameObject selectedPrefab = RoomElementPrefabs.Find((GameObject prefab) =>
            {
                RoomObject roomObject = prefab.GetComponent<RoomObject>();
                return roomObject.ObjectName == obj.type;
            });

            // creating the position vector using the positional data and the bounds of the room
            // TODO: implement in a way that better supports different room shapes and sizes
            Vector3 position = new Vector3(obj.x / MaxPositionDimension * RealMaxPosition, selectedPrefab.transform.localScale.y / 2f, obj.y / MaxPositionDimension * RealMaxPosition);

            // creating and tagging the object
            GameObject newRoomElement = Instantiate(selectedPrefab, position, Quaternion.identity);
            RoomObject roomObjectScript = newRoomElement.GetComponent<RoomObject>();
            roomObjectScript.ObjectId = obj.object_id;

            roomObjects.Add(roomObjectScript);
        }

        // adding the interactions to objects
        List<Trackable> interactions = new List<Trackable>();
        foreach (InteractionData inter in s.Interactions)
        {
            // Finding the associated room object
            // TODO: for some reason, all object ids here are 0
            RoomObject desiredRoomObject = roomObjects.Find((RoomObject ro) =>
            {
                return ro.ObjectId == inter.object_id;
            });
            GameObject roomObject = desiredRoomObject.gameObject;

            // TODO: finish implementing / find new way to do this
            // TODO: need to add some way to track interaction id (NOT CURRENTLY IN THE BSON)
            if (inter.interaction_type == "Proximity Task")
            {
                ProximityInteraction proxInter = roomObject.AddComponent<ProximityInteraction>();
                proxInter.SetDurationAccuracy(inter.duration_required, inter.accuracy_penalty);
                interactions.Add(proxInter);
            }
            else if (inter.interaction_type == "Touch Task")
            {
                ClickInteraction clickInter = roomObject.AddComponent<ClickInteraction>();
                interactions.Add(clickInter);
            }
            else if (inter.interaction_type == "Transport Task")
            {
                Debug.Log("<color=red>LOADER:</color> The transport task is not currently supported in the simulation loader!");
            }
        }

        // listing the prerequisites for each interaction
        foreach (InteractionData inter in s.Interactions)
        {
            // DEBUG
            Debug.Log($"{inter.interaction_type}: on {inter.object_id}");

            // Finding the associated room object
            RoomObject desiredRoomObject = roomObjects.Find((RoomObject ro) =>
            {
                return ro.ObjectId == inter.object_id;
            });
            GameObject roomObject = desiredRoomObject.gameObject;

            // Finding the interactions listed as prereqs
            foreach (Trackable t in interactions)
            {
                // TODO: match prereq ids to trackables and build a list
            }

            // TODO: revisit when prerequisites (hopefully with polymorphism at the trackable level)
            if (inter.interaction_type == "Proximity Task")
            {
                ProximityInteraction proxInter = roomObject.GetComponent<ProximityInteraction>();
                Debug.Log("<color=red>LOADER:</color> The proximity task is not currently supported for prerequisites in the simulation loader!");
            }
            else if (inter.interaction_type == "Touch Task")
            {
                Debug.Log("<color=red>LOADER:</color> The touch task is not currently supported for prerequisites in the simulation loader!");
            }
            else if (inter.interaction_type == "Transport Task")
            {
                Debug.Log("<color=red>LOADER:</color> The transport task is not currently supported for prerequisites the simulation loader!");
            }
        }
    }
}
