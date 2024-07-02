using UnityEngine;

public class JSVehicleDoor : MonoBehaviour
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
