using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class JSCameraSwitcher : MonoBehaviour
{
    public JSInputController inputController;
    private Camera currentCamera;
    private AudioListener currentAudioListener;
    private int currentIndex;

    private void Start()
    {
        currentCamera = inputController.cameras[0];
        currentCamera.enabled = true;
        currentAudioListener = currentCamera.GetComponent<AudioListener>();
        currentIndex = 0;
    }

    public void SwitchCamera()
    {
        // Disable the current camera and audio listener
        currentCamera.enabled = false;
        currentAudioListener.enabled = false;

        // Increment the index to switch to the next camera
        currentIndex++;

        // If the index exceeds the length of the cameras array, wrap around to the first camera
        if (currentIndex >= inputController.cameras.Length)
        {
            currentIndex = 0;
        }

        // Get the camera at the new index
        Camera newCamera = inputController.cameras[currentIndex];

        // Enable the new camera and audio listener
        newCamera.enabled = true;
        AudioListener newAudioListener = newCamera.GetComponent<AudioListener>();
        newAudioListener.enabled = true;

        // Update the currentCamera and currentAudioListener references
        currentCamera = newCamera;
        currentAudioListener = newAudioListener;
    }
}
