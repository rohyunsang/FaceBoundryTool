using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;


[System.Serializable]
public class UserData
{
    public string userName;
    public string userEmail;
    public string currentTime;
    public List<CircleDataInput> circleDataList;
}

[System.Serializable]
public class CircleDataInput
{
    public string imageName;
    public List<PointData> points;
}

[System.Serializable]
public class PointData
{
    public string name;
    public List<int> points;
}



[System.Serializable]
public class RectangleEntryFile
{
    public string name;
    public List<int> points;
}


[System.Serializable]
public class FaceLine
{
    public List<int> points;
}


[System.Serializable]
public class ImageDataFile
{
    public string imageName;
    public List<FaceLine> face_line;
    public List<RectangleEntry> rectangleEntries;
}

[System.Serializable]
public class RootObject
{
    public List<ImageData> imageDataList;
}

[System.Serializable]
public class Info  //structure
{
    public string id;  // needs int to string
    public string[] region_name;
    public List<int> point;
    public List<int> faceLinePoints;
}

[System.Serializable]
public class GameObjectList
{
    public List<GameObject> gameObjects = new List<GameObject>();
}

public class JsonParsing : MonoBehaviour
{
    public GameObject PanelManagerObj;
    public GameObject ObjInstantGameObject; // using call ObjInstantManager Class Function
    public RawImage faceImage;
    public GameObject serialJsonObj;

    [SerializeField]
    public List<GameObjectList> jsonSquares = new List<GameObjectList>();
    [SerializeField]
    public List<GameObjectList> jsonCircles = new List<GameObjectList>();
    public List<Texture2D> imageDatas = new List<Texture2D>();
    public List<Info> parsedInfo = new List<Info>();

    public int idx = 0;

    public GameObject portraitPrefab;
    public Transform scrollView;
    public Transform scrollViewInitPanel;

    public GameObject failWindow;

    // using UI Renderer
    public GameObject UILineRendererObj;

    public void MakeAnnoJsonArray(string jsonData)
    {
        ParseAnnoJSONData(jsonData);
    }
    public void MakeFaceLineArray(string jsonData)
    {
        ParseFaceLineJSONData(jsonData);
    }

    public void MakeImageStringArray(byte[] bytes)
    {
        // Create a Texture2D from the image bytes
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);
        imageDatas.Add(texture);

        int width = texture.width;
        int height = texture.height;
        serialJsonObj.GetComponent<JsonSerialization>().PIXEL_WIDTH = width;
        serialJsonObj.GetComponent<JsonSerialization>().PIXEL_HEIGHT = height;
        ObjInstantGameObject.GetComponent<ObjInstantManager>().PIXEL_WIDTH = width;
        ObjInstantGameObject.GetComponent<ObjInstantManager>().PIXEL_HEIGHT = height;

        Debug.Log("Texture Width: " + width);
        Debug.Log("Texture Height: " + height);
        faceImage.GetComponent<RectTransform>().sizeDelta = new Vector2((float)width / height * 1080f, faceImage.GetComponent<RectTransform>().sizeDelta.y);
    }


    public void CheckingFileCount()
    {
        Debug.Log("jsonCircles " + jsonCircles.Count);
        Debug.Log("imageDatas " + imageDatas.Count);
        if (jsonCircles.Count == imageDatas.Count && jsonCircles.Count != 0)
        {
            InitPortrait();
        }
        else
        {
            failWindow.SetActive(true);
            Invoke("FailWindowSetActiveFalse", 3f);
            ClearObjs();
        }
    }

    public void ClearFaceLine()
    {
        jsonCircles.Clear();
    }
    public void ClearBBOX()
    {
        jsonSquares.Clear();
    }


    public void ClearObjs()
    {
        this.idx = 0; // prevent queueManager out of range
        jsonSquares.Clear();
        jsonCircles.Clear();
        imageDatas.Clear();
        parsedInfo.Clear();
        foreach (Transform child in faceImage.transform)
        {
            if (child.name.Contains("UI"))
                continue;
            Destroy(child.gameObject);
        }
    }

    private void FailWindowSetActiveFalse()
    {
        failWindow.SetActive(false);
    }
    public void Portrait()
    {
        for (int i = 0; i < imageDatas.Count; i++)
        {
            GameObject portraitInstanceA = Instantiate(portraitPrefab, scrollView.transform);
            portraitInstanceA.name = parsedInfo[i].id;
            portraitInstanceA.GetComponent<Image>().sprite = Sprite.Create(imageDatas[i], new Rect(0, 0, imageDatas[i].width, imageDatas[i].height), Vector2.one * 0.5f);
        }
    }
    public void InitPortrait()
    {
        Debug.Log("InitPortrait");
        for (int i = 0; i < imageDatas.Count; i++)
        {
            GameObject portraitInstanceB = Instantiate(portraitPrefab, scrollViewInitPanel.transform);
            portraitInstanceB.name = i.ToString();
            portraitInstanceB.GetComponent<Image>().sprite = Sprite.Create(imageDatas[i], new Rect(0, 0, imageDatas[i].width, imageDatas[i].height), Vector2.one * 0.5f);
            Destroy(portraitInstanceB.GetComponent<Button>());
        }
    }


    public void RectanglesSetActiveFalse()
    {
        foreach (GameObjectList gameObjectList in jsonSquares)
        {
            for (int i = 0; i < gameObjectList.gameObjects.Count; i++)
            {
                gameObjectList.gameObjects.ForEach(square => square.SetActive(false));
            }
        }
    }
    public void CirclesSetActiveFalse()  // reference button
    {
        foreach (GameObjectList gameObjectList in jsonCircles)
        {
            for (int i = 0; i < gameObjectList.gameObjects.Count; i++)
            {
                gameObjectList.gameObjects.ForEach(circle => circle.SetActive(false));
            }
        }
    }

    public void InitFaceImage()
    {
        faceImage.texture = null;
    }
    public void OffUILineRenderer()
    {
        UILineRendererObj.SetActive(false);
    }

    public void QueueManager(int idx) // using btn;
    {
        UILineRendererObj.SetActive(true);
        if (jsonCircles.Count != 0) 
            jsonCircles[this.idx].gameObjects.ForEach(circle => circle.SetActive(false));
        // 1. ������ Ŭ���ϸ� idx�� �������� jsonSquare�� �̹����� �ٿ��. 
        this.idx = idx;
        if (jsonCircles.Count != 0)
        {
            jsonCircles[this.idx].gameObjects.ForEach(circle => circle.SetActive(true));

            UILineConnector lineConnector = UILineRendererObj.GetComponent<UILineConnector>();
            lineConnector.transforms = jsonCircles[this.idx].gameObjects.Select(go => go.GetComponent<RectTransform>()).ToArray();
        }
        faceImage.texture = imageDatas[this.idx];
        
    }

    public void ParseAnnoJSONData(string jsonData)
    {
        var rootObject = JsonConvert.DeserializeObject<RootObject>(jsonData);

        // rootObject���� �����Ϳ� �׼���
        foreach (var imageData in rootObject.imageDataList)
        {
            Info imageInfo = new Info();
            imageInfo.id = imageData.imageName;
            imageInfo.region_name = new string[imageData.rectangleEntries.Count];
            imageInfo.point = new List<int>();
            imageInfo.faceLinePoints = new List<int>();

            // Process face_line data
            foreach (var faceLine in imageData.face_line)
            {
                imageInfo.faceLinePoints.AddRange(faceLine.points);
            }

            int i = 0;  // region_name �迭 �ε����� ���� ����

            // �� rectangleEntry�� ó���ϰ� imageInfo ��ü�� ������ �߰�
            foreach (var rectangleEntry in imageData.rectangleEntries)
            {
                imageInfo.region_name[i] = rectangleEntry.name;
                imageInfo.point.AddRange(rectangleEntry.points);
                i++;
            }
            parsedInfo.Add(imageInfo); // ������ ������ ��Ͽ� �߰�
        }

        parsedInfo.Sort((info1, info2) => string.Compare(info1.id, info2.id)); // ���Ⱑ ���� 
        ObjInstantGameObject.GetComponent<ObjInstantManager>().ObjCircleInstant(parsedInfo);
    }

    public void ParseFaceLineJSONData(string jsonData)
    {
        var rootObject = JsonConvert.DeserializeObject<UserData>(jsonData);

        // rootObject���� �����Ϳ� �׼���
        foreach (var imageData in rootObject.circleDataList)
        {
            Info imageInfo = new Info();
            imageInfo.id = imageData.imageName;
            imageInfo.point = new List<int>();
            imageInfo.faceLinePoints = new List<int>();

            // Process face_line data
            foreach (var faceLine in imageData.points)
            {
                imageInfo.faceLinePoints.AddRange(faceLine.points);
            }

         
            parsedInfo.Add(imageInfo); // ������ ������ ��Ͽ� �߰�
        }


        parsedInfo.Sort((info1, info2) => string.Compare(info1.id, info2.id));
        ObjInstantGameObject.GetComponent<ObjInstantManager>().ObjCircleInstant(parsedInfo);
    }

    public void OnOffFaceLine()
    {
        foreach (GameObjectList gameObjectList in jsonCircles)
        {
            for (int i = 0; i < gameObjectList.gameObjects.Count; i++)
            {
                GameObject circle = gameObjectList.gameObjects[i];

                // GameObject�� ���� Ȱ��ȭ ���¸� Ȯ���ϰ� �� �ݴ� ���·� �����մϴ�.
                circle.SetActive(!circle.activeSelf);
            }
        }
    }
}
