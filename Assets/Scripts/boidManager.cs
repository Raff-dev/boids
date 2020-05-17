using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boidManager : MonoBehaviour
{
    public GameObject attractionPoint;
    public bool alignment = true;
    public bool separation = true;
    public bool cohesion = true;
    public bool getAttracted = false;
    public float separationDistance = 1f;

    public int aquariumSize = 40;
    public int boidsCount = 50;
    public float velocity = 8f;

    public float boidDetectionRadius = 8f;
    public float obstacleDetectionRadius = 8f;
    public float turningRate = 30f;
    public int raycastsCount = 20;

    public GameObject boidPrefab;
    public List<GameObject> boids { get; set; }

    void Start()
    {
        boids = generateBoids();
        attractionPoint.GetComponent<Renderer>().material.color = Color.red;
    }

    private List<GameObject> generateBoids()
    {
        List<GameObject> arr = new List<GameObject>();
        for (int i = 0; i < boidsCount; i++)
        {
            float randX = Random.Range(-aquariumSize * 0.9f, aquariumSize * 0.9f);
            float randY =
            // 0;
            Random.Range(-aquariumSize * 0.9f, aquariumSize * 0.9f);
            float randZ = Random.Range(-aquariumSize * 0.9f, aquariumSize * 0.9f);
            Vector3 spawnPlace = new Vector3(randX, randY, randZ);
            Quaternion rotation = Quaternion.Euler(
                 Random.Range(0, 360f),
                // 0,
                Random.Range(0, 360f),
                // Random.Range(0, 360f)
                90
                );
            GameObject boid = Instantiate(boidPrefab, spawnPlace, rotation);
            boid.GetComponent<boidController>().manager = this;
            arr.Add(boid);
        }
        return arr;
    }
}
