using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjInstantManager : MonoBehaviour
{
    public GameObject rectanglePrefab;
    public GameObject circlePrefab;
    public RawImage SpawnPoint;
    public GameObject JsonParsingObj;

    public float PIXEL_WIDTH = 2136f;
    public float PIXEL_HEIGHT = 3216f;

    private float PIXEL_FACEIMAGE_WIDTH = 715f;
    private const float PIXEL_FACEIMAGE_HEIGHT = 1080f;

    public void ObjCircleInstant(List<Info> parsedInfo)
    {
        PIXEL_FACEIMAGE_WIDTH = PIXEL_WIDTH / PIXEL_HEIGHT * PIXEL_FACEIMAGE_HEIGHT;
        Debug.Log(PIXEL_FACEIMAGE_WIDTH);
        foreach (Info info in parsedInfo)
        {
            GameObjectList circleGameObjectList = new GameObjectList();
            List<GameObject> newCircles = new List<GameObject>();
            // faceLinePoints���� �� ���� �߽� ��ǥ�� ������ �� ����
            for (int i = 0; i < info.faceLinePoints.Count; i += 2)
            {
                if (i + 1 < info.faceLinePoints.Count) // ��ǥ�� x, y ������ �����ǹǷ� �˻�
                {
                    float centerX = info.faceLinePoints[i];
                    float centerY = info.faceLinePoints[i + 1];
                    

                    // �� ��ü ����
                    GameObject circle = Instantiate(circlePrefab, SpawnPoint.transform);

                    // ���� �θ� ����
                    circle.transform.SetParent(SpawnPoint.transform, false);

                    // ���� ��ġ ����
                    RectTransform circleTransform = circle.GetComponent<RectTransform>();
                    Vector2 circleCenter = new Vector2(centerX / PIXEL_WIDTH * PIXEL_FACEIMAGE_WIDTH, (PIXEL_HEIGHT - centerY) / PIXEL_WIDTH * PIXEL_FACEIMAGE_WIDTH);
                    
                    circleTransform.anchoredPosition = circleCenter - new Vector2(PIXEL_FACEIMAGE_WIDTH / 2, PIXEL_FACEIMAGE_HEIGHT / 2);
                   
                    // ���� pivot ����
                    circleTransform.pivot = new Vector2(0.5f, 0.5f);
                    circle.name = "face_line_" + (i / 2).ToString();
                    // ������ ���� newCircles ����Ʈ�� �߰�
                    newCircles.Add(circle);
                }
            }

            circleGameObjectList.gameObjects = newCircles;
            JsonParsingObj.GetComponent<JsonParsing>().jsonCircles.Add(circleGameObjectList);
        }

    }
    public void ObjRectangleInstant(List<Info> parsedInfo)
    {
        
        PIXEL_FACEIMAGE_WIDTH = PIXEL_WIDTH / PIXEL_HEIGHT * PIXEL_FACEIMAGE_HEIGHT;
        Debug.Log(PIXEL_FACEIMAGE_WIDTH);
        foreach (Info info in parsedInfo)
        {
            GameObjectList gameObjectList = new GameObjectList();
            List<GameObject> newRectangles = new List<GameObject>();
            for (int i = 0; i < info.region_name.Length; i++) 
            {
                int x1 = info.point[i*4];
                int y1 = info.point[i*4 +1];
                int x2 = info.point[i*4 +2];
                int y2 = info.point[i*4 +3];

                // Instantiate a rectangle object from the prefab
                GameObject rectangle = Instantiate(rectanglePrefab, SpawnPoint.transform);

                // Set the parent of the rectangle object
                rectangle.transform.SetParent(SpawnPoint.transform, false);

                // Set the size of the rectangle object with scaling
                RectTransform rectTransform = rectangle.GetComponent<RectTransform>();

                // The width and height of the rectangle
                float rectWidth = ((float)(x2 - x1) / PIXEL_WIDTH) * PIXEL_FACEIMAGE_WIDTH;
                float rectHeight = (y2 - y1) / PIXEL_HEIGHT * PIXEL_FACEIMAGE_HEIGHT;

                // The position of the center of the rectangle
                Vector2 rectCenter = new Vector2(((x1 + x2) / 2) / PIXEL_WIDTH * PIXEL_FACEIMAGE_WIDTH, (PIXEL_HEIGHT - (y1 + y2) / 2) / PIXEL_WIDTH * PIXEL_FACEIMAGE_WIDTH);

                // Set the size and position of the rectangle
                rectTransform.sizeDelta = new Vector2(rectWidth, rectHeight);
                rectTransform.anchoredPosition = rectCenter - new Vector2(PIXEL_FACEIMAGE_WIDTH/2, PIXEL_FACEIMAGE_HEIGHT/2); // here is your offset
                rectTransform.pivot = new Vector2(0.5f, 0.5f);

                // Set the name of the rectangle object
                rectangle.gameObject.name = info.region_name[i];
                rectangle.layer = LayerMask.NameToLayer("UI");

                Text regionNameText = rectangle.GetComponentInChildren<Text>();
                if (regionNameText != null)
                {
                    regionNameText.text = info.region_name[i];
                }
                newRectangles.Add(rectangle);
                gameObjectList.gameObjects = newRectangles;
            }
            JsonParsingObj.GetComponent<JsonParsing>().jsonSquares.Add(gameObjectList);
        }
    }
}