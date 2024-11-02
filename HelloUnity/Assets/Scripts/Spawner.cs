using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public GameObject templateObject;
    public float spawnRange = 10f;
    public int maxSpawns = 5;

    private List<GameObject> spawnedObjects = new List<GameObject>();

    void Start()
    {
        // at the beginning, spawn maxSpawns number of collectibles
        for (int i = 0; i < maxSpawns; i++)
        {
            SpawnNewObject();
        }
    }

    void Update()
    {
        // check for inactive objects and respawn
        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            if (spawnedObjects[i] != null && !spawnedObjects[i].activeInHierarchy)
            {
                // reposition and reactivate
                spawnedObjects[i].transform.position = GenerateRandomPosition();
                spawnedObjects[i].SetActive(true);
                break;
            }
        }
    }

    void SpawnNewObject()
    {
        Vector3 spawnPosition = GenerateRandomPosition();
        GameObject newObject = Instantiate(templateObject, spawnPosition, Quaternion.identity);
        newObject.GetComponent<Collider>().enabled = true;
        spawnedObjects.Add(newObject);
    }

    Vector3 GenerateRandomPosition()
    {
        float x = Random.Range(-spawnRange, spawnRange);
        float z = Random.Range(-spawnRange, spawnRange);
        Vector3 randomPosition = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);

        // make sure it is above the terrain
        if (Physics.Raycast(randomPosition + Vector3.up * 10, Vector3.down, out RaycastHit hit))
        {
            randomPosition.y = hit.point.y + templateObject.GetComponent<Collider>().bounds.extents.y;
        }

        return randomPosition;
    }
}
