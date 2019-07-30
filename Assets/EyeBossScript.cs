using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeBossScript : MonoBehaviour
{
    public float SlowMoveSpeed = 0.75f;
    public float FastMoveSpeed = 1.5f;
    public float EyeOpenTime = 2.0f;
    public float FastEyeOpenTime = 1.5f;
    public float DeathTime = 5.0f;

    private int phase = 0;
    private float deathTimer = 0;
    private float EyeOpenDelay = 0;
    private BoxCameraConstraint constraint;
    private GameObject Target;

    // Start is called before the first frame update
    void Start()
    {
        constraint = GetComponentInChildren<BoxCameraConstraint>();
        Target = constraint.followObject;
    }

    private void FixedUpdate()
    {
        if (constraint && constraint.isClamped() && deathTimer == 0)
        {
            float moveSpeed = SlowMoveSpeed;
            if (phase == 2)
            {
                moveSpeed = FastMoveSpeed;
            }
            transform.position = new Vector3(transform.position.x + moveSpeed * Time.deltaTime, transform.position.y, transform.position.z);
        }
    }

    private static bool Unopened(BossEyeScript eye)
    {
        return !eye.IsOpen() && eye.gameObject.activeSelf;
    }

    void Update()
    {
        if (constraint)
        {
            if (deathTimer > 0)
            {
                SpriteRenderer sprite = GetComponent<SpriteRenderer>();
                sprite.color = new Color(1, 1, 1, deathTimer / DeathTime);
                deathTimer -= Time.deltaTime;
                if (deathTimer <= constraint.TimeToUnclamp)
                {
                    constraint.CanClamp = false;
                    if (deathTimer <= 0)
                    {
                        Destroy(this.gameObject);
                    }
                }
            }
            else if (constraint.isClamped())
            {
                int totalHealth = 0;
                int maxHealth = 0;
                List<BossEyeScript> Eyes = new List<BossEyeScript>();
                GetComponentsInChildren<BossEyeScript>(true, Eyes);
                foreach (BossEyeScript Eye in Eyes)
                {
                    Damageable EyeHealth = Eye.GetComponent<Damageable>();
                    totalHealth += EyeHealth.Health;
                    maxHealth += EyeHealth.MaxHealth;
                }
                if (totalHealth == 0)
                {
                    deathTimer = DeathTime;
                    EyeBossClawScript claw = GetComponentInChildren<EyeBossClawScript>();
                    if (claw)
                    {
                        claw.gameObject.SetActive(false);
                    }
                    GetComponent<Damager>().enabled = false;
                }
                else
                {
                    if (totalHealth <= maxHealth * 2 / 3)
                    {
                        phase = 1;
                        EyeBossClawScript claw = GetComponentInChildren<EyeBossClawScript>(true);
                        if (claw)
                        {
                            claw.gameObject.SetActive(true);
                        }
                    }
                    if (totalHealth <= maxHealth / 3)
                    {
                        phase = 2;
                    }
                    EyeOpenDelay += Time.deltaTime;
                    if ((phase == 2 && EyeOpenDelay > FastEyeOpenTime) || EyeOpenDelay > EyeOpenTime)
                    {
                        List<BossEyeScript> ActiveEyes = Eyes.FindAll(Unopened);
                        if (ActiveEyes.Count > 0)
                        {
                            BossEyeScript EyeToOpen = ActiveEyes[Mathf.FloorToInt(Random.value * ActiveEyes.Count)];
                            EyeToOpen.Target = Target;
                            EyeToOpen.Open();
                            EyeOpenDelay = 0;
                        }
                    }
                }
            }
        }
    }
}
