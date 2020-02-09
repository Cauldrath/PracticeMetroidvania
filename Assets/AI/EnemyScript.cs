using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public GameObject Target;
    public Camera ViewCamera;
    public List<AIBehavior> behaviors = new List<AIBehavior>();

    public void Update()
    {
        foreach (AIBehavior behavior in behaviors)
        {
            behavior.AIUpdate(gameObject, Target, ViewCamera, Time.deltaTime);
        }
    }
}
