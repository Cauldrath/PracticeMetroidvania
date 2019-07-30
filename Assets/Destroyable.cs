using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : MonoBehaviour
{
    public bool DestroyOnDeath = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnHurt(Damager damager, Damageable damageable)
    {
        if (damageable.Health <= 0)
        {
            if (DestroyOnDeath)
            {
                Destroy(this.gameObject);
            } else
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}
