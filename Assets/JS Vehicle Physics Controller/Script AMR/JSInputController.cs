using UnityEngine;
using UnityEngine.UI;

public class JSInputController : MonoBehaviour
{
    public JSCustomButton accelerateButton;
    public JSCustomButton revButton;
    public JSCustomButton leftButton;
    public JSCustomButton rightButton;
    public JSCustomButton brakeButton;

    public JSCustomButton headLightsButton;
    public JSCustomButton sirenButton;

    public JSCustomButton signalLightsButton;

    public JSCustomButton extraLightsButton;

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
