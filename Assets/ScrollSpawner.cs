using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollSpawner : MonoBehaviour
{
    public GameObject Spawnable;
    public Camera viewCamera;
    public EnemyScript scriptInit;
    public float offCameraDistance = 0.1f;

    private GameObject SpawnedObject;
    private bool wasOnScreen = false;

    public bool IsVisible()
    {
        Vector3 spawnPosition = viewCamera.WorldToViewportPoint(transform.position);
        return (spawnPosition.x >= offCameraDistance * -1 && spawnPosition.x <= 1 + offCameraDistance && spawnPosition.y >= offCameraDistance * -1 && spawnPosition.y <= 1 + offCameraDistance && spawnPosition.z > 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Spawnable && SpawnedObject == null && viewCamera)
        {
            if (IsVisible())
            {
                if (!wasOnScreen)
                {
                    SpawnedObject = GameObject.Instantiate(Spawnable, transform.position, transform.rotation);
                    Scrollable objectScrollable = SpawnedObject.GetComponent<Scrollable>();
                    if (objectScrollable)
                    {
                        objectScrollable.viewCamera = viewCamera;
                        objectScrollable.setSpawner(this);
                    }
                    EnemyScript enemy = SpawnedObject.GetComponent<EnemyScript>();
                    if (enemy)
                    {
                        if (scriptInit)
                        {
                            System.Type type = scriptInit.GetType();
                            System.Reflection.FieldInfo[] fields = type.GetFields();
                            foreach (System.Reflection.FieldInfo field in fields)
                            {
                                field.SetValue(enemy, field.GetValue(scriptInit));
                            }
                        }
                        enemy.ViewCamera = viewCamera;
                        FollowCamera follow = viewCamera.GetComponent<FollowCamera>();
                        if (follow)
                        {
                            enemy.Target = follow.followObject;
                        }
                    }
                }
                wasOnScreen = true;
            } else
            {
                wasOnScreen = false;
            }
        }
    }
}
