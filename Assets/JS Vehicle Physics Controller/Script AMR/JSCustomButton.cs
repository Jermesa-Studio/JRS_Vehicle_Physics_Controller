using UnityEngine;
using UnityEngine.EventSystems;

public class JSCustomButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    private bool isButtonPressed;
    private bool isButtonClicked;

    public void OnPointerDown(PointerEventData eventData)
    {
        isButtonPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isButtonPressed = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        isButtonClicked = true;
    }

    public bool IsButtonPressed()
    {
        return isButtonPressed;
    }

    public bool IsButtonReleased()
    {
        return !isButtonPressed;
    }

    public bool IsButtonClicked()
    {
        if (isButtonClicked)
        {
            isButtonClicked = false;
            return true;
        }
        return false;
    }
}
