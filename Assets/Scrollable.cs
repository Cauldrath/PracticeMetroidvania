using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrollable : MonoBehaviour
{
    public ScrollSpawner Spawner;
    public Camera viewCamera;

    private Renderer m_Renderer;

    public bool IsVisibleFrom(Renderer renderer, Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_Renderer = GetComponent<Renderer>();
        if (viewCamera == null)
        {
            viewCamera = Camera.current;
        }
    }

    public void setSpawner(ScrollSpawner newSpawner)
    {
        Spawner = newSpawner;
    }

    // Update is called once per frame
    void Update()
    {
        if (viewCamera && m_Renderer) {
            if (!IsVisibleFrom(m_Renderer, viewCamera))
            {
                if (Spawner == null || !Spawner.IsVisible())
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
