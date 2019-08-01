using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damager : MonoBehaviour
{
    [System.Serializable]
    public class DamageEvent : UnityEvent<Damager, Damageable>
    { }

    [EnumFlag]
    public DamageTypes DamageType = DamageTypes.Collision;
    [EnumFlag]
    public Factions FactionDamage = Factions.Player;
    public int damage = 1;
    public bool active = true;
    public Collider2D damageCollider;
    public DamageEvent OnDamage;

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
                    if ((damageable.DamagedByFaction & FactionDamage) != 0)
                    {
                        damageable.Hit(this);
                        OnDamage.Invoke(this, damageable);
                    }
                }
            }
        }
    }
}
