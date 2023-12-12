using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver;
using System;

namespace TestBSON
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator(Required=true, RootClass=true)]
    public class TestBSONWriter : MonoBehaviour
    {
        [SerializeField, BsonElement("text")] private string Text;

        [BsonIgnore]
        private static IMongoCollection<TestBSONWriter> testCollection;

        private void Awake()
        {
            // TODO: diagnose issue that is preventing secrets from being found unless selected in editor
            Secret[] secrets = Resources.FindObjectsOfTypeAll<Secret>();
            Secret secret = Array.Find<Secret>(secrets, (Secret s) => s.name == "DatabaseConnection");
            string connectionString = secret.Content;

            // Create MongoDB client
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("simulation");
            testCollection = database.GetCollection<TestBSONWriter>("test");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                testCollection.InsertOne(this);
            }
        }
    }
}
