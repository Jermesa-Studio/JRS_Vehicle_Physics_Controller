using UnityEngine;

public class JSLockRotation : MonoBehaviour
{
    public Transform wheel; // Reference to the wheel object

    private Vector3 initialPosition; // Initial position of the object

    private void Start()
    {
        initialPosition = transform.localPosition;
    }

    private void Update()
    {
        // Calculate the position offset based on the wheel's forward direction
        Vector3 positionOffset = wheel.forward * initialPosition.z;

        // Sync the position with the wheel
        transform.position = wheel.position + positionOffset;
    }
}
