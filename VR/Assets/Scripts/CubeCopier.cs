using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeCopier : MonoBehaviour
{

    public GameObject parent;
    public Transform worldPosition;
    private Vector3 firstPoint;
    private float mappingscale = 5f;
    private void Start()
    {
        firstPoint = parent.transform.position;
    }
    void Update()
    {
        if(parent == null)
        {
            Destroy(gameObject);
        }
        transform.position = mappingscale * (parent.transform.position - firstPoint) + (worldPosition.transform.position) ;
        transform.localRotation = parent.transform.localRotation;
        transform.localScale = new Vector3(3f, 3f, 3f);
    }
}
