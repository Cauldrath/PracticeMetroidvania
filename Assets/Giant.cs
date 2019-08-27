using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Giant : EnemyScript
{
    public float walkSpeed = 5.0f;
    public float falloffDistance = 0.5f;
    public float wallDistance = 0.5f;

    private Damageable head;
    private Animator animator;
    private Rigidbody2D body;
    private BoxCollider2D hitbox;
    private Damager sword;
    private Collider2D swordHitBox;
    private Collider2D targetHitbox;
    private SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        head = GetComponent<Damageable>();
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        hitbox = GetComponent<BoxCollider2D>();
        sword = GetComponentsInChildren<Damager>()[1];
        swordHitBox = sword.GetComponent<Collider2D>();
        targetHitbox = Target.GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        if (state.IsName("Giant_Walk"))
        {
            sword.active = false;
            body.velocity = new Vector2(transform.localScale.x * walkSpeed, body.velocity.y);
            Collider2D[] results = new Collider2D[10];
            int count = swordHitBox.OverlapCollider(new ContactFilter2D(), results);
            bool StartAttacking = false;
            for (int loop = 0; loop < count; ++loop)
            {
                if (results[loop] == targetHitbox)
                {
                    StartAttacking = true;
                }
            }
            animator.SetBool("StartAttacking", StartAttacking);
            if (StartAttacking)
            {
                sword.ResetHit();
            } else
            {
                Vector2[] m_RaycastPositions = new Vector2[3];
                Vector2 raycastStart = new Vector2(transform.position.x + hitbox.offset.x, transform.position.y + hitbox.offset.y);
                Vector2 raycastStartFrontCenter = raycastStart + new Vector2(hitbox.size.x * 0.5f * transform.localScale.x, 0);

                //Bottom front
                m_RaycastPositions[0] = raycastStartFrontCenter + Vector2.down * hitbox.size.y * 0.5f;
                //Center front
                m_RaycastPositions[1] = raycastStartFrontCenter;
                //Top front
                m_RaycastPositions[2] = raycastStartFrontCenter + Vector2.up * hitbox.size.y * 0.5f;

                LayerMask terrainMask = LayerMask.GetMask("Default");
                RaycastHit2D fallHit = Physics2D.Raycast(m_RaycastPositions[0], Vector2.down, falloffDistance, terrainMask);

                if (fallHit.collider == null)
                {
                    // Reached the end of a platform, so turn around
                    transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                } else
                {
                    bool onWall = false;
                    for (int i = 0; i < m_RaycastPositions.Length; i++)
                    {
                        RaycastHit2D hit = Physics2D.Raycast(m_RaycastPositions[i], new Vector2(transform.localScale.x, 0), wallDistance, terrainMask);
                        if (hit.collider != null)
                        {
                            onWall = true;
                        }
                    }
                    if (onWall)
                    {
                        // Reached a wall, so turn around
                        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                    }
                }

            }
        }
        else
        {
            body.velocity = new Vector2(0, body.velocity.y);
            if (state.IsName("Giant_Attack"))
            {
                sword.active = true;
            } else
            {
                sword.active = false;
            }
        }
        if (head.IsInvincible())
        {
            sprite.color = Color.red;
        } else
        {
            sprite.color = Color.white;
        }
    }
}
