using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleConnect : MonoBehaviour
{
    public RectTransform[] circles;
    public GameObject linePrefab;
    public Transform spawnPoint;

    public void SetLine()
    {
        for (int i = 0; i < circles.Length; i++)
        {
            RectTransform startCircle = circles[i];
            RectTransform endCircle = circles[(i + 1) % circles.Length];  // ���� �������� ù ��° ���� �����ϱ� ���� % ������ ���

            // ���� �� ������ �߰� ��ġ ���
            Vector2 middlePosition = (startCircle.anchoredPosition + endCircle.anchoredPosition) / 2;

            // ���� ���� ���
            float distance = Vector3.Distance(startCircle.position, endCircle.position);

            // ���� ���� ���
            Vector3 direction = (endCircle.position - startCircle.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // �� �ν��Ͻ� ���� �� ����
            GameObject lineInstance = Instantiate(linePrefab, middlePosition, Quaternion.Euler(0, 0, angle), spawnPoint.transform);
            RectTransform lineRect = lineInstance.GetComponent<RectTransform>();
            if (lineRect)
            {
                lineRect.sizeDelta = new Vector2(distance, lineRect.sizeDelta.y);
            }
        }
    }
}


