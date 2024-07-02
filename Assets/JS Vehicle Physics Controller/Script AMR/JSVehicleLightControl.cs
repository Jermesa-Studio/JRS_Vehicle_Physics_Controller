using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSVehicleLightControl : MonoBehaviour
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

    private JSInputController mobileInputController;



    void Update()
    {
        if (mobileInputController == null)
        {
            mobileInputController = FindObjectOfType<JSInputController>();
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








