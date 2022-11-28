using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportManager : MonoBehaviour
{
    public Transform cubeParent;
    public GameObject world;
    public Transform scenePosition;
    private float scalingFactor = 100f/4.5f;

    private void Start()
    {
        foreach (Transform child in cubeParent)
        {
            MapToWorld(child.gameObject);
        }

        MapToScene();
    }

    void MapToWorld(GameObject go)
    {
        GameObject copy = Instantiate(go, go.transform);
        copy.transform.position = world.transform.position;
        copy.transform.localScale *= scalingFactor;
        
    }

    void MapToScene()
    {
        GameObject copy = Instantiate(world, scenePosition.parent);
        copy.transform.position = scenePosition.position;
        copy.transform.localScale *= (1/scalingFactor);
        copy.transform.rotation = Quaternion.Euler(0,90,0);
        foreach (Transform child in copy.transform)
        {
            Renderer r = child.GetComponent<Renderer>();
            if(r != null)
            {
                r.enabled = false;
            }
        }
    }
    public void StartTracking()
    {
        StartParticles();
    }

    void StartParticles()
    {

    }

    void CopyCube()
    {

    }
}
