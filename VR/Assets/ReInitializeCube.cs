using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReInitializeCube : MonoBehaviour
{
    [SerializeField]
    private GameObject cubePrefab;

    public GameObject currentCube;
    public void ResetCube()
    {
        GameObject.Destroy(currentCube);
        GameObject.Instantiate(cubePrefab);
    }
}
