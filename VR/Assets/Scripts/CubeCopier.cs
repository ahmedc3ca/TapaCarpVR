using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeCopier : MonoBehaviour
{

    public GameObject parent;
    public Transform worldPosition;
    private Vector3 firstPoint;
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
        transform.position = parent.transform.position + (worldPosition.transform.position - firstPoint);
        transform.localRotation = parent.transform.localRotation;
        transform.localScale = parent.transform.localScale;
    }
}
