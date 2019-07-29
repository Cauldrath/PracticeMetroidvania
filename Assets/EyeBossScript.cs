using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeBossScript : MonoBehaviour
{
    public float SlowMoveSpeed = 0.75f;
    public float FastMoveSpeed = 1.5f;

    private int phase = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        float moveSpeed = 0;
        if (phase == 1)
        {
            moveSpeed = SlowMoveSpeed;
        } else if (phase == 2)
        {
            moveSpeed = FastMoveSpeed;
        }
        transform.position = new Vector3(transform.position.x + moveSpeed * Time.deltaTime, transform.position.y, transform.position.z);
    }

    void Update()
    {
        int totalHealth = 0;
        Damageable[] Eyes = GetComponentsInChildren<Damageable>();
        foreach(Damageable Eye in Eyes)
        {
            totalHealth += Eye.Health;
        }
        if (totalHealth <= 20)
        {
            phase = 1;
        }
        if (totalHealth <= 10)
        {
            phase = 2;
        }
        if (totalHealth == 0)
        {
            Destroy(this.gameObject);
        }
    }
}
