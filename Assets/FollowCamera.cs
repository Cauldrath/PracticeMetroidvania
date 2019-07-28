using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public GameObject followObject;
    public Vector3 followDirection = new Vector3(0, 0, -10.0f);
    public float XMultiplier = 15.0f;
    public float YMultiplier = 9.0f;
    public float XRange = 20.0f;
    public float YRange = 20.0f;
    public float OffsetTime = 2.0f;
    public float CenteringMultiplierX = 2.0f;
    public float CenteringMultiplierY = 2.0f;

    private Vector3 VelocityOffset = new Vector3(0, 0, 0);
    private Vector3 TargetVelocityOffset = new Vector3(0, 0, 0);

    void Start()
    {
        
    }

    float LERP(float a, float b, float ratio)
    {
        return a * (1 - ratio) + b * ratio;
    }

    void FixedUpdate()
    {
        if (followObject != null)
        {
            Rigidbody2D objectBody = followObject.GetComponent<Rigidbody2D>();
            if (objectBody != null)
            {
                float XOffset = Mathf.Min(1.0f, Mathf.Max(-1.0f, objectBody.velocity.x / XRange));
                float YOffset = Mathf.Min(1.0f, Mathf.Max(-1.0f, objectBody.velocity.y / YRange));
                TargetVelocityOffset.x = XOffset * XMultiplier;
                TargetVelocityOffset.y = YOffset * YMultiplier;

                if (Mathf.Abs(VelocityOffset.x) > Mathf.Abs(TargetVelocityOffset.x))
                {
                    VelocityOffset.x = LERP(TargetVelocityOffset.x, VelocityOffset.x, (OffsetTime - Time.deltaTime * CenteringMultiplierX) / OffsetTime);
                } else
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

                this.transform.position = followObject.transform.position + followDirection + VelocityOffset;
            } else
            {
                this.transform.position = followObject.transform.position + followDirection;
            }
        }
    }
}
