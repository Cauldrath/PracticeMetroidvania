using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeBossClawScript : MonoBehaviour
{
    public enum ClawPosition
    {
        TopLeft,
        TopRight,
        RightTop,
        RightBottom
    }

    public float ClawSpeed = 20.0f;
    public float ClawDelay = 1.0f;
    public float ClawDistance = 21.0f;
    public float EntryDelay = 1.0f;
    public float EntryDistance = 3.0f;
    public float VerticalPosition = 5.0f;
    public float HorizontalPosition = 19.0f;
    public float LeftPosition = 4.5f;
    public float RightPosition = 9.5f;
    public float TopPosition = 2.5f;
    public float BottomPosition = -2.5f;

    private float EntryTime = 0.0f;
    private float DelayTime = 0.0f;
    private float ClawTravel = 0.0f;
    private ClawPosition clawPosition;

    void Start()
    {
        clawPosition = (ClawPosition)Mathf.FloorToInt(Random.value * 4);
    }

    void FixedUpdate()
    {
        float XPosition = transform.position.x;
        float YPosition = transform.position.y;
        float OffsetDistance = 0.0f;
        if (EntryTime < EntryDelay)
        {
            OffsetDistance = EntryDistance * (1 - EntryTime / EntryDelay);
            EntryTime += Time.deltaTime;
        } else if (DelayTime < ClawDelay)
        {
            DelayTime += Time.deltaTime;
        } else if (ClawTravel < ClawDistance)
        {
            ClawTravel += ClawSpeed * Time.deltaTime;
        } else
        {
            EntryTime = 0;
            DelayTime = 0;
            ClawTravel = 0;
            OffsetDistance = EntryDistance;
            clawPosition = (ClawPosition)Mathf.FloorToInt(Random.value * 4);
        }

        if (clawPosition > ClawPosition.TopRight)
        {
            transform.rotation = Quaternion.Euler(0, 0, 180);
            if (clawPosition == ClawPosition.RightTop)
            {
                YPosition = TopPosition;
            } else
            {
                YPosition = BottomPosition;
            }
            XPosition = HorizontalPosition + OffsetDistance - ClawTravel;
        } else
        {
            transform.rotation = Quaternion.Euler(0, 0, 270);
            if (clawPosition == ClawPosition.TopLeft)
            {
                XPosition = LeftPosition;
            }
            else
            {
                XPosition = RightPosition;
            }
            YPosition = VerticalPosition + OffsetDistance - ClawTravel;
        }
        transform.localPosition = new Vector3(XPosition, YPosition, 0);
    }
}
