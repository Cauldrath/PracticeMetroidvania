using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashBoss : EnemyScript
{
    public float dashAwayRange = 5.0f;
    public float recoveryTime = 1.0f;
    public float aimDelay = 4.0f;
    public float lockTime = 1.0f;
    public float aimIndicator = 1.0f;
    public float dashSpeed = 40.0f;
    public float shotSpeed = 40.0f;
    public float aimLength = 40.0f;
    public float maxAimWidth = 0.1f;
    public float minAimWidth = 1.0f;
    public Color maxAimColor = Color.red;
    public Color minAimColor = Color.cyan;
    public GameObject Shot;
    public GameObject AimLine;
    public BoxCollider2D constraint;

    private Damageable damageable;
    private Rigidbody2D body;
    private Rigidbody2D targetBody;
    private SpriteRenderer sprite;
    private float recovery = 0.0f;
    private float aimTime = 0.0f;
    private Vector3 lockVector = Vector3.zero;
    private LineRenderer aimRenderer;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        damageable = GetComponent<Damageable>();
        sprite = GetComponent<SpriteRenderer>();
        if (Target != null)
        {
            targetBody = Target.GetComponent<Rigidbody2D>();
        }
        damageable.DamagedByFaction = 0;
        aimTime = aimDelay;
        if (AimLine)
        {
            AimLine = Instantiate(AimLine, transform);
            aimRenderer = AimLine.GetComponent<LineRenderer>();
        }
    }

    // Update is called once per frame
    new void Update()
    {
        if (aimTime > 0 && aimTime <= aimIndicator)
        {
            sprite.color = Color.blue;
        }
        else if (damageable.IsInvincible())
        {
            sprite.color = Color.red;
        }
        else if (recovery > 0)
        {
            sprite.color = Color.yellow;
        }
        else
        {
            sprite.color = Color.white;
        }

        aimTime -= Time.deltaTime;
        Vector3 aimVector;
        if (lockVector != Vector3.zero)
        {
            aimVector = lockVector;
        }
        else
        {
            aimVector = Target.transform.position;
            if (targetBody)
            {
                aimVector += (new Vector3(targetBody.velocity.x, targetBody.velocity.y, 0)) * Mathf.Min(lockTime, aimTime);
            }
            // Prevent aim from leaving the room
            aimVector.x = Mathf.Min(Mathf.Max(aimVector.x, constraint.bounds.min.x), constraint.bounds.max.x);
            aimVector.y = Mathf.Min(Mathf.Max(aimVector.y, constraint.bounds.min.y), constraint.bounds.max.y);
        }
        if (aimTime <= lockTime && lockVector == Vector3.zero)
        {
            lockVector = aimVector;
        }
        if (aimTime <= 0)
        {
            aimTime = aimDelay;
            if (Shot != null)
            {
                GameObject newShot = GameObject.Instantiate(Shot, transform.position, Quaternion.identity);
                Rigidbody2D shotBody = newShot.GetComponent<Rigidbody2D>();
                if (shotBody)
                {
                    Vector3 targetDirection = lockVector - transform.position;
                    targetDirection.Normalize();
                    shotBody.velocity = targetDirection * shotSpeed;
                }
                TimeLimited timeLimited = newShot.GetComponent<TimeLimited>();
                Scrollable scrollable = newShot.GetComponent<Scrollable>();
                if (timeLimited)
                {
                    timeLimited.Lifetime = 5.0f;
                    if (scrollable)
                    {
                        scrollable.enabled = false;
                    }
                }
                else
                {
                    if (scrollable)
                    {
                        scrollable.viewCamera = ViewCamera;
                    }
                }
            }
            lockVector = Vector3.zero;
        }
        aimRenderer.SetPosition(1, (aimVector - transform.position).normalized * aimLength);
        float aimRatio = aimTime / aimDelay;
        float lineWidth = Mathf.Lerp(maxAimWidth, minAimWidth, aimRatio);
        Color lineColor = Color.Lerp(maxAimColor, minAimColor, aimRatio);
        aimRenderer.startWidth = aimRenderer.endWidth = lineWidth;
        aimRenderer.startColor = aimRenderer.endColor = lineColor;
        if (recovery > 0)
        {
            recovery -= Time.deltaTime;
            if (recovery <= 0)
            {
                recovery = 0;
                damageable.DamagedByFaction = 0;
            }
        } else if (body.velocity == Vector2.zero && (Target.transform.position - transform.position).magnitude < dashAwayRange)
        {
            float xVel;
            float yVel;
            if (transform.localPosition.x > 0)
            {
                xVel = Random.Range(-1.0f, -0.5f);
            } else
            {
                xVel = Random.Range(0.5f, 1.0f);
            }
            if (transform.localPosition.y > 0)
            {
                yVel = Random.Range(-0.25f, 0.1f);
            } else
            {
                yVel = Random.Range(0.25f, 0.5f);
            }
            body.velocity = (new Vector2(xVel, yVel)).normalized * dashSpeed;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        body.velocity = Vector2.zero;
        recovery = recoveryTime;
        damageable.DamagedByFaction = Factions.Player;
    }

}
