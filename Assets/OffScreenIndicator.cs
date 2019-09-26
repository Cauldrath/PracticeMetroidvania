using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OffScreenIndicator : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject textureTarget;
    public Canvas canvas;
    public int drawWidth = 32;
    public int drawHeight = 32;
    public Vector3 screenOffset = new Vector3(340.0f, 165.0f);
    public float offCameraDistance = 0.0f;
    public Vector3 cameraOffset = new Vector3(0, 0, -5.0f);
    private List<IndicatedObject> indicatedObjects;

    private bool IsVisible(Vector3 position)
    {
        Vector3 spawnPosition = mainCamera.WorldToViewportPoint(position);
        return (spawnPosition.x >= offCameraDistance * -1 && spawnPosition.x <= 1 + offCameraDistance && spawnPosition.y >= offCameraDistance * -1 && spawnPosition.y <= 1 + offCameraDistance && spawnPosition.z > 0);
    }

    void Start()
    {
        indicatedObjects = new List<IndicatedObject>();
    }

    public void Add(GameObject gameObject)
    {
        GameObject newTextureTarget = Instantiate(textureTarget, canvas.transform);
        newTextureTarget.SetActive(false);
        Image targetImage = newTextureTarget.GetComponent<Image>();
        GameObject cameraObject = new GameObject("Off Screen Indicator Camera");
        Camera newCamera = cameraObject.AddComponent<Camera>();
        IndicatedObject newObject = new IndicatedObject
        {
            gameObject = gameObject,
            attachedCamera = newCamera,
            textureTarget = newTextureTarget,
            image = targetImage
        };
        newObject.attachedCamera.orthographic = true;
        newObject.attachedCamera.orthographicSize = 2;
        newObject.attachedCamera.targetTexture = new RenderTexture(drawWidth, drawHeight, 24);
        indicatedObjects.Add(newObject);
    }

    public void Remove(GameObject gameObject)
    {
        bool matchObject(IndicatedObject obj)
        {
            return gameObject == obj.gameObject;
        }
        int index = indicatedObjects.FindIndex(matchObject);
        if (index > -1)
        {
            indicatedObjects.RemoveAt(index);
        }
    }

    private void objectUpdate(IndicatedObject indicatedObject)
    {
        if (indicatedObject.gameObject != null)
        {
            bool visible = IsVisible(indicatedObject.gameObject.transform.position);
            indicatedObject.attachedCamera.enabled = !visible;
            indicatedObject.textureTarget.SetActive(!visible);
            if (!visible)
            {
                indicatedObject.attachedCamera.transform.position = indicatedObject.gameObject.transform.position + cameraOffset;
                Vector3 direction = (indicatedObject.gameObject.transform.position - mainCamera.transform.position);
                direction.z = 0;
                direction.Normalize();
                RectTransform indicatorTransform = indicatedObject.textureTarget.GetComponent<RectTransform>();
                if (Mathf.Abs(direction.x) * screenOffset.y > Mathf.Abs(direction.y) * screenOffset.x)
                {
                    indicatorTransform.anchoredPosition = new Vector2(screenOffset.x * direction.x / Mathf.Abs(direction.x), direction.y * screenOffset.x / Mathf.Abs(direction.x));
                }
                else
                {
                    indicatorTransform.anchoredPosition = new Vector2(direction.x * screenOffset.y / Mathf.Abs(direction.y), screenOffset.y * direction.y / Mathf.Abs(direction.y));
                }
                //indicatorTransform.anchoredPosition = new Vector2(direction.x * screenOffset.x, direction.y * screenOffset.y);
                //indicatedObject.textureTarget.transform.position = new Vector3(direction.x * screenOffset.x, direction.y * screenOffset.y, 0.0f);
                //RenderTexture texture = new RenderTexture(drawWidth, drawHeight, 24);
                //indicatedObject.attachedCamera.targetTexture = texture;
                indicatedObject.attachedCamera.Render();
                RenderTexture.active = indicatedObject.attachedCamera.targetTexture;
                Texture2D newTexture = new Texture2D(drawWidth, drawHeight, TextureFormat.RGB24, false);
                newTexture.ReadPixels(new Rect(0, 0, drawWidth, drawHeight), 0, 0);
                newTexture.Apply();
                RenderTexture.active = null;
                //indicatedObject.attachedCamera.targetTexture = null;
                indicatedObject.image.overrideSprite = Sprite.Create(newTexture, new Rect(0, 0, drawWidth, drawHeight), new Vector2(drawWidth * 0.5f, drawHeight * 0.5f));
                // TODO: draw directional arrow
            }
        }
    }

    void Update()
    {
        Remove(null);
        indicatedObjects.ForEach(objectUpdate);
    }
}
