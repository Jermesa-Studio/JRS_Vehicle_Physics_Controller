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

using System.Collections;
using UnityEngine;

public class JrsOrbitCamera : MonoBehaviour
{
    public Transform target;
    public float rotationSpeed = 5.0f;
    public float rotationSmoothTime = 0.1f;
    public float zoomSpeed = 10.0f;
    public float minZoom = 5.0f;
    public float maxZoom = 15.0f;
    public float zoomTime = 0.5f;
    public float distance;
    [SerializeField] private float middleAreaPercentage = 0.3f; // Adjust this value in the Inspector

    private float yaw = 0.0f;
    private float pitch = 45.0f; // Start from above the vehicle
    private float rotationVelocityX;
    private float rotationVelocityY;
    private Coroutine zoomCoroutine;

    void Start()
    {
        // If distance is not set in the inspector, use the initial distance to the target
        if (distance == 0)
        {
            distance = Vector3.Distance(transform.position, target.position);
        }
    }

    void Update()
    {
        // Define the middle area based on a percentage of the screen size
        //float middleAreaPercentage = 0.3f; // Adjust this value as needed
        float middleAreaWidth = Screen.width * middleAreaPercentage;
        float middleAreaHeight = Screen.height * middleAreaPercentage;
        float middleAreaX = (Screen.width - middleAreaWidth) / 2;
        float middleAreaY = (Screen.height - middleAreaHeight) / 2;
        Rect middleArea = new Rect(middleAreaX, middleAreaY, middleAreaWidth, middleAreaHeight);

        // Check if the mouse position is within the middle area
        if (middleArea.Contains(Input.mousePosition))
        {
            if (Input.GetMouseButton(0))
            {
                yaw += rotationSpeed * Input.GetAxis("Mouse X");
                pitch -= rotationSpeed * Input.GetAxis("Mouse Y");
                pitch = Mathf.Clamp(pitch, 5f, 90f); // Limit the pitch angle between -90 and 90 degrees
            }
        }

        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Check if both touches are within the middle area
            if (middleArea.Contains(touchZero.position) && middleArea.Contains(touchOne.position))
            {
                if (touchZero.phase == TouchPhase.Moved && touchOne.phase == TouchPhase.Moved)
                {
                    Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                    Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                    float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                    float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                    float deltaMagnitudeDiff = touchDeltaMag - prevTouchDeltaMag;

                    float scrollInput = deltaMagnitudeDiff * zoomSpeed * Time.deltaTime;

                    if (scrollInput != 0.0f)
                    {
                        float targetZoom = Mathf.Clamp(distance + scrollInput, minZoom, maxZoom);
                        if (zoomCoroutine != null)
                        {
                            StopCoroutine(zoomCoroutine);
                        }
                        zoomCoroutine = StartCoroutine(ZoomToTarget(targetZoom));
                    }

                    // Only perform orbiting if both touches are within the middle area
                    if (middleArea.Contains(touchZero.position) && middleArea.Contains(touchOne.position))
                    {
                        yaw += rotationSpeed * (touchZero.deltaPosition.x + touchOne.deltaPosition.x) / 2;
                        pitch -= rotationSpeed * (touchZero.deltaPosition.y + touchOne.deltaPosition.y) / 2;
                        pitch = Mathf.Clamp(pitch, 5f, 90f); // Limit the pitch angle between -90 and 90 degrees
                    }
                }
            }
        }
        else
        {
            // Zoom in and out with mouse scroll wheel
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (scrollInput != 0.0f)
            {
                float targetZoom = Mathf.Clamp(distance - scrollInput * zoomSpeed, minZoom, maxZoom);
                if (zoomCoroutine != null)
                {
                    StopCoroutine(zoomCoroutine);
                }
                zoomCoroutine = StartCoroutine(ZoomToTarget(targetZoom));
            }
        }

        // Smooth the rotation
        float smoothYaw = Mathf.SmoothDampAngle(transform.eulerAngles.y, yaw, ref rotationVelocityX, rotationSmoothTime);
        float smoothPitch = Mathf.SmoothDampAngle(transform.eulerAngles.x, pitch, ref rotationVelocityY, rotationSmoothTime);

        Quaternion rotation = Quaternion.Euler(smoothPitch, smoothYaw, 0.0f);
        transform.position = target.position - rotation * new Vector3(0, 0, distance);

        transform.LookAt(target);
    }

    IEnumerator ZoomToTarget(float targetZoom)
    {
        float startZoom = distance;
        float startTime = Time.time;
        while (Time.time < startTime + zoomTime)
        {
            distance = Mathf.Lerp(startZoom, targetZoom, (Time.time - startTime) / zoomTime);
            yield return null;
        }
        distance = targetZoom;
    }
}
