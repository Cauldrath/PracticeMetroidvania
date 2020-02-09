using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerShot : AIBehavior
{
    public GameObject Shot;
    public float ShotDelay = 2.0f;
    public float ShotSpeed = 7.0f;

    private float ShotTimer = 0;
    
    public override void AIUpdate(GameObject ControlledObject, GameObject Target, Camera ViewCamera, float deltaTime)
    {        
        ShotTimer += Time.deltaTime;
        if (ShotTimer >= ShotDelay)
        {
            GameObject newShot = GameObject.Instantiate(Shot, ControlledObject.transform.position, Quaternion.identity);
            Rigidbody2D shotBody = newShot.GetComponent<Rigidbody2D>();
            if (shotBody)
            {
                if (Target != null)
                {
                    Vector3 targetDirection = Target.transform.position - ControlledObject.transform.position;
                    targetDirection.Normalize();
                    shotBody.velocity = targetDirection * ShotSpeed;
                }
                else
                {
                    shotBody.velocity = new Vector3(ShotSpeed, 0, 0);
                }
            }
            Scrollable scrollable = newShot.GetComponent<Scrollable>();
            if (scrollable)
            {
                scrollable.viewCamera = ViewCamera;
            }
            ShotTimer = 0;
        }
    }
}
