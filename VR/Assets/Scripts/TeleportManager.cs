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

    [SerializeField]
    private GameObject AnimationCamera;
    [SerializeField]
    private GameObject VRCamera;

    private static int CUBE_LAYER = 6;
    private GameObject cube;
    private List<GameObject> copies;

    private int btn_count = 0;
    private void Start()
    {
        copies = new List<GameObject>();

        psGround.Stop();
        psVertical.Stop();



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

        //Can be mesh or box (in case of first shape)
        MeshCollider mc = copy.GetComponent<MeshCollider>();
        if(mc) mc.isTrigger = true;
        BoxCollider bc = copy.GetComponent<BoxCollider>();
        if (bc) bc.isTrigger = true;
        if (isTracked)
        {
            TriggerTracker tc = copy.AddComponent<TriggerTracker>();
            tc.tb = tb;
            tc.collidingMaterial = collidingMaterial;
            tc.defaultMaterial = defaultMaterial;
        }
    }

    public void ButtonPressed()
    {
        if(btn_count == 0)
        {
            StartTracking();
        }
        else
        {
            StartSimulation();
        }
    }
    public void StartTracking()
    {
        if (cube == null) return;
        StartParticles();
        cube.transform.position = (scenePosition.position + new Vector3(0, 0.1f, 0));
        RemoveCubes();
        MapObjectToWorld(cube, true);
        btn_count = 1;
    }

    public void StartSimulation()
    {
        Debug.Log("started");
        if (cube == null) return;
        Debug.Log("started2");
        MeshCollider mc = copies[0].GetComponent<MeshCollider>();
        if (mc) mc.isTrigger = false;
        BoxCollider bc = copies[0].GetComponent<BoxCollider>();
        if (bc) bc.isTrigger = false;
        Rigidbody rb = copies[0].GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
        Debug.Log("started3");
        AnimationCamera.SetActive(true);
        VRCamera.SetActive(false);
        StartCoroutine(StartWalking());
        Debug.Log("started4");
    }

    IEnumerator StartWalking()
    {
        yield return new WaitForSeconds(0.7f);
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


    void StartParticles()
    {
        psGround.Play();
        psVertical.Play();
        //PLAY SOUND
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == CUBE_LAYER)
        {
            cube = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject == cube)
        {
            //cube = null;
        }
    }
}
