using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freezable : MonoBehaviour
{
    public float freezeDuration = 5.0f;
    public float thawAnimation = 2.0f;
    public int freezeLayer = 0;
    public int freezeHealth = 1;
    public float flashDuration = 0.03333f;
    public Color freezeColor = Color.cyan;

    private int initialLayer = 9;
    private float freezeTimer = 0;
    private SpriteRenderer[] sprites = new SpriteRenderer[0];
    private Animator animator;
    private Damager[] objDamagers = new Damager[0];
    private EnemyScript ai;
    private Color[] initialColors = new Color[0];
    private Rigidbody2D body;
    private RigidbodyConstraints2D initialConstraint = RigidbodyConstraints2D.None;
    private float flashTimer;
    private bool flashToggle;

    void Start()
    {
        initialLayer = gameObject.layer;
        sprites = GetComponentsInChildren<SpriteRenderer>();
        objDamagers = GetComponentsInChildren<Damager>();
        ai = GetComponent<EnemyScript>();
        initialColors = new Color[sprites.Length];
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        if (body)
        {
            initialConstraint = body.constraints;
        }
        for (int loop = 0; loop < sprites.Length; ++loop)
        {
            initialColors[loop] = sprites[loop].color;
        }
    }

    void Update()
    {
        if (freezeTimer > 0)
        {
            freezeTimer -= Time.deltaTime;
            if (freezeTimer <= 0)
            {
                freezeTimer = 0;
                gameObject.layer = initialLayer;
                for (int loop = 0; loop < sprites.Length; ++loop)
                {
                    sprites[loop].color = initialColors[loop];
                }
                foreach(Damager objDamager in objDamagers) {
                    objDamager.enabled = true;
                }
                if (ai) {
                    ai.enabled = true;
                }
                if (body)
                {
                    body.constraints = initialConstraint;
                }
                if (animator)
                {
                    animator.speed = 1;
                }
            }
            else if (freezeTimer < thawAnimation)
            {
                if (flashToggle)
                {
                    for (int loop = 0; loop < sprites.Length; ++loop)
                    {
                        sprites[loop].color = initialColors[loop];
                    }
                } else {
                    float freezeRatio = freezeTimer / thawAnimation;
                    for (int loop = 0; loop < sprites.Length; ++loop)
                    {
                        Color initialColor = initialColors[loop];
                        sprites[loop].color = new Color(Mathf.Lerp(initialColor.r, freezeColor.r, freezeRatio), Mathf.Lerp(initialColor.g, freezeColor.g, freezeRatio),
                            Mathf.Lerp(initialColor.b, freezeColor.b, freezeRatio), Mathf.Lerp(initialColor.a, freezeColor.a, freezeRatio));
                    }
                }
                flashTimer += Time.deltaTime;
                if (flashTimer > flashDuration)
                {
                    flashTimer -= flashDuration;
                    flashToggle = !flashToggle;
                }
            }
        }
    }

    public void OnHurt(Damager damager, Damageable damageable)
    {
        if ((damager.DamageType & DamageTypes.Ice) != 0)
        {
            if (freezeTimer > 0)
            {
                freezeTimer = freezeDuration;
                for (int loop = 0; loop < sprites.Length; ++loop)
                {
                    sprites[loop].color = freezeColor;
                }
            }
            else if (damageable.Health <= freezeHealth)
            {
                gameObject.layer = freezeLayer;
                damageable.Health = freezeHealth;
                freezeTimer = freezeDuration;
                for (int loop = 0; loop < sprites.Length; ++loop)
                {
                    sprites[loop].color = freezeColor;
                }
                foreach (Damager objDamager in objDamagers)
                {
                    objDamager.enabled = false;
                }
                if (ai)
                {
                    ai.enabled = false;
                }
                if (body)
                {
                    body.constraints = RigidbodyConstraints2D.FreezePosition;
                }
                if (animator)
                {
                    animator.speed = 0;
                }
            }
        }
    }
}
