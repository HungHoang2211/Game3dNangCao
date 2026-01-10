using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickVisibility : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    CanvasGroup group;

    void Awake()
    {
        group = GetComponent<CanvasGroup>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        group.alpha = 1;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        group.alpha = 0;
    }
}
