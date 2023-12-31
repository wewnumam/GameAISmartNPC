﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// Component that is responsible for spawning many GameObjects in a certain area. 
/// Each Gameobject instaciated will be assigned a behavior to move around the area randomly.
/// </summary>
public class MassSpawner : MonoBehaviour
{
    ///<value>The gameobject that will be respawned</value>
    public GameObject prefab;
    public GameObject player;
    ///<value>Area where the Gameobjects will move</value>
    public GameObject wanderArea;
    public GameObject enemyHQ;

    ///<value>Times that The GameObject spawn</value>
    public int Spawns = 500;
    int spawnCount = 0;
    List<GameObject> entities;

    public float spawnInterval;

    /// <summary>
    /// Initialize the entities to pass them to the behaviors
    /// </summary>
    void Start()
    {
        entities = new List<GameObject>(FindObjectsOfType(typeof(GameObject)) as GameObject[]);
        entities.RemoveAll(e => e.GetComponent<BehaviorExecutor>() == null);
        InvokeRepeating("Spawn", 0f, spawnInterval);
    }

    /// <summary>
    /// Method that instantiates Gameobject, adds the behavior Executor component and sets the behavior parameters.
    /// </summary>
    void Spawn()
    {
        if (spawnCount <= Spawns)
        {
            GameObject instance = Instantiate(prefab, GetRandomPosition(), Quaternion.identity) as GameObject;
            instance.name = $"{prefab.name}_{spawnCount}";

            BehaviorExecutor component;

            if (instance.TryGetComponent<BehaviorExecutor>(out component))
            {
                component.SetBehaviorParam("player", player);
                component.SetBehaviorParam("wanderArea", wanderArea);
                component.SetBehaviorParam("enemyHQ", enemyHQ);
            }
            

            ++spawnCount;

            entities.Add(instance);
        }
        else
        {
            CancelInvoke();
        }

    }

    private Vector3 GetRandomPosition()
    {
        Vector3 randomPosition = Vector3.zero;
        BoxCollider boxCollider = wanderArea.GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            randomPosition = new Vector3(Random.Range(wanderArea.transform.position.x - wanderArea.transform.localScale.x * boxCollider.size.x * 0.5f,
                                                                  wanderArea.transform.position.x + wanderArea.transform.localScale.x * boxCollider.size.x * 0.5f),
                                         wanderArea.transform.position.y,
                                         Random.Range(wanderArea.transform.position.z - wanderArea.transform.localScale.z * boxCollider.size.z * 0.5f,
                                                                  wanderArea.transform.position.z + wanderArea.transform.localScale.z * boxCollider.size.z * 0.5f));
        }

        return randomPosition;
    }
}
