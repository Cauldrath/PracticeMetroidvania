using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEyeScript : MonoBehaviour
{
    public GameObject Shot;
    public GameObject Target;
    public Camera ViewCamera;

    public float OpenTime = 2.0f;
    public float ShotSpeed = 7.0f;
    public int NumberOfShots = 1;

    public float OpenDelay;
    private int ShotsFired = 0;
    private Damageable hitbox;
    private SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        OpenDelay = OpenTime;
        hitbox = GetComponent<Damageable>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        OpenDelay += Time.deltaTime;
        if (OpenDelay < OpenTime)
        {
            hitbox.enabled = true;
            sprite.enabled = true;
            if (ShotsFired < Mathf.FloorToInt((NumberOfShots + 1) * OpenDelay / OpenTime))
            {
                GameObject newShot = GameObject.Instantiate(Shot, transform.position, Quaternion.identity);
                Rigidbody2D shotBody = newShot.GetComponent<Rigidbody2D>();
                if (shotBody)
                {
                    if (Target != null)
                    {
                        Vector3 targetDirection = Target.transform.position - transform.position;
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
                ShotsFired++;
            }
            float openScale = ((OpenDelay / OpenTime) - 0.5f) * 2.0f;
            openScale = 1 - (openScale * openScale);
            this.transform.localScale = new Vector3(1, openScale, 1);
            if (hitbox.IsInvincible())
            {
                sprite.color = Color.red;
            } else
            {
                sprite.color = Color.white;
            }
        } else
        {
            hitbox.enabled = false;
            sprite.enabled = false;
        }
    }

    public bool IsOpen()
    {
        return OpenDelay < OpenTime;
    }

    public void Open()
    {
        OpenDelay = 0;
        ShotsFired = 0;
    }
}
