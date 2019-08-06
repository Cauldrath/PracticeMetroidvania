using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crawler : EnemyScript
{
    public bool MoveClockwise = true;
    public float ClingDistance = 1.0f;
    public float ClingPadding = 0.04f;
    public float ClimbDistance = 0.04f;
    public float MoveSpeed = 5.0f;

    private BoxCollider2D hitbox;

    // Start is called before the first frame update
    void Start()
    {
        hitbox = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float downAngle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        Vector2 downVector = new Vector2(Mathf.Sin(downAngle), -Mathf.Cos(downAngle));
        float walkAngle = (transform.rotation.eulerAngles.z + (MoveClockwise ? 90 : -90)) * Mathf.Deg2Rad;
        Vector2 walkVector = new Vector2(Mathf.Sin(walkAngle), -Mathf.Cos(walkAngle));

        Vector2[] m_RaycastPositions = new Vector2[2];
        Vector2 raycastStart = new Vector2(transform.position.x + hitbox.offset.x, transform.position.y + hitbox.offset.y);
        Vector2 raycastStartBottomCenter = raycastStart + downVector * hitbox.size.y * 0.5f;

        //Bottom right (assuming clockwise)
        m_RaycastPositions[0] = raycastStartBottomCenter + walkVector * hitbox.size.x * 0.5f;
        //Bottom left
        m_RaycastPositions[1] = raycastStartBottomCenter + walkVector * hitbox.size.x * -0.5f;

        LayerMask terrainMask = LayerMask.GetMask("Default");
        RaycastHit2D hit = Physics2D.Raycast(m_RaycastPositions[0], walkVector, ClimbDistance, terrainMask);

        if (hit.collider != null)
        {
            // Hit a wall, so climb it
            Vector2 ClingVector = hit.distance * walkVector;
            transform.position = new Vector3(transform.position.x + ClingVector.x, transform.position.y + ClingVector.y, transform.position.z);

            walkVector = downVector * -1;
            transform.rotation = Quaternion.AngleAxis(transform.rotation.eulerAngles.z + (MoveClockwise ? 90 : -90), new Vector3(0, 0, 1));
        }
        else
        {
            hit = Physics2D.Raycast(m_RaycastPositions[1], downVector, ClingDistance, terrainMask);
            if (hit.collider == null)
            {
                // Nothing below you to the left
                hit = Physics2D.Raycast(m_RaycastPositions[0], downVector, ClingDistance, terrainMask);
                if (hit.collider == null)
                {
                    // Nothing below you to the right, so rotate
                    walkVector = downVector;
                    transform.rotation = Quaternion.AngleAxis(transform.rotation.eulerAngles.z + (MoveClockwise ? -90 : 90), new Vector3(0, 0, 1));
                    // Move it the distance of the cling padding so it winds up on the new surface
                    Vector2 ClingVector = ClingPadding * walkVector;
                    transform.position = new Vector3(transform.position.x + ClingVector.x, transform.position.y + ClingVector.y, transform.position.z);
                }
                else
                {
                    // Pull the object to the surface hit
                    Vector2 ClingVector = (hit.distance - ClingPadding) * downVector;
                    transform.position = new Vector3(transform.position.x + ClingVector.x, transform.position.y + ClingVector.y, transform.position.z);
                }
            }
            else
            {
                // Pull the object to the surface hit
                Vector2 ClingVector = (hit.distance - ClingPadding) * downVector;
                transform.position = new Vector3(transform.position.x + ClingVector.x, transform.position.y + ClingVector.y, transform.position.z);
            }
        }

        Vector2 velocityVector = walkVector * Time.deltaTime * MoveSpeed;
        transform.position = new Vector3(transform.position.x + velocityVector.x, transform.position.y + velocityVector.y, transform.position.z);
    }
}
