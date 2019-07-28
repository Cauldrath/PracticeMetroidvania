using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public GameObject followObject;
    public Vector3 followDirection = new Vector3(0, 0, -10.0f);
    public float XMultiplier = 5.0f;
    public float YMultiplier = 5.0f;
    public float XRange = 20.0f;
    public float YRange = 20.0f;
    public float OffsetTime = 1.0f;

    private Vector3 VelocityOffset = new Vector3(0, 0, 0);

    void Start()
    {
        
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
                Vector3 NewVelocityOffset = new Vector3(XOffset * XMultiplier, YOffset * YMultiplier, 0);

                float timeRatio = (OffsetTime - Time.deltaTime) / OffsetTime;
                VelocityOffset = (NewVelocityOffset * (1 - timeRatio)) + VelocityOffset * timeRatio;

                this.transform.position = followObject.transform.position + followDirection + VelocityOffset;
            } else
            {
                this.transform.position = followObject.transform.position + followDirection;

            }
        }
    }
}
