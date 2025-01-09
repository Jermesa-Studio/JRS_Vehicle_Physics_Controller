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
using UnityEngine.UI;

public class JrsInputController : MonoBehaviour
{
    public JrsCustomButton accelerateButton;
    public JrsCustomButton revButton;
    public JrsCustomButton leftButton;
    public JrsCustomButton rightButton;
    public JrsCustomButton brakeButton;

    public JrsCustomButton headLightsButton;
    public JrsCustomButton sirenButton;

    public JrsCustomButton signalLightsButton;

    public JrsCustomButton extraLightsButton;

    public float steerSpeed = 2f; // Adjust this value to control the steering speed

    private float verticalInput;
    private float horizontalInput;

    public Camera[] cameras;

    private void Update()
    {
        // Reset input values
        verticalInput = 0f;

        // Handle acceleration and braking
        if (Input.GetKey(KeyCode.W) || accelerateButton.IsButtonPressed())
        {
            verticalInput = 1f;
            //Debug.Log("Accelerate: verticalInput = " + verticalInput);
        }
        else if (Input.GetKey(KeyCode.S) || revButton.IsButtonPressed())
        {
            verticalInput = -1f;
            //Debug.Log("Brake: verticalInput = " + verticalInput);
        }

        // Handle steering
        float targetHorizontalInput = 0f;
        if (Input.GetKey(KeyCode.A) || leftButton.IsButtonPressed())
        {
            targetHorizontalInput = -1f;
           // Debug.Log("SteerLeft: targetHorizontalInput = " + targetHorizontalInput);
        }
        else if (Input.GetKey(KeyCode.D) || rightButton.IsButtonPressed())
        {
            targetHorizontalInput = 1f;
           // Debug.Log("SteerRight: targetHorizontalInput = " + targetHorizontalInput);
        }

        // Gradually change the horizontalInput value towards the targetHorizontalInput
        horizontalInput = Mathf.MoveTowards(horizontalInput, targetHorizontalInput, steerSpeed * Time.deltaTime);
    }

    public float GetVerticalInput()
    {
        return verticalInput;
    }

    public float GetHorizontalInput()
    {
        return horizontalInput;
    }
}
