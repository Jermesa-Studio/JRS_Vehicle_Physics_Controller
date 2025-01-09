// MIT License
//
// Copyright (c) 2023 Samborlang Pyrtuh
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using UnityEngine;

public class JrsFollowCamera : MonoBehaviour
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
