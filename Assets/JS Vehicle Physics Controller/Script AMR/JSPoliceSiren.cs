using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSPoliceSiren : MonoBehaviour
{
    private bool isSirenOn = false;
    public AudioSource sirenSound;
    public GameObject[] redLights;
    public GameObject[] blueLights;

    private JSInputController mobileInputController;

    private void Update()
    {
        if (mobileInputController == null)
        {
            mobileInputController = FindObjectOfType<JSInputController>();
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
