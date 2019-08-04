using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerSet : MonoBehaviour
{
    public Camera viewCamera;
    
    // Start is called before the first frame update
    void Start()
    {
        if (viewCamera)
        {
            List<ScrollSpawner> spawners = new List<ScrollSpawner>();
            GetComponentsInChildren<ScrollSpawner>(true, spawners);
            foreach(ScrollSpawner spawner in spawners)
            {
                spawner.viewCamera = viewCamera;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
