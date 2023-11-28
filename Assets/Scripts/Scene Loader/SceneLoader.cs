using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text.Json;
using MongoDB.Driver;
using System.Threading.Tasks;

public class SceneLoader : MonoBehaviour
{
    [TextArea]
    private string jsonScene;
    private GameObject TablePrefabA, TablePrefabB;

    private float MaxPositionDimension = 175, RealMaxPosition;

    // MongoDB (relies on Secrets)
    private string ConnectionString;
    private static IMongoCollection<Scene> sceneCollection;

    private void Start()
    {
        Secret[] secrets = Resources.FindObjectsOfTypeAll<Secret>();
        Secret secret = Array.Find<Secret>(secrets, (Secret s) => s.name == "DatabaseConnection");
        Debug.Log(secret.Content);

        // Create MongoDB client
        var client = new MongoClient(ConnectionString);
        var database = client.GetDatabase("simulation");
        sceneCollection = database.GetCollection<Scene>("scene");
    }

    public async Task<List<Scene>> GetAllSimulationsAsync()
    {
        var filter = Builders<Scene>.Filter.Empty;
        return await sceneCollection.Find(filter).ToListAsync();
    }
}
