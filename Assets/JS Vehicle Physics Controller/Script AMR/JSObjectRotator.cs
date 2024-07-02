using UnityEngine;

public class JSObjectRotator : MonoBehaviour
{
    public Vector3 rotationAxis = Vector3.up; // The axis around which the object will rotate
    public float rotationSpeed = 10f; // The speed at which the object will rotate

    void Update()
    {
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
    }
}
