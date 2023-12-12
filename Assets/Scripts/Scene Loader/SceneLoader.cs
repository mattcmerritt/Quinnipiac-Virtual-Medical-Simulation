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
using UnityEngine.XR.Interaction.Toolkit;

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

    // Important prefabs needed for interactions
    [SerializeField] public GameObject InteractionUIPrefab;
    [SerializeField] public GameObject InteractionUIButtonPrefab;

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
                proxInter.SetInteractionId(inter.interaction_id);
                interactions.Add(proxInter);
            }
            else if (inter.interaction_type == "Touch Task")
            {
                ClickInteraction clickInter = roomObject.AddComponent<ClickInteraction>();
                clickInter.SetInteractionId(inter.interaction_id);
                interactions.Add(clickInter);
            }
            else if (inter.interaction_type == "Transport Task")
            {
                Debug.Log("<color=red>LOADER:</color> The transport task is not currently supported in the simulation loader!");
            }
            else if (inter.interaction_type == "Audio Task")
            {
                // attach outline script to object
                Outline outlineScript = roomObject.AddComponent<Outline>();
                outlineScript.OutlineColor = new Color(255, 252, 102);
                outlineScript.OutlineWidth = 7;
                outlineScript.enabled = false;

                // Toggle script necessary for supporting interaction
                ToggleObject toggleObj = roomObject.AddComponent<ToggleObject>();
                toggleObj.DistanceToDisable = 10;

                RampingAudioInteraction audioInter = roomObject.AddComponent<RampingAudioInteraction>();
                audioInter.AddUIPrefabs(InteractionUIPrefab, InteractionUIButtonPrefab);
                audioInter.AddDetails(inter.volume_increase_time_interval, inter.volume_increase_magnitude, inter.acceptable_volume_threshold, inter.loop, inter.initial_volume, inter.selected_audio_soure);
                audioInter.SetInteractionId(inter.interaction_id);
                interactions.Add(audioInter);

                // handle callbacks for XRSimpleInteractable
                XRSimpleInteractable XRSimpleInteractableComponent = roomObject.GetComponent<XRSimpleInteractable>();
                XRSimpleInteractableComponent.hoverEntered.AddListener((HoverEnterEventArgs) => {
                    outlineScript.enabled = true;
                });
                XRSimpleInteractableComponent.hoverExited.AddListener((HoverExitEventArgs) => {
                    outlineScript.enabled = false;
                });
                XRSimpleInteractableComponent.selectEntered.AddListener((SelectEnterEventArgs) => {
                    toggleObj.ToggleObjectEnabled();
                });
            }
            else if (inter.interaction_type == "Display Text Task")
            {
                // attach outline script to object
                Outline outlineScript = roomObject.AddComponent<Outline>();
                outlineScript.OutlineColor = new Color(255, 252, 102);
                outlineScript.OutlineWidth = 7;
                outlineScript.enabled = false;

                // Toggle script necessary for supporting interaction
                ToggleObject toggleObj = roomObject.AddComponent<ToggleObject>();
                toggleObj.DistanceToDisable = 10;

                DisplayTextInteraction textInter = roomObject.AddComponent<DisplayTextInteraction>();
                textInter.AddUIPrefabs(InteractionUIPrefab, InteractionUIButtonPrefab);
                textInter.AddDetails(inter.text_to_display, inter.text_initially_active);
                textInter.SetInteractionId(inter.interaction_id);
                interactions.Add(textInter);

                // handle callbacks for XRSimpleInteractable
                XRSimpleInteractable XRSimpleInteractableComponent = roomObject.GetComponent<XRSimpleInteractable>();
                XRSimpleInteractableComponent.hoverEntered.AddListener((HoverEnterEventArgs) => {
                    outlineScript.enabled = true;
                });
                XRSimpleInteractableComponent.hoverExited.AddListener((HoverExitEventArgs) => {
                    outlineScript.enabled = false;
                });
                XRSimpleInteractableComponent.selectEntered.AddListener((SelectEnterEventArgs) => {
                    toggleObj.ToggleObjectEnabled();
                });
            }
        }

        // TODO: remove with better loading functions at a later date
        for (int i = 0; i < interactions.Count; i++)
        {
            interactions[i].SetTaskName("Task " + i);
        }

        // iterate through the interactions
        foreach (InteractionData inter in s.Interactions)
        {
            // Finding the associated room object
            RoomObject desiredRoomObject = roomObjects.Find((RoomObject ro) =>
            {
                return ro.ObjectId == inter.object_id;
            });
            GameObject roomObject = desiredRoomObject.gameObject;

            // Finding the currently selected interaction
            Trackable interactionScript = interactions.Find((Trackable t) =>
            {
                return t.GetInteractionId() == inter.interaction_id;
            });

            // step through all the prereqs and assign them to the trackable
            foreach (int prereqInterId in inter.prereqlist)
            {
                // find the associated id
                Trackable prereqInteraction = interactions.Find((Trackable t) =>
                {
                    return t.GetInteractionId() == prereqInterId;
                });

                // TODO: need to add a way to load in the numbers for duration, accuracy required, and accuracy penalty
                Prerequisite newPrereq = new Prerequisite(prereqInteraction, 1, 0, 0.25f);

                // attach the prerequisite interaction to the interaction
                interactionScript.AddPrerequisite(newPrereq);
            }
        }
    }
}
