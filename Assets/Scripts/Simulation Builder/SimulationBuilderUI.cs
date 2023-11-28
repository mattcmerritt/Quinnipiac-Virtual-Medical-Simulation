using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;
using System.Threading.Tasks;

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
    [SerializeField] private List<string> InteractionIds;
    [SerializeField] private string SimulationName;

    //MongoDB
    //DO NOT LEAVE THIS IN PRODUCTION
    private string ConnectionString = "mongodb+srv://admin:capybara@cluster0.4pk1iio.mongodb.net/?retryWrites=true&w=majority";
    private static IMongoCollection<Scene> sceneCollection;
    

    private void Start()
    {
        ObjectEntries = new List<ObjectEntry>();
        ObjectIds = new List<string>();
        InteractionEntries = new List<InteractionEntry>();
        InteractionIds = new List<string>();

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
        DraggableObjectVisual draggableVisual = objectVisual.GetComponent<DraggableObjectVisual>();
        Image visualImage = objectVisual.GetComponent<Image>();

        draggableVisual.SetLinkedObjectEntry(objectEntryScript);

        objectEntryScript.SetId(ObjectEntries.Count);
        objectEntryScript.AttachObjectVisual(visualTransform, visualImage);

        ObjectIds.Add($"{ObjectEntries.Count}");
        ObjectEntries.Add(objectEntryScript);

        // updating all of the options for the interactions
        foreach (InteractionEntry interaction in InteractionEntries)
        {
            interaction.SetObjectOptions(ObjectIds);
        }

        // Relayering objects around new object
        ReorderObjectVisualsByHeight();

        UpdateListSizes();
    }

    public void ReorderObjectVisualsByHeight()
    {
        // TODO: potentially revisit this to find a better way to compare
        ObjectEntries.Sort((ObjectEntry entry1, ObjectEntry entry2) => Mathf.RoundToInt((entry1.GetHeight() - entry2.GetHeight()) * 100));
        foreach (ObjectEntry entry in ObjectEntries)
        {
            entry.GetVisualObject().transform.SetAsLastSibling();
        }
    }

    public void AddEvent()
    {
        GameObject interactionEntry = Instantiate(EventEntryPrefab, EventScrollWindow);

        InteractionEntry interactionEntryScript = interactionEntry.GetComponent<InteractionEntry>();

        interactionEntryScript.SetId(InteractionEntries.Count);
        interactionEntryScript.SetObjectOptions(ObjectIds);

        InteractionIds.Add($"{InteractionEntries.Count}");
        InteractionEntries.Add(interactionEntryScript);

        // updating the list of interactions for prerequisite selections
        foreach (InteractionEntry interaction in InteractionEntries)
        {
            interaction.SetInteractionOptions(InteractionIds);
        }

        UpdateListSizes();
    }

    public void UpdateListSizes()
    {
        VerticalLayoutGroup objectGroup = ObjectScrollWindow.GetComponent<VerticalLayoutGroup>();
        VerticalLayoutGroup interactionGroup = EventScrollWindow.GetComponent<VerticalLayoutGroup>();

        // toggling this reorders the UI
        objectGroup.childScaleHeight = false;
        objectGroup.childScaleHeight = true;

        // toggling this reorders the UI
        interactionGroup.childScaleHeight = false;
        interactionGroup.childScaleHeight = true;
    }

    public void ChangeSimulationName(string newName)
    {
        SimulationName = newName;
    }

    public void GenerateSimulationSceneData()
    {
        List<ObjectData> oData = new List<ObjectData>();
        for (int i = 0; i < ObjectEntries.Count; i++)
        {   
            ObjectEntry objectEntry = ObjectEntries[i];
            oData.Add(new ObjectData(objectEntry.GetId(), objectEntry.GetObjectType(), objectEntry.GetPosition().x, objectEntry.GetPosition().y, objectEntry.GetHeight()));
        }
        List<InteractionData> iData = new List<InteractionData>();
        for (int i = 0; i < InteractionEntries.Count; i++)
        {
            InteractionEntry interactionEntry = InteractionEntries[i];
            iData.Add(new InteractionData(System.Int32.Parse(interactionEntry.GetObjectId()), interactionEntry.GetInteractionType(), interactionEntry.GetPrerequisiteList(), interactionEntry.GetAccuracyPenalty(), interactionEntry.GetDuration()));
        }
        Scene newScene = new()
        {
            Name = SimulationName,
            Objects = oData,
            Interactions = iData
        };
        sceneCollection.InsertOne(newScene);
    }
    public async Task<List<Scene>> GetAllSimulationsAsync()
    {
        var filter = Builders<Scene>.Filter.Empty;
        return await sceneCollection.Find(filter).ToListAsync();
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

    public float height {  get; set; }
    public ObjectData(int id, string type, float x, float y, float height)
    {
        this.object_id = id;
        this.type = type;
        this.x = x;
        this.y = y;
        this.height = height;
    }
}

public class InteractionData
{
    public int object_id { get; set; }
    public string interaction_type { get; set; }

    [BsonElement("prerequisites")]
    public List<int> prereqlist { get; set; }

    public float accuracy_penalty { get; set; }

    public float duration_required { get; set; }

    public InteractionData(int object_id, string interaction_type, List<int> prereqlist, float accuracy_penalty, float duration_required)
    {
        this.object_id = object_id;
        this.interaction_type = interaction_type;
        this.prereqlist = prereqlist;
        this.accuracy_penalty = accuracy_penalty;
        this.duration_required = duration_required;
    }
}
