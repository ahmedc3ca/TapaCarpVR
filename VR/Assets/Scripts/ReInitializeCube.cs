using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReInitializeCube : MonoBehaviour
{
    [SerializeField]
    private GameObject cubePrefab;

    [SerializeField]
    private TeleportManager tm;

    public Transform cubeParent;
    public void ResetCube()
    {
        SceneManager.LoadScene("Level1");
/*        foreach(Transform child in cubeParent)
        {
            Destroy(child.gameObject);
        }
        
        GameObject newcube = Instantiate(cubePrefab);
        newcube.transform.parent = cubeParent;
        tm.MapToWorld();*/
    }
}
