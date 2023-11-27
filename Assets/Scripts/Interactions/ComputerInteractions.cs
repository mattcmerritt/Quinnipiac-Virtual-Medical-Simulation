using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ComputerInteractions : MonoBehaviour
{
    [SerializeField] private GameObject ItemSpawner;
    
    public void SpawnItemPrefab(GameObject prefab)
    {
        GameObject newObj = Instantiate(prefab, ItemSpawner.transform.position, Quaternion.identity);
    }
}
