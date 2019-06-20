﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupManager : MonoBehaviour
{
    public Camera camera;

    [SerializeField] private bool alignmentEnabled = false;
    [SerializeField] private bool separationEnabled = false;
    [SerializeField] private bool cohesionEnabled = false;
    [SerializeField] private bool seekEnabled = false;
    [SerializeField] private bool wanderEnabled = false;

    [Tooltip("The factor by which alignment will be scaled. Higher values emphasize this behavior relative to the others.")]
    [SerializeField] private float alignmentWeight = 1.0f;
    [Tooltip("The factor by which separation will be scaled. Higher values emphasize this behavior relative to the others.")]
    [SerializeField] private float separationWeight = 2.0f;
    [Tooltip("The factor by which cohesion will be scaled. Higher values emphasize this behavior relative to the others.")]
    [SerializeField] private float cohesionWeight = 1.5f;
    [Tooltip("The factor by which seeking will be scaled. Higher values emphasize this behavior relative to the others.")]
    [SerializeField] private float seekWeight = 1.0f;
    [Tooltip("The factor by which wandering will be scaled. Higher values emphasize this behavior relative to the others.")]
    [SerializeField] private float wanderWeight = 1.0f;
    
    // These two members are used by the Boid.cs script to access information about their neighbors
    public static int numBoidsToSpawn = 50;
    public static List<GameObject> boids;

    [SerializeField] private GameObject boidPrefab;

    // For spawning boids. Each boid will be given a random x between -minX and minX; same goes for z.
    private float minX = 200;
    private float minZ = 200;
    
    
    /* Sets up all necessary info for the script. Main role is to spawn boids.
     */ 
    void Awake()
    {
        boids = new List<GameObject>(numBoidsToSpawn);
        float boidRadius = boidPrefab.transform.localScale.x / 2;

        for(int i = 0; i < numBoidsToSpawn; i++)
        {
            // Note: boidRadius ensures that boids spawn at ground level, since they're spheres
            Vector3 spawnPoint = new Vector3(Random.Range(-minX, minX), boidRadius, Random.Range(-minZ, minZ));
            GameObject newBoid = Instantiate(boidPrefab, spawnPoint, Quaternion.identity);
            boids.Add(newBoid);
        }        
    }
   

    /* Called by the physics engine. Update loop.
     */ 
    void FixedUpdate()
    {
        foreach(GameObject vehicle in boids)
        {
            Boid boid = vehicle.GetComponent<Boid>();
            Rigidbody body = boid.GetComponent<Rigidbody>();

            if(alignmentEnabled)
            {
                body.AddForce(boid.Align() * alignmentWeight);
            }

            if(separationEnabled)
            {
                body.AddForce(boid.Separate() * separationWeight);
            }

            if(cohesionEnabled)
            {
                body.AddForce(boid.Cohere() * cohesionWeight);
            }

            if(seekEnabled)
            {
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                // If the camera is pointing somewhere on the floor
                if (Physics.Raycast(ray, out hit))
                {
                    body.AddForce(boid.Seek(hit.point) * seekWeight);
                }
            }

            if(wanderEnabled)
            {
                body.AddForce(boid.Wander() * wanderWeight);
            }
        }
    }
}
