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

public class JrsVehicleDoor : MonoBehaviour
{
    public Animator doorAnimator;
    public AudioSource openSound;
    public AudioSource closeSound;
    private bool isOpen = false;

    private void Awake()
    {
        // Ensure the doorAnimator reference is set
        if (doorAnimator == null)
        {
            doorAnimator = GetComponent<Animator>();
        }

        // Disable the Animator component to stop the animation
        doorAnimator.enabled = false;
    }

    private void OnMouseDown()
    {
        // Toggle the door open/close state only if the click is on the door object
        if (IsClickOnDoor())
        {
            // Re-enable the Animator component
            doorAnimator.enabled = true;

            isOpen = !isOpen;
            doorAnimator.SetBool("IsOpen", isOpen);

            // Play sound based on door state
            if (isOpen)
            {
                openSound.Play();
            }
            else
            {
                closeSound.Play();
            }
        }
    }

    private bool IsClickOnDoor()
    {
        // Cast a ray from the mouse position and check if it hits the door collider
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider == GetComponent<Collider>())
            {
                return true;
            }
        }
        return false;
    }
}
