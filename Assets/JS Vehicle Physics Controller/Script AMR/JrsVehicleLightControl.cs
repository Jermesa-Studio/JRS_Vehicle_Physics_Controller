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

public class JrsVehicleLightControl : MonoBehaviour
{
    public GameObject[] headlightElements;
    public GameObject[] signalElements;
    public GameObject[] extraLightsElements;
    public GameObject[] reverseLightsElements;
    public GameObject[] brakeLightsElements;


    private bool lightsOn = false;
    private bool signalOn = false;
    private bool extraLightsOn = false;

    private float flickerSpeed = 0.5f;
    private bool isFlickering = false;

    private JrsInputController mobileInputController;



    void Update()
    {
        if (mobileInputController == null)
        {
            mobileInputController = FindObjectOfType<JrsInputController>();
        }


        if (Input.GetKeyDown(KeyCode.H) || mobileInputController.headLightsButton.IsButtonClicked())
        {
            lightsOn = !lightsOn;
            ToggleLights();
        }

        if (Input.GetKeyDown(KeyCode.T) || mobileInputController.signalLightsButton.IsButtonClicked())
        {
            signalOn = !signalOn;
            ToggleSignal();
        }

        if (Input.GetKeyDown(KeyCode.E) || mobileInputController.extraLightsButton.IsButtonClicked())
        {
            extraLightsOn = !extraLightsOn;
            ToggleExtraLights();
        }

        if (Input.GetKey(KeyCode.S))
        {
            ToggleReverseLights(true);
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            ToggleReverseLights(false);
        }

        // Uncomment the below function so that the the reverse light for mobile will work

        // if (mobileInputController.revButton.IsButtonPressed())
        // {
        //     ToggleReverseLights(true);
        // }

        // if (mobileInputController.revButton.IsButtonReleased())
        // {
        //     ToggleReverseLights(false);
        // }
        
        
        
        if (Input.GetKey(KeyCode.Space))
        {
            ToggleBrakeLights(true);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            ToggleBrakeLights(false);
        }


        //Uncomment the below function so that the the brake light for mobile will work
        
        // if (mobileInputController.brakeButton.IsButtonPressed())
        // {
        //     ToggleBrakeLights(true);
        // }

        // if (mobileInputController.brakeButton.IsButtonReleased())
        // {
        //     ToggleBrakeLights(false);
        // }
        
    }

    void ToggleLights()
    {
        foreach (GameObject element in headlightElements)
        {
            element.SetActive(lightsOn);
        }
    }

    void ToggleSignal()
    {
        if (signalOn)
        {
            StartFlickering();
        }
        else
        {
            StopFlickering();
        }

        foreach (GameObject element in signalElements)
        {
            element.SetActive(signalOn);
        }
    }

    void ToggleExtraLights()
    {
        foreach (GameObject element in extraLightsElements)
        {
            element.SetActive(extraLightsOn);
        }
    }

    void ToggleReverseLights(bool isOn)
    {
        foreach (GameObject element in reverseLightsElements)
        {
            element.SetActive(isOn);
        }
    }

    void ToggleBrakeLights(bool isOn)
    {
        foreach (GameObject element in brakeLightsElements)
        {
            element.SetActive(isOn);
        }
    }


    void StartFlickering()
    {
        if (!isFlickering)
        {
            isFlickering = true;
            StartCoroutine(FlickerCoroutine());
        }
    }

    void StopFlickering()
    {
        if (isFlickering)
        {
            isFlickering = false;
            StopCoroutine(FlickerCoroutine());
            // Reset the object to its original state
          
        }
    }

    IEnumerator FlickerCoroutine()
    {
        while (isFlickering)
        {
            foreach (GameObject element in signalElements)
            {
                // Toggle the visibility of the object
                element.SetActive(!element.activeSelf);
            }
            yield return new WaitForSeconds(flickerSpeed);
        }
    }
}








