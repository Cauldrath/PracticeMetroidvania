using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoomer : EnemyScript
{
    public float speed = 10.0f;
    public float raycastDistance = 0.05f;

    private BoxCollider2D hitbox;
    private LayerMask terrainMask;

    void Start()
    {
        hitbox = GetComponent<BoxCollider2D>();
        terrainMask = LayerMask.GetMask("Default", "Hazards");
    }

    void FixedUpdate()
    {
        Vector2 raycastStart = new Vector2(transform.position.x, transform.position.y) + hitbox.offset;
        // Check to see if you are running into a wall to the left
        Vector2 raycastDirection = new Vector2(transform.localScale.x, 0);
        raycastDirection.Normalize();
        transform.position += new Vector3(raycastDirection.x, raycastDirection.y, 0) * speed * Time.deltaTime;

        Vector2 raycastStartFrontCenter = raycastStart + raycastDirection * (hitbox.size.x * 0.5f);
        Vector2[] m_RaycastPositions = new Vector2[3];

        m_RaycastPositions[0] = raycastStartFrontCenter + Vector2.up * hitbox.size.y * 0.5f;
        m_RaycastPositions[1] = raycastStartFrontCenter;
        m_RaycastPositions[2] = raycastStartFrontCenter + Vector2.down * hitbox.size.y * 0.5f;

        bool onWall = false;
        for (int i = 0; i < m_RaycastPositions.Length; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(m_RaycastPositions[i], raycastDirection, raycastDistance, terrainMask);
            if (hit.collider != null)
            {
                onWall = true;
            }
        }
        if (onWall)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
    }
}
