using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Runtime.CompilerServices;
using MongoDB.Bson.Serialization.Attributes;

public class ExitDoor : MonoBehaviour
{
    //DO NOT LEAVE THIS IN PRODUCTION
    private int exampleID = 0;
    private string ConnectionString = "mongodb+srv://admin:capybara@cluster0.4pk1iio.mongodb.net/?retryWrites=true&w=majority";
    private static IMongoCollection<StatData> statisticsCollection;

    public static event Action OnExit;

    private void OnEnable()
    {
        OnExit += PushAllStatisticsAtEnd;
    }

    private void OnDisable()
    {
        OnExit -= PushAllStatisticsAtEnd;
    }

    public void ExitScene()
    {
        OnExit?.Invoke();
    }

    public void PushAllStatisticsAtEnd()
    {
        Debug.Log("Simulation ended.");
        // TODO: mark all running Trackables as completed so they are gathered in the statistics at the end

        List<Statistic> statistics = StatisticManager.Instance.GetStatistics();

        if (statistics.Count != 0)
        {
            //Create MongoDB client
            var client = new MongoClient(ConnectionString);
            var database = client.GetDatabase("simulation");
            statisticsCollection = database.GetCollection<StatData>("statistics");
            StatData newStats = new StatData(exampleID, statistics);
            statisticsCollection.InsertOne(newStats);

        }
    }
}

public class StatData
{
    public int UserID { get; set; }

    [BsonElement("Statistics")]
    public List<Statistic> stats { get; set; }

    public StatData(int userID, List<Statistic> stats)
    {
        this.UserID = userID;
        this.stats = stats;
    }

}