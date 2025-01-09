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

public class JrsDoorMechanic : MonoBehaviour
{
    public float angle = 90f; // The angle to open the door
    public float speed = 90f; // The speed at which the door opens/closes
    public KeyCode toggleKey = KeyCode.Space; // The key to toggle the door

    public AudioSource openSound; // AudioSource for the sound when opening the door
    public AudioSource closeSound; // AudioSource for the sound when closing the door

    public Axis axisToRotate = Axis.Y; // The axis to rotate the door

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion initialRotation; // Store the initial rotation relative to the parent

    private void Start()
    {
        closedRotation = transform.localRotation;
        initialRotation = transform.localRotation; // Use localRotation instead of eulerAngles
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isOpen = !isOpen;
            StopAllCoroutines(); // Stop any ongoing door rotation
            if (isOpen)
            {
                Quaternion targetRotation = GetTargetRotation();
                StartCoroutine(RotateDoor(transform.localRotation, targetRotation));
                PlaySound(openSound);
            }
            else
            {
                StartCoroutine(RotateDoor(transform.localRotation, closedRotation));
                PlaySound(closeSound);
            }
        }
    }

    private Quaternion GetTargetRotation()
    {
        Vector3 targetEulerAngles = initialRotation.eulerAngles;
        switch (axisToRotate)
        {
            case Axis.X:
                targetEulerAngles.x += angle;
                break;
            case Axis.Y:
                targetEulerAngles.y += angle;
                break;
            case Axis.Z:
                targetEulerAngles.z += angle;
                break;
        }
        return Quaternion.Euler(targetEulerAngles);
    }

    private System.Collections.IEnumerator RotateDoor(Quaternion startRotation, Quaternion targetRotation)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            transform.localRotation = Quaternion.Lerp(startRotation, targetRotation, t);
            yield return null;
        }
    }

    private void PlaySound(AudioSource sound)
    {
        if (sound != null)
        {
            sound.PlayOneShot(sound.clip);
        }
    }
}

public enum Axis
{
    X,
    Y,
    Z
}
