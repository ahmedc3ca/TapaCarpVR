using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TeleportManager : MonoBehaviour
{
    public Transform cubeParent;
    public GameObject world;
    public Transform scenePosition;


    [SerializeField]
    private ParticleSystem psGround;
    [SerializeField]
    private ParticleSystem psVertical;

    [SerializeField]
    private NavMeshSurface surface;
    [SerializeField]
    private WalkerController walkerController;
    [SerializeField]
    private TeleportButton tb;

    [SerializeField]
    private Material collidingMaterial;
    [SerializeField]
    private Material defaultMaterial;

    private float scalingFactor = 1;
    private static int CUBE_LAYER = 6;
    private GameObject cube;
    private List<GameObject> copies;
    private void Start()
    {
        copies = new List<GameObject>();
        psGround.enableEmission = false;
        psVertical.enableEmission = false;

        MapToWorld();
        //MapToScene();
    }
    

    public void MapToWorld()
    {
        RemoveCopies();
        foreach (Transform child in cubeParent)
        {
            MapObjectToWorld(child.gameObject, false);
        }
    }

    void RemoveCopies()
    {
        foreach (var go in copies)
        {
            Destroy(go);
        }
        copies = new List<GameObject>();
    }
    void MapObjectToWorld(GameObject go, bool isTracked)
    {
        if (go == null) return;
        GameObject copy = Instantiate(go);
        CubeCopier cc = copy.AddComponent<CubeCopier>();
        cc.parent = go;
        cc.worldPosition = world.transform;
        copies.Add(copy);
        copy.GetComponent<Rigidbody>().isKinematic = true;
        if (isTracked)
        {
            TriggerTracker tc = copy.AddComponent<TriggerTracker>();
            tc.tb = tb;
            tc.collidingMaterial = collidingMaterial;
            tc.defaultMaterial = defaultMaterial;
        }
    }

    public void StartTracking()
    {
        if (cube == null) return;
        StartParticles();
        cube.transform.position = (scenePosition.position + new Vector3(0, 0.1f, 0));
        RemoveCubes();
        MapObjectToWorld(cube, true);
    }

    public void StartSimulation()
    {
        surface.BuildNavMesh();
        walkerController.GoToDestination();
    }
    void RemoveCubes()
    {
        foreach (Transform child in cubeParent)
        {
            if (child.gameObject == cube) continue;
            Destroy(child.gameObject);
        }

        RemoveCopies();
    }
    void ResetChildPosition()
    {
        foreach (Transform child in cube.transform)
        {
            child.position = world.transform.position + new Vector3(0, 0.1f, 0);
        }
    }

    void StartParticles()
    {
        psGround.enableEmission = true;
        psVertical.enableEmission = true;
        //PLAY SOUND
    }

    void CopyCube()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == CUBE_LAYER)
        {
            cube = other.gameObject;
        }
    }
}
