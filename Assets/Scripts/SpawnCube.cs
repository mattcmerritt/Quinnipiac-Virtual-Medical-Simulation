using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCube : MonoBehaviour
{
    [SerializeField] private Vector3 SpawnPoint;
    [SerializeField] private GameObject Cube;

    public void SpawnCubeInScene()
    {
        Instantiate(Cube, SpawnPoint, Quaternion.identity);
    }
}
