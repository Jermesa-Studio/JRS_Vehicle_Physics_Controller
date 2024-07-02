using UnityEngine;

public class JSDoorMechanic : MonoBehaviour
{
    public float angle = 90f; // The angle to open the door
    public float speed = 90f; // The speed at which the door opens/closes
    public KeyCode toggleKey = KeyCode.Space; // The key to toggle the door

    public AudioSource openSound; // AudioSource for the sound when opening the door
    public AudioSource closeSound; // AudioSource for the sound when closing the door

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Vector3 initialRotation;

    private void Start()
    {
        closedRotation = transform.localRotation;
        initialRotation = transform.eulerAngles;
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isOpen = !isOpen;
            StopAllCoroutines(); // Stop any ongoing door rotation
            if (isOpen)
            {
                StartCoroutine(RotateDoor(transform.localRotation, Quaternion.Euler(initialRotation.x, initialRotation.y + angle, initialRotation.z)));
                PlaySound(openSound);
            }
            else
            {
                StartCoroutine(RotateDoor(transform.localRotation, closedRotation));
                PlaySound(closeSound);
            }
        }
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
