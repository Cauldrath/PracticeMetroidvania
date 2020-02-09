using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashBossContainer : MonoBehaviour
{
    public GameObject walls;
    public GameObject boss;
    public GameObject[] switches;
    public GameObject[] blades;
    public float switchDelay = 5.0f;
    public OffScreenIndicator offScreenIndicator;

    private CameraConstraint constraint;
    private BoxCollider2D constraintCollider;
    private Vector2 initialColliderSize;
    private float switchTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        constraint = GetComponentInChildren<CameraConstraint>();
        constraintCollider = constraint.GetComponent<BoxCollider2D>();
        initialColliderSize = new Vector2(constraintCollider.size.x, constraintCollider.size.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (walls != null && constraint != null && constraint.isClamped() && !boss.activeSelf)
        {
            constraintCollider.size = initialColliderSize + (new Vector2(2, 2));
            walls.SetActive(true);
            boss.SetActive(true);
            int switchIndex = Random.Range(0, switches.Length - 1);
            switches[switchIndex].SetActive(true);
            offScreenIndicator.Add(switches[switchIndex]);
        }
        if (switchTime > 0)
        {
            switchTime -= Time.deltaTime;
            if (switchTime <= 0)
            {
                GameObject activeSwitch = switches[Random.Range(0, switches.Length - 1)];
                activeSwitch.SetActive(true);
                offScreenIndicator.Add(activeSwitch);
            }
        }
        for (int bladeLoop = 0; bladeLoop < blades.Length; ++bladeLoop)
        {
            if (!constraintCollider.OverlapPoint(blades[bladeLoop].transform.position))
            {
                // If one of the blades escaped the room, put it back in the starting position
                blades[bladeLoop].transform.localPosition = new Vector3(0, 4.5f, 0);
            }
        }
    }

    public void onHitSwitch(Damager damager, Damageable damageable)
    {
        constraint.transform.localScale = new Vector3(constraint.transform.localScale.x * 0.9f, constraint.transform.localScale.y, constraint.transform.localScale.z);
        switchTime = switchDelay;
        for (int loop = 0; loop < switches.Length; ++loop)
        {
            if (switches[loop].activeSelf)
            {
                offScreenIndicator.Remove(switches[loop]);
                switches[loop].SetActive(false);
            }
        }
        for (int bladeLoop = 0; bladeLoop < blades.Length; ++bladeLoop)
        {
            blades[bladeLoop].transform.localPosition = new Vector3(blades[bladeLoop].transform.localPosition.x * 0.8f, blades[bladeLoop].transform.localPosition.y, blades[bladeLoop].transform.localPosition.z);
        }
        boss.transform.localPosition = new Vector3(boss.transform.localPosition.x * 0.8f, boss.transform.localPosition.y, boss.transform.localPosition.z);
    }

    public void onHitBoss(Damager damager, Damageable damageable)
    {
        constraint.transform.localScale = Vector3.one;
        if (damageable.Health <= 0)
        {
            EnemyScript bossScript = boss.GetComponent<EnemyScript>();
            if (bossScript != null)
            {
                PlayerScript player = bossScript.Target.GetComponent<PlayerScript>();
                if (player != null)
                {
                    player.SetBossDeath(1);
                }
            }
            GameObject.Destroy(this.gameObject);
        }
        else
        {
            if (blades.Length >= damageable.Health)
            {
                blades[damageable.Health - 1].SetActive(true);
            }
        }
    }
}
