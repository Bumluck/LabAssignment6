using UnityEngine;
using System.Collections;

public class CreateScene : MonoBehaviour
{

    /* NOTES

    Plane is 10x10 with the center at 0, 0, 0 so work with
    -5 to 5 on X and Z axis and probably a little less so
    stuff doesnt go over the edge

    I have set plane scale to be doubled in X and Z so
    go from -10 to 10 now

    */

    #region VARIABLES

    [Header("Color Settings")]
    public Color groundColor;
    public Color treeTrunkColor;
    public Color leavesColor;

    [Header("Tree Settings")]
    public int sizeOfForest;
    public float treeProximity;

    [Header("Pyramid Settings")]
    public int stonesRequired;
    [Space]
    [Space]
    public GameObject[] trees;
    public GameObject[] stones;

    #endregion

    #region MONOBEHAVIOUR FUNCTIONS

    //----------------//
    private void Start()
    //----------------//
    {
        CreateGround();
        CreateTrees(sizeOfForest);
    }

    #endregion

    #region CREATE SCENE

    void CreateGround()
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.gameObject.name = "Ground";
        ground.GetComponent<MeshRenderer>().material.color = groundColor;
        ground.layer = 1;
        ground.transform.position = Vector3.zero;
        ground.transform.localScale = new Vector3(2, 1, 2);
    }

    void CreateTrees(int sizeOfForest)
    {
        GameObject treeParentObject = new GameObject("Forest");
        treeParentObject.transform.position = Vector3.zero;
        trees = new GameObject[sizeOfForest];
        for (int i = 0; i < sizeOfForest; i++)
        {
            GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.transform.localScale = new Vector3(.375f, .5f, .375f);
            trunk.GetComponent<MeshRenderer>().material.color = treeTrunkColor;

            GameObject leaves = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leaves.GetComponent<MeshRenderer>().material.color = leavesColor;

            leaves.transform.parent = trunk.transform;
            leaves.transform.localPosition = new Vector3(0, 2, 0);

            leaves.transform.localScale = new Vector3(2, 3, 2);

            trees[i] = trunk;

            bool clearProx = false;
            Vector3 trunkCoords = Vector3.zero;
            while (!clearProx)
            {
                trunkCoords = new Vector3(Random.Range(-9.4f, 9.4f), .5f, Random.Range(-9.4f, 9.4f));
                if (!Physics.CheckSphere(trunk.transform.position, treeProximity, 0))
                {
                    clearProx = true;
                }
            }

            trunk.transform.position = trunkCoords;
            trunk.transform.parent = treeParentObject.transform;
        }
    }

    void CreatePyramid()
    {

    }

    #endregion

    #region VECTOR3 STUFF

    float ExcludeCenter()
    {
        float coordinate = 0;
        bool notInCenter = false;

        while (!notInCenter)
        {
            coordinate = Random.Range(-9.4f, 9.4f);
            if (coordinate > 4.5 || coordinate < -4.5)
            {
                notInCenter = true;
            }
        }

        return coordinate;
    }

    #endregion

}
