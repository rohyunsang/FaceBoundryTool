using UnityEngine.UI.Extensions;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/UI Line Connector")]
    [RequireComponent(typeof(UILineRenderer))]
    [ExecuteInEditMode]
    public class UILineConnector : MonoBehaviour
    {
        public RectTransform[] transforms;
        private Vector3[] previousPositions;
        private RectTransform canvas;
        private RectTransform rt;
        private UILineRenderer lr;

        private void Awake()
        {
            var canvasParent = GetComponentInParent<RectTransform>().GetParentCanvas();
            if (canvasParent != null)
            {
                canvas = canvasParent.GetComponent<RectTransform>();
            }
            rt = GetComponent<RectTransform>();
            lr = GetComponent<UILineRenderer>();
        }

        public void Update()
        {
            if (transforms == null || transforms.Length < 1)
            {
                return;
            }

            // Check each transform in the array
            for (int i = 0; i < transforms.Length; i++)
            {
                if (transforms[i] == null || transforms[i].Equals(null))
                {
                    return; // Exit if any transform is null or destroyed.
                }
            }

            // Performance check to only redraw when the child transforms move
            if (previousPositions != null && previousPositions.Length == transforms.Length)
            {
                bool updateLine = false;
                for (int i = 0; i < transforms.Length; i++)
                {
                    if (!updateLine && previousPositions[i] != transforms[i].position)
                    {
                        updateLine = true;
                    }
                }
                if (!updateLine) return;
            }

            Vector2 thisPivot = rt.pivot;
            Vector2 canvasPivot = canvas.pivot;

            Vector3[] worldSpaces = new Vector3[transforms.Length];
            Vector3[] canvasSpaces = new Vector3[transforms.Length];
            Vector2[] points = new Vector2[transforms.Length];

            for (int i = 0; i < transforms.Length; i++)
            {
                worldSpaces[i] = transforms[i].TransformPoint(thisPivot);
            }

            for (int i = 0; i < transforms.Length; i++)
            {
                canvasSpaces[i] = canvas.InverseTransformPoint(worldSpaces[i]);
            }

            for (int i = 0; i < transforms.Length; i++)
            {
                points[i] = new Vector2(canvasSpaces[i].x, canvasSpaces[i].y);
            }

            lr.Points = points;
            lr.RelativeSize = false;
            lr.drivenExternally = true;

            previousPositions = new Vector3[transforms.Length];
            for (int i = 0; i < transforms.Length; i++)
            {
                previousPositions[i] = transforms[i].position;
            }
        }
    }
}