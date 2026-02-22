using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreateScene : MonoBehaviour
{

    /* NOTES

    Plane is 10x10 with the center at 0, 0, 0 so work with
    -5 to 5 on X and Z axis and probably a little less so
    stuff doesnt go over the edge

    plane is now 25 by 25

    */

    #region VARIABLES

    [Header("Color Settings")]
    public Color groundColor;
    public Color treeTrunkColor;
    public Color leavesColor;

    [Header("Tree Settings - Tree Proximity (3 - 10)")]
    public int sizeOfForest;
    public float treeProximity;

    [Header("Danger Mode - More than 75 Trees")]
    public bool dangerMode;

    [Header("Pyramid Settings - Grid Size (3 - 10)")]
    public int baseGridSize;

    [Header("Celestial Object Settings")]
    public float rotationSpeed;
    public float dayNightDuration;
    public Color dayColor;
    public Color nightColor;
    [Space]
    [Space]
    public List<GameObject> trees;
    public List<GameObject> stones;

    public Light directionalLight;
    #endregion

    #region MONOBEHAVIOUR FUNCTIONS

    //----------------//
    private void Start()
    //----------------//
    {
        CheckVariables();
        CreateGround();
        CreateTrees(sizeOfForest);
        CreatePyramid();
        SpawnObject();
        directionalLight = FindAnyObjectByType<Light>();
        StartCoroutine(DayNightCycle());
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < trees.Count; i++)
        {
            Gizmos.DrawSphere(trees[i].transform.position, treeProximity);
        }
    }

    private void Update()
    {
        RotateObject();
    }

    #endregion

    #region CREATE SCENE

    void CheckVariables()
    {
        if (baseGridSize < 3 || baseGridSize > 10)
        {
            Debug.LogWarning("Pyramid Base Grid Size is outside of bounds (3 - 10) setting grid size to the default of 3...");
            baseGridSize = 3;
        }

        if (treeProximity < 3 || treeProximity > 10)
        {
            Debug.LogWarning("Tree Proximity Range is outside of bounds (3 - 10) setting proximity to the default of 3...");
            treeProximity = 3;
        }

        if (dangerMode == false)
        {
            if (sizeOfForest > 75)
            {
                Debug.LogWarning("Size of Forest is more than 100, setting size of forest to 50, enable Danger Mode for more than 75 trees");
                sizeOfForest = 50;
            }
        }

        if (dayNightDuration <= 0)
        {
            Debug.Log("Day and Night Duration is invalid, setting duration to a default of 5");
            dayNightDuration = 5;
        }
    }

    void CreateGround()
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.gameObject.name = "Ground";
        ground.GetComponent<MeshRenderer>().material.color = groundColor;
        ground.transform.position = Vector3.zero;
        ground.transform.localScale = new Vector3(5, 1, 5);
    }

    void CreateTrees(int sizeOfForest)
    {
        GameObject treeParentObject = new GameObject("Forest");
        treeParentObject.transform.position = Vector3.zero;
        for (int i = 0; i < sizeOfForest; i++)
        {
            GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.transform.localScale = new Vector3(.375f, .5f, .375f);
            trunk.GetComponent<MeshRenderer>().material.color = treeTrunkColor;
            trunk.layer = 3;

            GameObject leaves = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leaves.GetComponent<MeshRenderer>().material.color = leavesColor;
            leaves.layer = 3;

            leaves.transform.parent = trunk.transform;
            leaves.transform.localPosition = new Vector3(0, 2, 0);

            leaves.transform.localScale = new Vector3(2, 3, 2);

            trees.Add(trunk);

            trunk.transform.position = new Vector3(0, -20, 0);
            trunk.transform.localScale = new Vector3(Random.Range(.5f, 1.5f) * trunk.transform.localScale.x, Random.Range(.5f, 1.5f) * trunk.transform.localScale.y, Random.Range(.5f, 1.5f) * trunk.transform.localScale.z);

            bool clearProx = false;
            Vector3 trunkCoords = Vector3.zero;
            while (clearProx == false)
            {
                bool outsideProx = true;
                trunkCoords = ExcludeCenter();

                foreach (GameObject t in trees)
                {
                    float distance = Vector3.Distance(t.transform.position, trunkCoords);
                    if (distance <= treeProximity)
                    {
                        outsideProx = false;
                        break;
                    }
                }

                if (outsideProx == true)
                {
                    clearProx = true;
                    trunk.transform.position = trunkCoords;
                    trunk.transform.parent = treeParentObject.transform;
                    Debug.Log("clearProx = " + clearProx);
                }
            }
        }
    }

    void CreatePyramid()
    {
        GameObject pyramidParentObject = new GameObject("Pyramid");
        Vector3 pyramidCoords = new Vector3(0, 0.5f, 0);
        Vector3 zAxis = pyramidCoords;
        Vector3 layerStart = pyramidCoords;
        float pyramidLength = ((baseGridSize - 1) * .125f) + baseGridSize;

        for (int i = 0; i < baseGridSize; i++)
        {
            zAxis = layerStart;
            pyramidCoords = zAxis;
            int layerGridSize = baseGridSize - i;
            Color brickColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            for (int j = 0; j < Mathf.Pow(baseGridSize - i, 2); j++)
            {
                if (j % layerGridSize == 0 && j > 0)
                {
                    pyramidCoords = new Vector3(zAxis.x, zAxis.y, zAxis.z + 1.125f);
                    zAxis = pyramidCoords;
                }
                GameObject brick = GameObject.CreatePrimitive(PrimitiveType.Cube);
                brick.transform.position = new Vector3(pyramidCoords.x, pyramidCoords.y, pyramidCoords.z);
                stones.Add(brick);
                brick.GetComponent<MeshRenderer>().material.color = brickColor;
                brick.transform.parent = pyramidParentObject.transform;
                pyramidCoords = new Vector3(pyramidCoords.x + 1.125f, pyramidCoords.y, pyramidCoords.z);
            }
            layerStart = new Vector3(layerStart.x + 0.5625f, layerStart.y + 1, layerStart.z + 0.5625f);
        }

    }

    #endregion

    #region VECTOR3 STUFF

    Vector3 ExcludeCenter()
    {
        Vector3 coordinate = Vector3.zero;
        bool notInCenter = false;

        while (!notInCenter)
        {
            coordinate = new Vector3(Random.Range(-24.4f, 24.4f), .5f, Random.Range(-24.4f, 24.4f));
            if (coordinate.x > 13.5 || coordinate.x < -3.5 || coordinate.z > 13.5 || coordinate.z < -3.5)
            {
                notInCenter = true;
            }
        }

        return coordinate;
    }

    #endregion

    #region CELESTIAL OBJECT

    GameObject celestialObject;

    public void SpawnObject()
    {
        celestialObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        celestialObject.name = "CelestialObject";
        celestialObject.transform.position = new Vector3(0, 10, 0);
        celestialObject.transform.localScale = new Vector3(1.5f, 2, 1);
        celestialObject.GetComponent<MeshRenderer>().material.color = dayColor;
    }

    public void RotateObject()
    {
        celestialObject.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    IEnumerator DayNightCycle()
    {
        while (true)
        {
            // Transition from color1 to color2
            yield return StartCoroutine(TransitionColor(dayColor, nightColor, dayNightDuration));

            // Transition from color2 back to color1
            yield return StartCoroutine(TransitionColor(nightColor, dayColor, dayNightDuration));
        }
    }

    IEnumerator TransitionColor(Color startColor, Color endColor, float time)
    {
        float timer = 0.0f;

        while (timer < time)
        {
            timer += Time.deltaTime;
            // Use Lerp to smoothly interpolate between colors
            directionalLight.color = Color.Lerp(startColor, endColor, timer / time);
            celestialObject.GetComponent<MeshRenderer>().material.color = Color.Lerp(startColor, endColor, timer / time);
            yield return null; // Wait for the next frame
        }

        // Ensure the final color is set precisely
        directionalLight.color = endColor;
        celestialObject.GetComponent<MeshRenderer>().material.color = endColor;
    }

    #endregion

}