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
    public GameObject Shot;

    private Damageable damageable;
    private Rigidbody2D body;
    private Rigidbody2D targetBody;
    private SpriteRenderer sprite;
    private float recovery = 0.0f;
    private float aimTime = 0.0f;
    private Vector3 lockVector = Vector3.zero;

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
    }

    // Update is called once per frame
    void Update()
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
        if (aimTime <= lockTime && lockVector == Vector3.zero)
        {
            lockVector = Target.transform.position;
            if (targetBody)
            {
                lockVector += (new Vector3(targetBody.velocity.x, targetBody.velocity.y, 0)) * aimTime;
            }
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
                Scrollable scrollable = newShot.GetComponent<Scrollable>();
                if (scrollable)
                {
                    scrollable.viewCamera = ViewCamera;
                }
            }
            lockVector = Vector3.zero;
        }
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
            float yVel = Random.Range(0.1f, 1.0f);
            if (transform.localPosition.x > 2)
            {
                xVel = Random.Range(-1.0f, 0.5f);
            } else if (transform.localPosition.x < -2)
            {
                xVel = Random.Range(0.5f, 1.0f);
            } else
            {
                xVel = Random.Range(-1.0f, 1.0f);
            }
            if (transform.localPosition.y > 0)
            {
                yVel *= -1;
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
