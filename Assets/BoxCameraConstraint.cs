using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCameraConstraint : CameraConstraint
{
    public bool SoftClampLeft = true;
    public bool SoftClampRight = true;
    public bool SoftClampTop = true;
    public bool SoftClampBottom = true;

    public bool HardClampLeft = true;
    public bool HardClampRight = true;
    public bool HardClampTop = true;
    public bool HardClampBottom = true;

    float LERP(float a, float b, float ratio)
    {
        return a * (1 - ratio) + b * ratio;
    }

    private void clampToBounds(ref float targetMin, ref float targetMax, float clampMin, float clampMax, bool ClampMin, bool ClampMax) {
        float range = targetMax - targetMin;
        bool ClampingMin = false;
        if (ClampMin && clampMin > targetMin)
        {
            ClampingMin = true;
            targetMin = clampMin;
            targetMax = clampMin + range;
        }
        if (ClampMax && clampMax < targetMax)
        {
            targetMax = clampMax;
            if (!ClampingMin)
            {
                // If only clamping max, restore the range
                if (ClampMin)
                {
                    targetMin = Mathf.Max(targetMax - range, clampMin);
                } else
                {
                    targetMin = targetMax - range;
                }
            }
        }
    }

    private void clampRect(ref Rect targetRect, bool ClampLeft, bool ClampRight, bool ClampTop, bool ClampBottom)
    {
        float targetXMin = targetRect.xMin;
        float targetXMax = targetRect.xMax;
        float targetYMin = targetRect.yMin;
        float targetYMax = targetRect.yMax;
        clampToBounds(ref targetXMin, ref targetXMax, clampCollider.bounds.min.x, clampCollider.bounds.max.x, ClampLeft, ClampRight);
        clampToBounds(ref targetYMin, ref targetYMax, clampCollider.bounds.min.y, clampCollider.bounds.max.y, ClampBottom, ClampTop);
        targetRect.xMin = targetXMin;
        targetRect.xMax = targetXMax;
        targetRect.yMin = targetYMin;
        targetRect.yMax = targetYMax;
    }

    public override void constrainCamera(ref Rect ClampedRect)
    {
        if (clampCollider != null)
        {
            Rect newClampedRect = new Rect(ClampedRect);
            clampRect(ref newClampedRect, HardClampLeft, HardClampRight, HardClampTop, HardClampBottom);
            float ClampRatio = 0;
            if (TimeClamped >= 0)
            {
                ClampRatio = Mathf.Min(TimeClamped, TimeToClamp) / TimeToClamp;
            }
            if (UnclampTimeLeft > 0)
            {
                ClampRatio = Mathf.Max(UnclampTimeLeft, 0) / TimeToUnclamp;
            }
            ClampedRect.xMin = LERP(ClampedRect.xMin, newClampedRect.xMin, ClampRatio);
            ClampedRect.xMax = LERP(ClampedRect.xMax, newClampedRect.xMax, ClampRatio);
            ClampedRect.yMin = LERP(ClampedRect.yMin, newClampedRect.yMin, ClampRatio);
            ClampedRect.yMax = LERP(ClampedRect.yMax, newClampedRect.yMax, ClampRatio);
        }
    }
}
