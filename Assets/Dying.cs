using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dying : MonoBehaviour
{
    public float duration = 1.0f;

    private float timer = 0;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= duration)
        {
            Destroy(this.gameObject);
        }
    }
}
