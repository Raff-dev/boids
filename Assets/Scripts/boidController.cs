using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boidController : MonoBehaviour
{
    public boidManager manager;
    private string boidTag = "boid";
    private List<GameObject> nearbyBoids = new List<GameObject>();
    private bool getNearby = true;

    float startFi = 0 * Mathf.Deg2Rad;
    float endFi = 180 * Mathf.Deg2Rad;
    float startTheta = -90f * Mathf.Deg2Rad;
    float endTheta = 90f * Mathf.Deg2Rad;

    void Update()
    {
        if (manager == null) return;
        transform.Translate(Vector3.forward * manager.velocity * Time.deltaTime);

        List<RaycastHit> obstacleHits = getObstacles();


        nearbyBoids = getNearbyBoids(manager.boids);
        getBehaviour(nearbyBoids,
            out float[] rotation,
            out Vector3 avgPosition,
            out Vector3 separationVector,
            out int nearbyCount,
            out int tooCloseCount);
        stayInBoundaries();
        avoidObstacles(obstacleHits);
        staySeparated(separationVector, tooCloseCount);
        stayAligned(rotation, nearbyCount);
        stayCohesed(avgPosition, nearbyCount);
    }

    private void getBehaviour(
        List<GameObject> nearbyBoids,
        out float[] rotation,
        out Vector3 avgPosition,
        out Vector3 separationVector,
        out int nearbyCount,
        out int tooCloseCount)
    {
        rotation = new float[] { 0, 0, 0 };
        avgPosition = new Vector3(0, 0, 0);
        separationVector = new Vector3(0, 0, 0);
        nearbyCount = 0;
        tooCloseCount = 0;

        foreach (GameObject boid in nearbyBoids)
        {
            // ------draw connection to visible boids------
            // Debug.DrawLine(transform.position, boid.transform.position, Color.red);
            nearbyCount++;
            avgPosition += boid.transform.position;
            rotation[0] += boid.transform.rotation.eulerAngles.x;
            rotation[1] += boid.transform.rotation.eulerAngles.y;
            rotation[2] += boid.transform.rotation.eulerAngles.z;
            if (Vector3.Distance(transform.position, boid.transform.position) < manager.separationDistance)
            {
                tooCloseCount++;
                separationVector += boid.transform.position - transform.position;
            }
        }
    }

    private void avoidObstacles(List<RaycastHit> hits)
    {
        foreach (RaycastHit hit in hits)
        {

        }
    }

    private void stayAligned(float[] rotation, int nearbyCount)
    {
        if (manager.alignment)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                 Quaternion.Euler(
                    rotation[0] / nearbyCount,
                    rotation[1] / nearbyCount,
                    rotation[2] / nearbyCount),
                manager.turningRate * 1.2f * Time.deltaTime);
        }
    }

    private void stayCohesed(Vector3 avgPosition, int nearbyCount)
    {
        if (manager.cohesion && nearbyCount > 0)
        {
            // ------draw average position------
            // Debug.DrawLine(avgPosition, avgPosition + Vector3.forward, Color.green);
            avgPosition /= nearbyCount;
            var c = Quaternion.LookRotation(avgPosition - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, c,
                manager.turningRate * Time.deltaTime);
        }
    }

    private void staySeparated(Vector3 separationVector, int tooCloseCount)
    {
        if (manager.separation && tooCloseCount > 0)
        {
            Vector3 vector = transform.position - separationVector;
            Debug.DrawLine(transform.position, vector, Color.magenta);
            var s = Quaternion.LookRotation(vector);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, s,
                manager.turningRate * 1.2f * Time.deltaTime);
        }
    }

    private List<GameObject> getNearbyBoids(List<GameObject> boids)
    {
        List<GameObject> nearbyBoids = new List<GameObject>();
        foreach (GameObject boid in boids)
        {
            bool isNearby = Vector3.Distance(transform.position, boid.transform.position) < manager.detectionRadius;
            if (isNearby && boid != gameObject) nearbyBoids.Add(boid);
        }
        return nearbyBoids;
    }

    private List<RaycastHit> getObstacles()
    {
        List<RaycastHit> obstacleHits = new List<RaycastHit>();
        if (manager.raycastsCount <= 0) return null;

        int count = manager.raycastsCount;
        float radius = manager.detectionRadius;
        float step = 360f / count * Mathf.Deg2Rad;

        for (float fi = startFi; fi <= endFi; fi += step)
        {
            for (float theta = startTheta; theta <= endTheta; theta += step)
            {
                // calculate destination point coordinates
                float x = radius * Mathf.Cos(theta) * Mathf.Cos(fi);
                float z = radius * Mathf.Cos(theta) * Mathf.Sin(fi);
                float y = radius * Mathf.Sin(theta);
                // rotate calculated point to match boid's rotation
                Vector3 vector = Quaternion.Euler(
                    transform.rotation.eulerAngles.x,
                    transform.rotation.eulerAngles.y,
                    transform.rotation.eulerAngles.z) * new Vector3(x, y, z);

                //  ------Draw Raycasts------
                //  Debug.DrawRay(transform.position, vector, Color.red);

                RaycastHit hit;
                if (Physics.Raycast(transform.position, vector, out hit, radius))
                {
                    //  ------Draw hit------
                    // Debug.DrawRay(transform.position, vector, Color.green);
                    obstacleHits.Add(hit);
                    Debug.DrawLine(transform.position, hit.transform.position, Color.red);
                }
            }
        }
        return obstacleHits;
    }

    void stayInBoundaries()
    {
        bool[] outOfBoundary = new bool[3];
        float[] coords = new float[] {
            transform.position.x,
            transform.position.y,
            transform.position.z};

        for (int i = 0; i < coords.Length; i++)
        {
            outOfBoundary[i] = coords[i] > manager.aquariumSize || coords[i] < -manager.aquariumSize;
            if (coords[i] != 0)
            {
                int sign = (int)(coords[i] / Mathf.Abs(coords[i]));
                float inBoundaries = Mathf.Min(Mathf.Abs(coords[i]), manager.aquariumSize);
                coords[i] = inBoundaries * sign;
            }
        }
        if (outOfBoundary[0] || outOfBoundary[1] || outOfBoundary[2])
            transform.position = new Vector3(-coords[0], -coords[1], -coords[2]);
    }

}
