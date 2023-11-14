using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;

public class SimulationBuilderUI : MonoBehaviour
{
    // UI Components and Elements
    [SerializeField] private Button ObjectTabButton, EventTabButton;
    [SerializeField] private GameObject ObjectPanel, EventPanel;
    [SerializeField] private Transform ObjectScrollWindow, EventScrollWindow;

    // Prefabs to instantiate entries in the lists
    [SerializeField] private GameObject ObjectEntryPrefab, EventEntryPrefab;

    // Data for the map visualization
    [SerializeField] private Transform Map;
    [SerializeField] private GameObject ObjectPrefab;

    // Added data
    [SerializeField] private List<ObjectEntry> ObjectEntries;
    [SerializeField] private List<string> ObjectIds;
    [SerializeField] private List<InteractionEntry> InteractionEntries;

    //MongoDB
    //DO NOT LEAVE THIS IN PRODUCTION
    private string ConnectionString = "mongodb+srv://admin:capybara@cluster0.4pk1iio.mongodb.net/?retryWrites=true&w=majority";
    private static IMongoCollection<Scene> sceneCollection;
    

    private void Start()
    {
        ObjectEntries = new List<ObjectEntry>();
        ObjectIds = new List<string>();
        InteractionEntries = new List<InteractionEntry>();

        //Create MongoDB client
        var client = new MongoClient(ConnectionString);
        var database = client.GetDatabase("simulation");
        sceneCollection = database.GetCollection<Scene>("scene");
        
    }

    public void SwitchToObjectsTab()
    {
        ObjectTabButton.interactable = false;
        EventTabButton.interactable = true;

        ObjectPanel.SetActive(true);
        EventPanel.SetActive(false);
    }

    public void SwitchToEventsTab()
    {
        ObjectTabButton.interactable = true;
        EventTabButton.interactable = false;

        ObjectPanel.SetActive(false);
        EventPanel.SetActive(true);
    }

    public void AddObject()
    {
        GameObject objectEntry = Instantiate(ObjectEntryPrefab, ObjectScrollWindow);
        GameObject objectVisual = Instantiate(ObjectPrefab, Map);

        ObjectEntry objectEntryScript = objectEntry.GetComponent<ObjectEntry>();
        RectTransform visualTransform = objectVisual.GetComponent<RectTransform>();

        objectEntryScript.SetId(ObjectEntries.Count);
        objectEntryScript.AttachObjectVisual(visualTransform);

        ObjectIds.Add($"{ObjectEntries.Count}");
        ObjectEntries.Add(objectEntryScript);

        // updating all of the options for the interactions
        foreach (InteractionEntry interaction in InteractionEntries)
        {
            interaction.SetObjectOptions(ObjectIds);
        }
    }

    public void AddEvent()
    {
        GameObject interactionEntry = Instantiate(EventEntryPrefab, EventScrollWindow);

        InteractionEntry interactionEntryScript = interactionEntry.GetComponent<InteractionEntry>();

        interactionEntryScript.SetObjectOptions(ObjectIds);

        InteractionEntries.Add(interactionEntryScript);
    }

    public void GenerateSimulationSceneData()
    {
        List<ObjectData> oData = new List<ObjectData>();
        for (int i = 0; i < ObjectEntries.Count; i++)
        {   
            ObjectEntry objectEntry = ObjectEntries[i];
            oData.Add(new ObjectData(objectEntry.GetId(), objectEntry.GetObjectType(), objectEntry.GetPosition().x, objectEntry.GetPosition().y));
        }
        List<InteractionData> iData = new List<InteractionData>();
        for (int i = 0; i < InteractionEntries.Count; i++)
        {
            InteractionEntry interactionEntry = InteractionEntries[i];
            iData.Add(new InteractionData(System.Int32.Parse(interactionEntry.GetObjectId()), interactionEntry.GetInteractionType()));
        }
        Scene newScene = new()
        {
            Name = "Trauma Simulation",
            Objects = oData,
            Interactions = iData
        };
        sceneCollection.InsertOne(newScene);
    }
}

public class Scene
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }

    [BsonElement("objects")]
    public List<ObjectData> Objects { get; set; }
    [BsonElement("interactions")]
    public List<InteractionData> Interactions { get; set; }

}

public class ObjectData
{
    public int object_id { get; set; }
    public string type { get; set; }
    public float x { get; set; }
    public float y { get; set; }
    public ObjectData(int id, string type, float x, float y)
    {
        this.object_id = id;
        this.type = type;
        this.x = x;
        this.y = y;
    }
}

public class InteractionData
{
    public int object_id { get; set; }
    public string interaction_type { get; set; }

    public InteractionData(int object_id, string interaction_type)
    {
        this.object_id = object_id;
        this.interaction_type = interaction_type;
    }
}
