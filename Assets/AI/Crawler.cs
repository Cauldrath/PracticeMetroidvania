using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crawler : EnemyScript
{
    public bool MoveClockwise = true;
    public float ClingDistance = 0.1f;
    public float FarClingDistance = 0.9f;
    public float ClimbDistance = 0.04f;
    public float MoveSpeed = 5.0f;
    public float spinSpeed = 0.0f;

    private BoxCollider2D hitbox;
    private float clingAngle = 0;
    private float spin = 0.0f;
    private int foundTerrain = 0;

    // Start is called before the first frame update
    void Start()
    {
        hitbox = GetComponent<BoxCollider2D>();
    }

    private void ClingToHit(RaycastHit2D hit, Vector2 downVector)
    {
        // Pull the object to the surface hit
        Vector2 ClingVector = (hit.distance - ClingDistance / 2) * downVector;
        transform.position = new Vector3(transform.position.x + ClingVector.x, transform.position.y + ClingVector.y, transform.position.z);
    }

    private void RotateAroundTerrain()
    {
        clingAngle += MoveClockwise ? -90 : 90;
        clingAngle = clingAngle % 360;
        transform.rotation = Quaternion.AngleAxis(clingAngle + spin, new Vector3(0, 0, 1));
        // If you rotate 4 times in a row, look for terrain again
        foundTerrain--;
    }

    void FixedUpdate()
    {
        float downAngle = clingAngle * Mathf.Deg2Rad;
        Vector2 downVector = new Vector2(Mathf.Sin(downAngle), -Mathf.Cos(downAngle));
        float walkAngle = (clingAngle + (MoveClockwise ? 90 : -90)) * Mathf.Deg2Rad;
        Vector2 walkVector = new Vector2(Mathf.Sin(walkAngle), -Mathf.Cos(walkAngle));

        Vector2[] m_RaycastPositions = new Vector2[2];
        Vector2 raycastStart = new Vector2(transform.position.x + hitbox.offset.x, transform.position.y + hitbox.offset.y);
        Vector2 raycastStartBottomCenter = raycastStart + downVector * (hitbox.size.y * 0.5f - (ClingDistance / 2));

        //Bottom right (assuming clockwise)
        m_RaycastPositions[0] = raycastStartBottomCenter + walkVector * (hitbox.size.x * 0.5f - (ClimbDistance / 2));
        //Bottom left
        m_RaycastPositions[1] = raycastStartBottomCenter + walkVector * (hitbox.size.x * -0.5f + (ClingDistance / 2));

        LayerMask terrainMask = LayerMask.GetMask("Default");
        RaycastHit2D hit = Physics2D.Raycast(m_RaycastPositions[1], downVector, ClingDistance, terrainMask);
        if (hit.collider == null)
        {
            // Nothing below you to the left
            hit = Physics2D.Raycast(m_RaycastPositions[0], downVector, ClingDistance, terrainMask);
            if (hit.collider == null)
            {
                if (foundTerrain > 0)
                {
                    // Nothing below you to the right, so rotate
                    walkVector = downVector;
                    RotateAroundTerrain();
                }
                else {
                    // Haven't found terrain yet, so see if you can pull yourself down to it
                    hit = Physics2D.Raycast(m_RaycastPositions[0], downVector, FarClingDistance, terrainMask);
                    if (hit.collider)
                    {
                        foundTerrain = 4;
                        ClingToHit(hit, downVector);
                    } else
                    {
                        // Nothing below you, so rotate
                        walkVector = downVector;
                        RotateAroundTerrain();
                    }
                }
            }
            else
            {
                foundTerrain = 4;
                if (hit.distance > ClingDistance / 2)
                {
                    ClingToHit(hit, downVector);
                }
            }
        }
        else
        {
            foundTerrain = 4;
            if (hit.distance > ClingDistance / 2)
            {
                ClingToHit(hit, downVector);
            }
        }
        // If you didn't already rotate, check to see if you are running into a wall
        if (foundTerrain == 4)
        {
            hit = Physics2D.Raycast(m_RaycastPositions[0], walkVector, ClimbDistance, terrainMask);

            if (hit.collider != null)
            {
                // Hit a wall, so climb it
                Vector2 ClingVector = (hit.distance + (ClimbDistance / 2) - (ClingDistance / 2)) * walkVector;
                walkVector = downVector * -1;
                transform.position = new Vector3(transform.position.x + ClingVector.x, transform.position.y + ClingVector.y, transform.position.z);
                clingAngle += MoveClockwise ? 90 : -90;
                clingAngle = clingAngle % 360;
                transform.rotation = Quaternion.AngleAxis(clingAngle + spin, new Vector3(0, 0, 1));
            }
        }

        Vector2 velocityVector = walkVector * Time.deltaTime * MoveSpeed;
        transform.position = new Vector3(transform.position.x + velocityVector.x, transform.position.y + velocityVector.y, transform.position.z);
        if (spinSpeed != 0.0f)
        {
            spin += spinSpeed * Time.deltaTime;
            transform.rotation = Quaternion.AngleAxis(clingAngle + spin, new Vector3(0, 0, 1));
        }
    }
}
