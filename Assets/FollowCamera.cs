using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public GameObject followObject;
    public Vector3 followDirection = new Vector3(0, 0, -10.0f);
    public float XMultiplier = 15.0f;
    public float YMultiplier = 15.0f;
    public float XRange = 20.0f;
    public float YRange = 22.5f;
    public float OffsetTime = 2.0f;
    public float CenteringMultiplierX = 2.0f;
    public float CenteringMultiplierY = 2.0f;
    public float MaxZoom = 8.0f;
    public float MinZoom = 10.0f;
    public float MinZoomRatio = 0.55f;
    public float MinXOffsetRatio = 0.55f;
    public float MinYOffsetRatio = 0.25f;
    public float HardClampTime = 1.0f;
    public List<CameraConstraint> constraints = new List<CameraConstraint>();
    public Camera AttachedCamera;

    private Vector3 VelocityOffset = new Vector3(0, 0, 0);
    private Vector3 TargetVelocityOffset = new Vector3(0, 0, 0);
    private float TargetZoom;
    private Rigidbody2D objectBody;

    void Start()
    {
        objectBody = followObject.GetComponent<Rigidbody2D>();
        AttachedCamera = GetComponent<Camera>();
        TargetZoom = MaxZoom;
    }

    float LERP(float a, float b, float ratio)
    {
        return a * (1 - ratio) + b * ratio;
    }

    void FixedUpdate()
    {
        if (followObject != null)
        {
            if (objectBody == null)
            {
                objectBody = followObject.GetComponent<Rigidbody2D>();
            }
            if (objectBody != null)
            {
                float XOffset = Mathf.Min(1.0f, Mathf.Max(-1.0f, objectBody.velocity.x / XRange));
                float YOffset = Mathf.Min(1.0f, Mathf.Max(-1.0f, objectBody.velocity.y / YRange));
                if (Mathf.Abs(XOffset) > MinXOffsetRatio)
                {
                    TargetVelocityOffset.x = XOffset * XMultiplier;
                } else
                {
                    TargetVelocityOffset.x = 0;
                }
                if (Mathf.Abs(YOffset) > MinYOffsetRatio)
                {
                    TargetVelocityOffset.y = YOffset * YMultiplier;
                } else
                {
                    TargetVelocityOffset.y = 0;
                }
                float ZoomRatio = Mathf.Max(Mathf.Abs(XOffset), Mathf.Abs(YOffset));
                if (ZoomRatio > MinZoomRatio)
                {
                    TargetZoom = LERP(MaxZoom, MinZoom, ZoomRatio);
                } else
                {
                    TargetZoom = MaxZoom;
                }
            } else
            {
                TargetVelocityOffset.x = 0;
                TargetVelocityOffset.y = 0;
                TargetZoom = MaxZoom;
            }

            if (Mathf.Abs(VelocityOffset.x) > Mathf.Abs(TargetVelocityOffset.x))
            {
                VelocityOffset.x = LERP(TargetVelocityOffset.x, VelocityOffset.x, (OffsetTime - Time.deltaTime * CenteringMultiplierX) / OffsetTime);
            }
            else
            {
                VelocityOffset.x = LERP(TargetVelocityOffset.x, VelocityOffset.x, (OffsetTime - Time.deltaTime) / OffsetTime);
            }
            if (Mathf.Abs(VelocityOffset.y) > Mathf.Abs(TargetVelocityOffset.y))
            {
                VelocityOffset.y = LERP(TargetVelocityOffset.y, VelocityOffset.y, (OffsetTime - Time.deltaTime * CenteringMultiplierX) / OffsetTime);
            }
            else
            {
                VelocityOffset.y = LERP(TargetVelocityOffset.y, VelocityOffset.y, (OffsetTime - Time.deltaTime) / OffsetTime);
            }
            
            if (TargetZoom > AttachedCamera.orthographicSize)
            {
                AttachedCamera.orthographicSize = LERP(TargetZoom, AttachedCamera.orthographicSize, (OffsetTime - Time.deltaTime) / OffsetTime);
            } else
            {
                AttachedCamera.orthographicSize = LERP(TargetZoom, AttachedCamera.orthographicSize, (OffsetTime - Time.deltaTime * CenteringMultiplierX) / OffsetTime);
            }
            AttachedCamera.transform.position = followObject.transform.position + followDirection + VelocityOffset;

            Vector3 lowerLeft = AttachedCamera.ViewportToWorldPoint(new Vector3(0, 0, 0));
            Vector3 upperRight = AttachedCamera.ViewportToWorldPoint(new Vector3(1, 1, 0));
            Rect currentRect = new Rect(lowerLeft.x, lowerLeft.y, upperRight.x - lowerLeft.x, upperRight.y - lowerLeft.y);
            Rect clampedRect = new Rect(currentRect);
            float clampedZoom = AttachedCamera.orthographicSize;

            foreach (CameraConstraint constraint in constraints)
            {
                constraint.constrainCamera(ref clampedRect);
            }

            float hardZoomRatio = Mathf.Min(clampedRect.width / currentRect.width, clampedRect.height / currentRect.height);
            AttachedCamera.transform.position = new Vector3(clampedRect.center.x, clampedRect.center.y, AttachedCamera.transform.position.z);
            AttachedCamera.orthographicSize = hardZoomRatio * AttachedCamera.orthographicSize;
        }
    }
}
