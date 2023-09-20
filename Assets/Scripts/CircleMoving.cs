using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;
using UnityEngine;

public class CircleMoving : MonoBehaviour, IDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private UILineConnector lineConnector;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        lineConnector = FindObjectOfType<UILineConnector>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 newPos = rectTransform.anchoredPosition + eventData.delta;

        float adjustedPosX = newPos.x - rectTransform.rect.width * rectTransform.pivot.x;
        float adjustedPosY = newPos.y - rectTransform.rect.height * rectTransform.pivot.y;

        rectTransform.anchoredPosition = newPos;

        if (lineConnector != null)
        {
            lineConnector.Update();  // 원의 위치가 변경될 때마다 UILineConnector의 Update 메서드를 호출하여 선을 갱신합니다.
        }
    }
}