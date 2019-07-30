using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraConstraint : MonoBehaviour
{
    public FollowCamera clampedCamera;
    public GameObject followObject;

    protected Collider2D clampCollider;
    public float TimeToClamp = 1.0f;
    public float TimeToUnclamp = 1.0f;
    public bool CanClamp = true;

    protected float TimeClamped = 0.0f;
    protected float UnclampTimeLeft = 0.0f;

    void Start()
    {
        clampCollider = GetComponent<Collider2D>();
    }

    void FixedUpdate()
    {
        if (clampCollider == null)
        {
            clampCollider = GetComponent<Collider2D>();
        }
        if (clampCollider != null && clampedCamera != null && followObject != null)
        {
            if (CanClamp && clampCollider.OverlapPoint(followObject.transform.position))
            {
                if (UnclampTimeLeft > 0)
                {
                    TimeClamped = TimeToClamp * UnclampTimeLeft / TimeToUnclamp;
                    UnclampTimeLeft = 0;
                } else
                {
                    TimeClamped += Time.deltaTime;

                }
                TimeClamped = Mathf.Min(TimeClamped, TimeToClamp);
                if (!clampedCamera.constraints.Contains(this))
                {
                    clampedCamera.constraints.Add(this);
                }
            }
            else
            {
                if (TimeClamped > 0)
                {
                    UnclampTimeLeft = TimeToUnclamp * TimeClamped / TimeToClamp;
                    TimeClamped = 0;
                } else
                {
                    UnclampTimeLeft -= Time.deltaTime;
                }
                UnclampTimeLeft = Mathf.Max(UnclampTimeLeft, 0);
                if (UnclampTimeLeft <= 0)
                {
                    int ClampIndex = clampedCamera.constraints.IndexOf(this);
                    if (ClampIndex >= 0)
                    {
                        clampedCamera.constraints.RemoveAt(ClampIndex);
                    }
                }
            }
        }
    }

    public bool isClamped()
    {
        return TimeClamped > 0;
    }

    public bool doneUnclamping()
    {
        return UnclampTimeLeft <= 0;
    }

    public virtual void constrainCamera(ref Rect ClampedRect)
    {

    }
}
