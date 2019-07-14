using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damager : MonoBehaviour
{
    [EnumFlag]
    public DamageTypes DamageType = DamageTypes.Collision;
    public int damage = 1;
    public bool active = true;
    public Collider2D damageCollider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        if (active)
        {
            Collider2D[] results = new Collider2D[10];
            int count = damageCollider.OverlapCollider(new ContactFilter2D(), results);
            for (int loop = 0; loop < count; ++loop)
            {
                Damageable damageable = results[loop].GetComponent<Damageable>();
                if (damageable)
                {
                    List<Damager> damagerChildren = new List<Damager>();
                    damageable.gameObject.GetComponentsInChildren<Damager>(damagerChildren);
                    // Only damage other objects
                    if (damagerChildren.IndexOf(this) == -1)
                    {
                        damageable.Hit(this);
                    }
                }
            }
        }
    }
}
