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
using System;


public class JrsCameraSwitcher : MonoBehaviour
{
    public JrsInputController inputController;
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
