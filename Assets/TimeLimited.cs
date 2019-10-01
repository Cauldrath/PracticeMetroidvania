using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLimited : MonoBehaviour
{
    public float Lifetime = 10.0f;

    private float lifeElapsed = 0;

    void Update()
    {
        if (Lifetime >= 0)
        {
            lifeElapsed += Time.deltaTime;
            if (lifeElapsed >= Lifetime)
            {
                GameObject.Destroy(this.GetComponent<GameObject>());
            }
        }
    }
}
