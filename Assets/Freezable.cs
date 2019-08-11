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
    private SpriteRenderer sprite;
    private Damager objDamager;
    private EnemyScript ai;
    private Color initialColor;
    private float flashTimer;
    private bool flashToggle;

    void Start()
    {
        initialLayer = gameObject.layer;
        sprite = GetComponent<SpriteRenderer>();
        objDamager = GetComponent<Damager>();
        ai = GetComponent<EnemyScript>();
        if (sprite)
        {
            initialColor = sprite.color;
        } else
        {
            initialColor = Color.white;
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
                if (sprite)
                {
                    sprite.color = initialColor;
                }
                if (objDamager)
                {
                    objDamager.enabled = true;
                }
                if (ai) {
                    ai.enabled = true;
                }
            }
            else if (freezeTimer < thawAnimation)
            {
                if (flashToggle)
                {
                    sprite.color = initialColor;
                } else {
                    float freezeRatio = freezeTimer / thawAnimation;
                    sprite.color = new Color(Mathf.Lerp(initialColor.r, freezeColor.r, freezeRatio), Mathf.Lerp(initialColor.g, freezeColor.g, freezeRatio),
                        Mathf.Lerp(initialColor.b, freezeColor.b, freezeRatio), Mathf.Lerp(initialColor.a, freezeColor.a, freezeRatio));
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
                if (sprite)
                {
                    sprite.color = freezeColor;
                }
            }
            else if (damageable.Health <= freezeHealth)
            {
                gameObject.layer = freezeLayer;
                damageable.Health = freezeHealth;
                freezeTimer = freezeDuration;
                if (sprite)
                {
                    sprite.color = freezeColor;
                }
                if (objDamager)
                {
                    objDamager.enabled = false;
                }
                if (ai)
                {
                    ai.enabled = false;
                }
            }
        }
    }
}
