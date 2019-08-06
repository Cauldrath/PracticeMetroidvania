using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public int maxJumps = 0;
    public float moveSpeed = 5.0f;
    public float fallSpeed = 0.5f;
    public float terminalVelocity = 10.0f;
    public float jumpSpeed = 10.0f;
    public float jumpTime = 1.0f;
    public float highJumpSpeed = 20.0f;
    public float highJumpTime = 1.33f;
    public float highJumpMinSpeed = 10.0f;
    public float energyAbsorbDuration = 10.0f;
    public float dashSpeed = 10.0f;
    public float airDashTime = 1.5f;
    public float groundDashTime = 1.5f;
    public float bounceTolerance = 0.01f;
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
    public GameObject explosion;
    public Damager groundMelee;
    public Damager jumpMelee;
    public Damager downStab;
    public Damager uppercut;
    public Damager currentMelee;
    [EnumFlag]
    public DamageTypes absorbableTypes = DamageTypes.Fire | DamageTypes.Ice | DamageTypes.Electric | DamageTypes.Explosive;

    public SavedGame saveData = new SavedGame();
    public List<GameObject> savePoints = new List<GameObject>();
    public List<GameObject> bosses = new List<GameObject>();

    private Rigidbody2D body;
    private BoxCollider2D hitbox;
    private Animator animator;
    private LayerMask terrainMask;
    private Damageable damageable;

    private float jumpLeft = 0.0f;
    private float offGroundTime = 0.0f;
    private float walkVelocity = 0.0f;
    private int jumpsLeft = 0;
    private bool dashJumping = false;
    private bool highJumping = false;
    private bool ceilingClinging = false;
    private bool wallClimbing = false;
    private float dashLeft = 0.0f;
    private float knockbackLeft = 0.0f;
    private float attackDuration = 0.0f;
    private float downstabDuration = 0;
    private DamageTypes energyAbsorbed = 0;
    private float energyAbsorbLeft = 0;
    private bool isAirDashing = false;
    private bool isDownstabbing = false;
    private bool isUppercutting = false;
    private bool onGround = false;
    private bool fastFalling = false;

    public void SaveGame()
    {
        SavedGame.SaveGame(saveData, SavedGame.saveSlot);
    }

    public void LoadGame()
    {
        saveData = SavedGame.LoadGame(SavedGame.saveSlot);
        if (saveData == null)
        {
            saveData = new SavedGame();
        }
        if (savePoints.Count > saveData.savedPoint)
        {
            transform.position = savePoints[saveData.savedPoint].gameObject.transform.position;
        }
        damageable.Health = damageable.MaxHealth;
        for (int bossIndex = 0; bossIndex < saveData.bossesKilled.Count; bossIndex++)
        {
            if (saveData.bossesKilled[bossIndex] && bosses.Count > bossIndex)
            {

                Destroy(bosses[bossIndex].gameObject);
            }
        }
        maxJumps = saveData.hasDoubleJump ? 2 : 1;
    }

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        hitbox = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        terrainMask = LayerMask.GetMask("Default", "Hazards");
        damageable = GetComponent<Damageable>();
        Cursor.visible = false;
        LoadGame();
        while (saveData.bossesKilled.Count < bosses.Count)
        {
            saveData.bossesKilled.Add(false);
        }
    }

    private void OnGUI()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.activeSelf)
        {
            int savePointIndex = savePoints.IndexOf(collision.gameObject);
            if (savePointIndex != -1)
            {
                saveData.savedPoint = savePointIndex;
                damageable.Health = damageable.MaxHealth;
                SaveGame();
            }
        }
    }

    private void FixedUpdate()
    {
        float horizontalVelocity = body.velocity.x;
        float verticalVelocity = body.velocity.y;
        Vector2 raycastStart = body.position + hitbox.offset;

        onGround = false;
        if (jumpLeft <= 0 && !isAirDashing && !ceilingClinging)
        {
            if (body.velocity.y <= bounceTolerance)
            {
                // Check to see if you are on the ground
                Vector2[] m_RaycastPositions = new Vector2[3];

                Vector2 raycastDirection = Vector2.down;
                Vector2 raycastStartBottomCenter = raycastStart + Vector2.down * (hitbox.size.y * 0.5f);

                m_RaycastPositions[0] = raycastStartBottomCenter + Vector2.left * hitbox.size.x * 0.5f;
                m_RaycastPositions[1] = raycastStartBottomCenter;
                m_RaycastPositions[2] = raycastStartBottomCenter + Vector2.right * hitbox.size.x * 0.5f;

                for (int i = 0; i < m_RaycastPositions.Length; i++)
                {
                    RaycastHit2D hit = Physics2D.Raycast(m_RaycastPositions[i], raycastDirection, groundedRaycastDistance, terrainMask);
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
                    if (downstabDuration >= downstabChargeTime && explosion != null)
                    {
                        GameObject.Instantiate(explosion, transform.position + Vector3.down * hitbox.size.y, Quaternion.identity);
                    }
                    downstabDuration = 0.0f;
                    isDownstabbing = false;
                    jumpLeft = 0;
                    highJumping = false;
                    wallClimbing = false;
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
            } else
            {
                offGroundTime = jumpForgiveness;
            }
        }
        else
        {
            offGroundTime = jumpForgiveness;
        }

        if (knockbackLeft > 0)
        {
            horizontalVelocity = knockbackSpeed;
            verticalVelocity -= fallSpeed * Time.deltaTime;
            knockbackLeft -= Time.deltaTime;
            body.transform.localScale = new Vector3(-1, 1, 1);
            if (knockbackLeft < 0)
            {
                knockbackLeft = 0;
            }
        }
        else if (knockbackLeft < 0)
        {
            horizontalVelocity = -knockbackSpeed;
            verticalVelocity -= fallSpeed * Time.deltaTime;
            knockbackLeft += Time.deltaTime;
            body.transform.localScale = new Vector3(1, 1, 1);
            if (knockbackLeft > 0)
            {
                knockbackLeft = 0;
            }
        }
        else if (highJumping)
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
                RaycastHit2D hit = Physics2D.Raycast(m_RaycastPositions[i], raycastDirection, ceilingRaycastDistance, terrainMask);
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
                horizontalVelocity = 0;
                verticalVelocity = 0;
                jumpsLeft = maxJumps - 1;
            } else {
                if (jumpLeft > 0)
                {
                    verticalVelocity = highJumpSpeed;
                    jumpLeft -= Time.deltaTime;
                    if (jumpLeft < 0)
                    {
                        jumpLeft = 0;
                    }
                } else
                {
                    verticalVelocity -= fallSpeed * Time.deltaTime;
                    if (verticalVelocity < highJumpMinSpeed)
                    {
                        highJumping = false;
                    }
                }
            }
            damageable.Vulnerabilities |= DamageTypes.Collision;
        }
        else if (ceilingClinging)
        {
            verticalVelocity = 0;
            horizontalVelocity = 0;
            jumpsLeft = maxJumps - 1;
            damageable.Vulnerabilities |= DamageTypes.Collision;
            if (walkVelocity > 0)
            {
                body.transform.localScale = new Vector3(1, 1, 1);
            }
            else if (walkVelocity < 0)
            {
                body.transform.localScale = new Vector3(-1, 1, 1);
            }
        }
        else
        {
            bool onWall = false;
            if (walkVelocity < 0)
            {
                // Check to see if you are running into a wall to the left
                Vector2 raycastDirection = Vector2.left;
                Vector2 raycastStartLeftCenter = raycastStart + Vector2.left * (hitbox.size.x * 0.5f);
                Vector2[] m_RaycastPositions = new Vector2[3];

                m_RaycastPositions[0] = raycastStartLeftCenter + Vector2.up * hitbox.size.y * 0.5f;
                m_RaycastPositions[1] = raycastStartLeftCenter;
                m_RaycastPositions[2] = raycastStartLeftCenter + Vector2.down * hitbox.size.y * 0.5f;

                for (int i = 0; i < m_RaycastPositions.Length; i++)
                {
                    RaycastHit2D hit = Physics2D.Raycast(m_RaycastPositions[i], raycastDirection, wallRaycastDistance, terrainMask);
                    if (hit.collider != null)
                    {
                        onWall = true;
                    }
                }

                if (horizontalVelocity > 0)
                {
                    EndDash();
                }
            }
            else if (walkVelocity > 0)
            {
                // Check to see if you are running into a wall to the right
                Vector2 raycastDirection = Vector2.right;
                Vector2 raycastStartRightCenter = raycastStart + Vector2.right * (hitbox.size.x * 0.5f);
                Vector2[] m_RaycastPositions = new Vector2[3];

                m_RaycastPositions[0] = raycastStartRightCenter + Vector2.up * hitbox.size.y * 0.5f;
                m_RaycastPositions[1] = raycastStartRightCenter;
                m_RaycastPositions[2] = raycastStartRightCenter + Vector2.down * hitbox.size.y * 0.5f;

                for (int i = 0; i < m_RaycastPositions.Length; i++)
                {
                    RaycastHit2D hit = Physics2D.Raycast(m_RaycastPositions[i], raycastDirection, wallRaycastDistance, terrainMask);
                    if (hit.collider != null)
                    {
                        onWall = true;
                    }
                }

                if (horizontalVelocity < 0)
                {
                    EndDash();
                }
            }
            else
            {
                wallClimbing = false;
            }

            if (onWall)
            {
                horizontalVelocity = 0;
                EndDash();
                if (saveData.hasWallClimb && !onGround && jumpLeft <= 0 && !wallClimbing)
                {
                    wallClimbing = true;
                    EndAttack();
                    EndDash();
                    downstabDuration = 0.0f;
                    isUppercutting = false;
                    currentMelee.active = false;
                }
            }
            else
            {
                wallClimbing = false;
                horizontalVelocity = walkVelocity;
                if (dashJumping)
                {
                    if (walkVelocity > 0)
                    {
                        horizontalVelocity = dashSpeed;
                    }
                    else if (walkVelocity < 0)
                    {
                        horizontalVelocity = -dashSpeed;
                    }
                }
                else if (dashLeft > 0)
                {
                    if (walkVelocity > 0)
                    {
                        horizontalVelocity = dashSpeed;
                    } else if (walkVelocity < 0)
                    {
                        horizontalVelocity = -dashSpeed;
                    } else
                    {
                        horizontalVelocity = body.transform.localScale.x * dashSpeed;
                    }
                    dashLeft -= Time.deltaTime;
                    if (dashLeft <= 0)
                    {
                        EndDash();
                    }
                }
            }

            if (attackDuration == 0)
            {
                if (walkVelocity > 0)
                {
                    body.transform.localScale = new Vector3(1, 1, 1);
                }
                else if (walkVelocity < 0)
                {
                    body.transform.localScale = new Vector3(-1, 1, 1);
                }
            }
            else
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

            if (isAirDashing)
            {
                verticalVelocity = 0;
                damageable.Vulnerabilities &= ~DamageTypes.Collision;
            }
            else
            {
                damageable.Vulnerabilities |= DamageTypes.Collision;

                if (jumpLeft > 0)
                {
                    jumpLeft -= Time.deltaTime;
                    verticalVelocity = jumpSpeed;
                    if (jumpLeft < 0)
                    {
                        jumpLeft = 0;
                    }
                }
                else if (wallClimbing)
                {
                    if (fastFalling)
                    {
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
                }
            }
        }

        Physics2D.IgnoreLayerCollision(2, 8, (damageable.Vulnerabilities & DamageTypes.Collision) == 0);
        if ((damageable.Vulnerabilities & DamageTypes.Collision) == 0)
        {
            terrainMask = LayerMask.GetMask("Default");
        }
        else
        {
            terrainMask = LayerMask.GetMask("Default", "Hazards");
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

    void Update()
    {
        // Do this every update just so it works with the editor
        if (saveData.hasEnergyAbsorb)
        {
            groundMelee.DamageType |= DamageTypes.ProjectileDestroyer;
            jumpMelee.DamageType |= DamageTypes.ProjectileDestroyer;
        } else
        {
            groundMelee.DamageType &= ~DamageTypes.ProjectileDestroyer;
            jumpMelee.DamageType &= ~DamageTypes.ProjectileDestroyer;
        }
        if (saveData.hasDoubleJump)
        {
            maxJumps = 2;
        } else
        {
            maxJumps = 1;
        }
        if (energyAbsorbLeft > 0)
        {
            energyAbsorbLeft -= Time.deltaTime;
            groundMelee.DamageType |= energyAbsorbed;
            jumpMelee.DamageType |= energyAbsorbed;
        }
        else
        {
            groundMelee.DamageType &= ~absorbableTypes;
            jumpMelee.DamageType &= ~absorbableTypes;
        }
        if (knockbackLeft == 0)
        {
            if (Input.GetButtonUp("Jump"))
            {
                jumpLeft = 0;
            }

            if (Input.GetButtonUp("Dash"))
            {
                EndDash();
            }

            if (jumpLeft <= 0 && !isAirDashing && !ceilingClinging && jumpsLeft > 0 && Input.GetButtonDown("Jump"))
            {
                jumpLeft = jumpTime;
                jumpsLeft--;
                // If this is your first jump off the ground
                if (jumpsLeft == maxJumps - 1)
                {
                    if (saveData.hasHighJump && !wallClimbing && Input.GetAxis("Vertical") > 0)
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
                wallClimbing = false;
                EndDash();
                EndAttack();
            }

            walkVelocity = Input.GetAxis("Horizontal") * moveSpeed;
            if (Input.GetButtonDown("Dash"))
            {
                if (onGround)
                {
                    dashLeft = groundDashTime;
                }
                else if (jumpsLeft > 0 && maxJumps > 1 && saveData.hasAirDash)
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

            // If you release up, stop high jumping and ceiling clinging
            if (Input.GetAxis("Vertical") <= 0)
            {
                if (highJumping)
                {
                    highJumping = false;
                }
                if (ceilingClinging)
                {
                    ceilingClinging = false;
                }
            }

            if (Input.GetButtonDown("Attack") && attackDuration == 0)
            {
                EndDash();
                currentMelee.active = false;
                if (onGround)
                {
                    if (saveData.hasUppercut && Input.GetAxis("Vertical") > 0)
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
                    if (saveData.hasDownStab && Input.GetAxis("Vertical") < 0)
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
            if (wallClimbing)
            {
                fastFalling = Input.GetAxis("Vertical") < 0;
            }
        }
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

    public void OnHurt(Damager damager, Damageable damageable)
    {
        if (damageable.Health <= 0)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
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

    public void OnHit(Damager damager, Damageable damageable)
    {
        if ((damageable.Vulnerabilities & DamageTypes.ProjectileDestroyer) != 0)
        {
            Damager shotDamage = damageable.GetComponent<Damager>();
            energyAbsorbed = absorbableTypes & shotDamage.DamageType;
            energyAbsorbLeft = energyAbsorbDuration;
        }
    }

    public void SetBossDeath(int bossID)
    {
        saveData.bossesKilled[bossID] = true;
        SaveGame();
    }
}
