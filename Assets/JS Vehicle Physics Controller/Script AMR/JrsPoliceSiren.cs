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
using System.Collections.Generic;
using UnityEngine;

public class JrsPoliceSiren : MonoBehaviour
{
    private bool isSirenOn = false;
    public AudioSource sirenSound;
    public GameObject[] redLights;
    public GameObject[] blueLights;

    private JrsInputController mobileInputController;

    private void Update()
    {
        if (mobileInputController == null)
        {
            mobileInputController = FindObjectOfType<JrsInputController>();
        }
        if (Input.GetKeyDown(KeyCode.P)  || mobileInputController.sirenButton.IsButtonClicked())
        {
            isSirenOn = !isSirenOn;
            ToggleSirenLights();
            ToggleLightsVisibility(redLights, isSirenOn);
            ToggleLightsVisibility(blueLights, isSirenOn);
        }
    }

    private void ToggleSirenLights()
    {
        if (isSirenOn)
        {
            // Start the light flickering effect
            InvokeRepeating("FlickerLights", 0f, 0.1f);
            // Play the siren sound
            sirenSound.Play();
        }
        else
        {
            // Stop the light flickering effect
            CancelInvoke("FlickerLights");
            // Stop the siren sound
            sirenSound.Stop();
        }
    }

    private void FlickerLights()
    {
        foreach (GameObject redLight in redLights)
        {
            if (Random.value < 0.8f)
            {
                redLight.SetActive(!redLight.activeSelf);
            }
        }

        foreach (GameObject blueLight in blueLights)
        {
            if (Random.value < 1.5f)
            {
                blueLight.SetActive(!blueLight.activeSelf);
            }
        }
    }

    private void ToggleLightsVisibility(GameObject[] lights, bool isEnabled)
    {
        foreach (GameObject light in lights)
        {
            light.SetActive(isEnabled);
        }
    }
}
