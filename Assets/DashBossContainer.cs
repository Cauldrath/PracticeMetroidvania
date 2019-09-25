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
        int switchIndex = Random.Range(0, switches.Length - 1);
        switches[switchIndex].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (walls != null && constraint != null && constraint.isClamped())
        {
            constraintCollider.size = initialColliderSize + (new Vector2(2, 2));
            walls.SetActive(true);
            boss.SetActive(true);
        }
        if (switchTime > 0)
        {
            switchTime -= Time.deltaTime;
            if (switchTime <= 0)
            {
                switches[Random.Range(0, switches.Length - 1)].SetActive(true);
            }
        }
        for (int bladeLoop = 0; bladeLoop < blades.Length; ++bladeLoop)
        {
            if (!constraintCollider.OverlapPoint(blades[bladeLoop].transform.position))
            {
                // If one of the blades escaped the room, put it back in the middle
                blades[bladeLoop].transform.localPosition = Vector3.zero;
            }
        }
    }

    public void onHitSwitch(Damager damager, Damageable damageable)
    {
        constraint.transform.localScale *= 0.9f;
        switchTime = switchDelay;
        for (int loop = 0; loop < switches.Length; ++loop)
        {
            if (switches[loop].activeSelf)
            {
                switches[loop].SetActive(false);
            }
        }
        for (int bladeLoop = 0; bladeLoop < blades.Length; ++bladeLoop)
        {
            blades[bladeLoop].transform.localPosition *= 0.8f;
        }
    }

    public void onHitBoss(Damager damager, Damageable damageable)
    {
        constraint.transform.localScale = Vector3.one;
        if (damageable.Health <= 0)
        {
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
