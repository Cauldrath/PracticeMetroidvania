using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public int maxJumps = 0;
    public float moveSpeed = 5.0f;
    public float fallSpeed = 0.5f;
    public float terminalVelocity = 10.0f;
    public float jumpSpeed = 1.0f;
    public float jumpTime = 1.0f;
    public float highJumpSpeed = 1.5f;
    public float highJumpTime = 1.33f;
    public float energyAbsorbDuration = 10.0f;
    public float dashSpeed = 10.0f;
    public float airDashTime = 1.5f;
    public float groundDashTime = 1.5f;
    public float groundedRaycastDistance = 0.02f;
    public float ceilingRaycastDistance = 0.1f;
    public float wallRaycastDistance = 0.02f;
    public float jumpForgiveness = 0.1f;
    public float knockbackDuration = 0.25f;
    public float knockbackPopup = 5.0f;
    public float knockbackSpeed = 5.0f;
    public float groundAttackDuration = 0.5f;
    public float uppercutDuration = 0.75f;
    public float jumpAttackDuration = 1.0f;
    public float downstabChargeTime = 2.0f;
    public float downstabFallSpeed = 0.75f;
    public float downstabTerminalVelocity = 15.0f;
    public float wallClimbFallSpeed = 0.5f;
    public float wallClimbFastFallSpeed = 0.5f;
    public float wallClimbTerminalVelocity = 5.0f;
    public float wallClimbFastTerminalVelocity = 10.0f;
    public Damageable damageable;
    public Damager groundMelee;
    public Damager jumpMelee;
    public Damager downStab;
    public Damager uppercut;
    public Damager currentMelee;

    public bool canAirDash = false;
    public bool canDownStab = false;
    public bool canHighJump = false;
    public bool canEnergyAbsorb = false;
    public bool canWallClimb = false;
    public bool canUppercut = false;


    private Rigidbody2D body;
    private BoxCollider2D hitbox;
    private Animator animator;
    private float jumpLeft = 0.0f;
    private float offGroundTime = 0.0f;
    private int jumpsLeft = 0;
    private bool dashJumping = false;
    private bool highJumping = false;
    private bool ceilingClinging = false;
    private bool wallClimbing = false;
    private float dashLeft = 0.0f;
    private bool showEnding = false;
    private float knockbackLeft = 0.0f;
    private float attackDuration = 0.0f;
    private float downstabDuration = 0;
    private DamageTypes energyAbsorbed = 0;
    private float energyAbsorbLeft = 0;
    private bool isAirDashing = false;
    private bool isDownstabbing = false;
    private bool isUppercutting = false;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        hitbox = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    private void OnGUI()
    {
        if (showEnding)
        {
            GUI.ModalWindow(0, new Rect(100, 100, 200, 100), windowFunc, "Game Over");
        }
    }

    private void windowFunc(int id)
    {
        GUI.Label(new Rect(10, 20, 180, 50), "You won the game. Item completion: " + (maxJumps * 100 / 3) + "%");
        if (GUI.Button(new Rect(50, 60, 100, 30), "OK"))
        {
            Application.Quit();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.activeSelf)
        {
            if (collision.gameObject.name.StartsWith("Jump Power Up"))
            {
                maxJumps++;
                collision.gameObject.SetActive(false);
            }
            if (collision.gameObject.name.StartsWith("Air Dash Power Up"))
            {
                canAirDash = true;
                collision.gameObject.SetActive(false);
            }
            if (collision.gameObject.name.StartsWith("Down Stab Power Up"))
            {
                canDownStab = true;
                collision.gameObject.SetActive(false);
            }
            if (collision.gameObject.name.StartsWith("High Jump Power Up"))
            {
                canHighJump = true;
                collision.gameObject.SetActive(false);
            }
            if (collision.gameObject.name.StartsWith("Energy Absorb Power Up"))
            {
                canEnergyAbsorb = true;
                collision.gameObject.SetActive(false);
            }
            if (collision.gameObject.name.StartsWith("Wall Climb Power Up"))
            {
                canWallClimb = true;
                collision.gameObject.SetActive(false);
            }
            if (collision.gameObject.name.StartsWith("Uppercut Power Up"))
            {
                canUppercut = true;
                collision.gameObject.SetActive(false);
            }
            if (collision.gameObject.name.StartsWith("Exit"))
            {
                showEnding = true;
            }
        }
    }

    void Update()
    {
        float verticalVelocity = body.velocity.y;
        Vector2 raycastStart = body.position + hitbox.offset;

        if (showEnding)
        {
            if (Input.GetButtonDown("Jump"))
            {
                Application.Quit();
            }
            body.velocity = new Vector2(0, 0);
            return;
        }

        if (Input.GetButtonUp("Jump"))
        {
            jumpLeft = 0;
            highJumping = false;
        }
        if (Input.GetButtonUp("Fire1"))
        {
            EndDash();
        }

        bool onGround = false;
        if (jumpLeft <= 0 && !isAirDashing && !ceilingClinging)
        {
            if (body.velocity.y <= 0)
            {
                // Check to see if you are on the ground
                Vector2[] m_RaycastPositions = new Vector2[3];

                Vector2 raycastDirection = Vector2.down;
                Vector2 raycastStartBottomCenter = raycastStart + Vector2.down * (hitbox.size.y - groundedRaycastDistance);

                m_RaycastPositions[0] = raycastStartBottomCenter + Vector2.left * hitbox.size.x * 0.5f;
                m_RaycastPositions[1] = raycastStartBottomCenter;
                m_RaycastPositions[2] = raycastStartBottomCenter + Vector2.right * hitbox.size.x * 0.5f;

                for (int i = 0; i < m_RaycastPositions.Length; i++)
                {
                    RaycastHit2D hit = Physics2D.Raycast(m_RaycastPositions[i], raycastDirection, groundedRaycastDistance * 2);
                    if (hit.collider != null)
                    {
                        onGround = true;
                    }
                }

                if (onGround)
                {
                    // If you are on the ground, restore jumps
                    jumpsLeft = maxJumps;
                    if (offGroundTime > 0) 
                    {
                        EndAttack();
                    }
                    offGroundTime = 0.0f;
                    dashJumping = false;
                    if (downstabDuration >= downstabChargeTime)
                    {
                        // TODO: Spawn charged downstab explosion
                    }
                    downstabDuration = 0.0f;
                    isDownstabbing = false;
                    jumpLeft = 0;
                    highJumping = false;
                }
                else
                {
                    // If you are not on the ground, give a little forgiveness, then remove the first jump
                    offGroundTime += Time.deltaTime;
                    if (offGroundTime > jumpForgiveness && jumpsLeft >= maxJumps)
                    {
                        jumpsLeft = maxJumps - 1;
                        EndDash();
                    }
                }
            }

            if (knockbackLeft == 0) {
                if (jumpsLeft > 0 && Input.GetButtonDown("Jump"))
                {
                    wallClimbing = false;
                    jumpLeft = jumpTime;
                    jumpsLeft--;
                    if (offGroundTime <= jumpForgiveness)
                    {
                        if (canHighJump && !wallClimbing && Input.GetAxis("Vertical") > 0)
                        {
                            jumpLeft = highJumpTime;
                            highJumping = true;
                            // You get one less jump when high jumping
                            jumpsLeft--;
                        }
                        else if (dashLeft > 0)
                        {
                            dashJumping = true;
                            // You get one less jump when dash jumping
                            jumpsLeft--;
                        }
                    }
                    EndDash();
                    EndAttack();
                }
            }
        } else
        {
            offGroundTime = jumpForgiveness;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            if (onGround)
            {
                dashLeft = groundDashTime;
            }
            else if (jumpsLeft > 0 && maxJumps > 1 && canAirDash)
            {
                // Air dash
                dashLeft = airDashTime;
                jumpLeft = 0;
                // If you are in the jump forgiveness time, remove the forgiveness jump
                if (jumpsLeft == maxJumps)
                {
                    jumpsLeft--;
                }
                jumpsLeft--;
                ceilingClinging = false;
                EndAttack();
                isAirDashing = true;
            }
        }

        float horizontalVelocity = Input.GetAxis("Horizontal") * moveSpeed;
        if (knockbackLeft > 0)
        {
            horizontalVelocity = knockbackSpeed;
            knockbackLeft -= Time.deltaTime;
            if (knockbackLeft < 0)
            {
                knockbackLeft = 0;
            }
        }
        else if (knockbackLeft < 0)
        {
            horizontalVelocity = -knockbackSpeed;
            knockbackLeft += Time.deltaTime;
            if (knockbackLeft > 0)
            {
                knockbackLeft = 0;
            }
        }
        else
        {
            if (highJumping)
            {
                if (Input.GetAxis("Vertical") > 0)
                {
                    horizontalVelocity = 0;

                    Vector2[] m_RaycastPositions = new Vector2[3];

                    Vector2 raycastDirection = Vector2.up;
                    Vector2 raycastStartTopCenter = raycastStart + Vector2.up * (hitbox.size.y * 0.5f);

                    m_RaycastPositions[0] = raycastStartTopCenter + Vector2.left * hitbox.size.x * 0.5f;
                    m_RaycastPositions[1] = raycastStartTopCenter;
                    m_RaycastPositions[2] = raycastStartTopCenter + Vector2.right * hitbox.size.x * 0.5f;

                    float closestHitDistance = -1.0f;
                    for (int i = 0; i < m_RaycastPositions.Length; i++)
                    {
                        RaycastHit2D hit = Physics2D.Raycast(m_RaycastPositions[i], raycastDirection, ceilingRaycastDistance);
                        if (hit.collider != null)
                        {
                            ceilingClinging = true;
                            if (closestHitDistance == -1 || closestHitDistance > hit.distance)
                            {
                                closestHitDistance = hit.distance;
                            }
                        }
                    }
                    if (ceilingClinging)
                    {
                        if (closestHitDistance != -1.0f)
                        {
                            transform.position = new Vector3(transform.position.x, transform.position.y + closestHitDistance, transform.position.z);
                        }
                        EndDash();
                        EndAttack();
                        highJumping = false;
                        jumpLeft = 0;
                    }
                }
                else
                {
                    highJumping = false;
                }
            }
            if (attackDuration == 0)
            {
                if (horizontalVelocity > 0)
                {
                    body.transform.localScale = new Vector3(1, 1, 1);
                }
                else if (horizontalVelocity < 0)
                {
                    body.transform.localScale = new Vector3(-1, 1, 1);
                }
            }
            if (Input.GetButtonDown("Fire2"))
            {
                if (attackDuration == 0)
                {
                    EndDash();
                    currentMelee.active = false;
                    if (onGround)
                    {
                        if (canUppercut && Input.GetAxis("Vertical") > 0)
                        {
                            // Uppercut
                            isUppercutting = true;
                            attackDuration = uppercutDuration;
                            currentMelee = uppercut;
                        }
                        else
                        {
                            // Normal ground slash
                            attackDuration = groundAttackDuration;
                            currentMelee = groundMelee;
                        }
                    }
                    else
                    {
                        if (canDownStab && Input.GetAxis("Vertical") < 0)
                        {
                            // Downstab
                            isDownstabbing = true;
                            currentMelee = downStab;
                            attackDuration = -1;
                        }
                        else
                        {
                            // Normal jump slash
                            currentMelee = jumpMelee;
                            attackDuration = jumpAttackDuration;
                        }
                        ceilingClinging = false;
                    }
                    currentMelee.active = true;
                }
            }
        }
        if (attackDuration != 0)
        {
            if (attackDuration > 0)
            {
                attackDuration -= Time.deltaTime;
                if (attackDuration < 0)
                {
                    EndAttack();
                }
            }
            if (onGround)
            {
                horizontalVelocity = 0;
            }
        }

        if (dashLeft > 0)
        {
            dashLeft -= Time.deltaTime;
            if (dashLeft <= 0)
            {
                EndDash();
            }
        }
        if (jumpLeft > 0)
        {
            jumpLeft -= Time.deltaTime;
            if (highJumping)
            {
                verticalVelocity = highJumpSpeed;
            }
            else
            {
                verticalVelocity = jumpSpeed;
            }
            if (jumpLeft < 0)
            {
                jumpLeft = 0;
                highJumping = false;
            }
            damageable.Vulnerabilities |= DamageTypes.Collision;
        }
        else if (ceilingClinging)
        {
            verticalVelocity = 0;
            horizontalVelocity = 0;
            jumpsLeft = maxJumps - 1;
            if (Input.GetAxis("Vertical") <= 0)
            {
                ceilingClinging = false;
            }
        }
        else if (isAirDashing)
        {
            verticalVelocity = 0;
            damageable.Vulnerabilities &= ~DamageTypes.Collision;
        }
        else if(wallClimbing)
        {
            if (Input.GetAxis("Vertical") < 0)
            {
                // Fast falling
                verticalVelocity -= wallClimbFastFallSpeed * Time.deltaTime;
                if (verticalVelocity < wallClimbFastTerminalVelocity * -1)
                {
                    verticalVelocity = wallClimbFastTerminalVelocity * -1;
                }
            }
            else
            {
                verticalVelocity -= wallClimbFallSpeed * Time.deltaTime;
                if (verticalVelocity < wallClimbTerminalVelocity * -1)
                {
                    verticalVelocity = wallClimbTerminalVelocity * -1;
                }
            }
            jumpsLeft = maxJumps;
        }
        else 
        {
            jumpLeft = 0;
            if (isDownstabbing)
            {
                downstabDuration += Time.deltaTime;
                verticalVelocity -= downstabFallSpeed * Time.deltaTime;
                if (verticalVelocity < downstabTerminalVelocity * -1)
                {
                    verticalVelocity = downstabTerminalVelocity * -1;
                }
            }
            else
            {
                verticalVelocity -= fallSpeed * Time.deltaTime;
                if (verticalVelocity < terminalVelocity * -1)
                {
                    verticalVelocity = terminalVelocity * -1;
                }
            }
            damageable.Vulnerabilities |= DamageTypes.Collision;
        }
        Physics2D.IgnoreLayerCollision(2, 8, (damageable.Vulnerabilities & DamageTypes.Collision) == 0);
        LayerMask terrainMask;
        if ((damageable.Vulnerabilities & DamageTypes.Collision) == 0)
        {
            terrainMask = LayerMask.GetMask("Default");
        } else
        {
            terrainMask = LayerMask.GetMask("Default", "Hazards", "Enemies");
        }
        if (dashLeft > 0 || dashJumping)
        {
            horizontalVelocity = Input.GetAxis("Horizontal") * dashSpeed;
            if (dashLeft > 0 && horizontalVelocity == 0)
            {
                horizontalVelocity = body.transform.localScale.x * dashSpeed;
            }
        }
        if (horizontalVelocity < 0)
        {
            // Check to see if you are running into a wall to the left
            Vector2 raycastDirection = Vector2.left;
            Vector2 raycastStartLeftCenter = raycastStart + Vector2.left * (hitbox.size.x * 0.5f);
            Vector2[] m_RaycastPositions = new Vector2[3];

            m_RaycastPositions[0] = raycastStartLeftCenter + Vector2.up * hitbox.size.y * 0.5f;
            m_RaycastPositions[1] = raycastStartLeftCenter;
            m_RaycastPositions[2] = raycastStartLeftCenter + Vector2.down * hitbox.size.y * 0.5f;

            bool onWall = false;
            for (int i = 0; i < m_RaycastPositions.Length; i++)
            {
                RaycastHit2D hit = Physics2D.Raycast(m_RaycastPositions[i], raycastDirection, wallRaycastDistance, terrainMask);
                if (hit.collider != null)
                {
                    onWall = true;
                }
            }


            if (onWall)
            {
                horizontalVelocity = 0;
                EndDash();
                if (canWallClimb && !onGround && jumpLeft <= 0 && !wallClimbing)
                {
                    StartWallClimb();
                }
            } else
            {
                wallClimbing = false;
            }
            if (body.velocity.x > 0)
            {
                EndDash();
            }
        }
        else if (horizontalVelocity > 0)
        {
            // Check to see if you are running into a wall to the right
            Vector2 raycastDirection = Vector2.right;
            Vector2 raycastStartRightCenter = raycastStart + Vector2.right * (hitbox.size.x * 0.5f);
            Vector2[] m_RaycastPositions = new Vector2[3];

            m_RaycastPositions[0] = raycastStartRightCenter + Vector2.up * hitbox.size.y * 0.5f;
            m_RaycastPositions[1] = raycastStartRightCenter;
            m_RaycastPositions[2] = raycastStartRightCenter + Vector2.down * hitbox.size.y * 0.5f;

            bool onWall = false;
            for (int i = 0; i < m_RaycastPositions.Length; i++)
            {
                RaycastHit2D hit = Physics2D.Raycast(m_RaycastPositions[i], raycastDirection, wallRaycastDistance, terrainMask);
                if (hit.collider != null)
                {
                    onWall = true;
                }
            }


            if (onWall)
            {
                horizontalVelocity = 0;
                EndDash();
                if (canWallClimb && !onGround && jumpLeft <= 0 && !wallClimbing)
                {
                    StartWallClimb();
                }
            } else
            {
                wallClimbing = false;
            }
            if (body.velocity.x < 0)
            {
                EndDash();
            }
        } else
        {
            EndDash();
            wallClimbing = false;
        }
        body.velocity = new Vector2(horizontalVelocity, verticalVelocity);

        animator.SetBool("OnGround", onGround);
        animator.SetBool("DownStab", isDownstabbing);
        animator.SetBool("CeilingCling", ceilingClinging);
        animator.SetBool("WallClimb", wallClimbing);
        animator.SetBool("Melee", attackDuration != 0);
        animator.SetBool("Dash", dashLeft > 0);
        animator.SetBool("ChargedDownStab", downstabDuration >= downstabChargeTime);
        animator.SetBool("Stun", knockbackLeft != 0);
        animator.SetBool("Uppercut", isUppercutting);
    }

    private void EndDash()
    {
        isAirDashing = false;
        dashLeft = 0;
    }

    private void EndAttack()
    {
        isUppercutting = false;
        isDownstabbing = false;
        attackDuration = 0;
        currentMelee.active = false;
    }

    private void StartWallClimb()
    {
        wallClimbing = true;
        EndAttack();
        isUppercutting = false;
        currentMelee.active = false;
    }

    public void OnHurt(Damager damager, Damageable damageable)
    {
        Debug.Log("Took " + damager.damage + " damage");
        jumpLeft = 0;
        EndAttack();
        EndDash();
        ceilingClinging = false;
        wallClimbing = false;
        currentMelee.active = false;
        if (damager.damageCollider.gameObject.transform.position.x < damageable.vulnerableCollider.gameObject.transform.position.x)
        {
            knockbackLeft = knockbackDuration;
        } else
        {
            knockbackLeft = -knockbackDuration;
        }
        body.velocity = new Vector2(body.velocity.x, knockbackPopup);
    }
}
