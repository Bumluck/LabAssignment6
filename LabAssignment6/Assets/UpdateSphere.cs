using UnityEngine;

public class UpdateSphere : MonoBehaviour
{

    public float radius;
    public LayerMask layerMask;

    void Update()
    {
        if (Physics.CheckSphere(transform.position, radius, layerMask))
        {
            Debug.LogWarning("Trees too close to one another...");
        }
    }
}
