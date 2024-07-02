using UnityEngine;

public class JSFollowCamera : MonoBehaviour
{
    public Transform target; // The vehicle to follow
    public Vector3 offset; // The offset from the vehicle
    public float horizontalSpringConstant = 0.5f; // The spring constant for horizontal movement
    public float horizontalDampingConstant = 0.3f; // The damping constant for horizontal movement
    private Vector3 velocity; // The velocity of the camera

    void FixedUpdate()
    {
        Vector3 desiredPosition = target.TransformPoint(offset);
        Vector3 horizontalDisplacement = new Vector3(desiredPosition.x - transform.position.x, 0, desiredPosition.z - transform.position.z);
        Vector3 horizontalSpringForce = horizontalSpringConstant * horizontalDisplacement;
        Vector3 horizontalDampingForce = -horizontalDampingConstant * new Vector3(velocity.x, 0, velocity.z);
        Vector3 force = horizontalSpringForce + horizontalDampingForce;

        velocity += force * Time.fixedDeltaTime;

        transform.position += new Vector3(velocity.x, 0, velocity.z) * Time.fixedDeltaTime;

        // Calculate the desired camera height based on the target's position and offset
        float desiredCameraHeight = target.position.y + offset.y;

        // Set the camera's position with the desired height
        transform.position = new Vector3(transform.position.x, desiredCameraHeight, transform.position.z);

        Vector3 lookDirection = target.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(new Vector3(lookDirection.x, lookDirection.y, lookDirection.z));
        transform.rotation = rotation;
    }
}
