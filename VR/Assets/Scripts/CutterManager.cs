using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CutterManager : MonoBehaviour
{
    [SerializeField]
    private GameObject corner1;
    [SerializeField]
    private GameObject corner2;
    [SerializeField]
    private GameObject corner3;
    [SerializeField]
    private GameObject cubeParent;
    

    protected MeshFilter meshFilter;
    protected Mesh mesh;


    private List<GameObject> ogs;
    private List<GameObject> rightCuts;
    private List<GameObject> leftCuts;
    
    //MESH CODE TO BE REMOVED SINCE IT'S STATIC
    // Start is called before the first frame update
    void Start()
    {
        ogs = new List<GameObject>();
        rightCuts = new List<GameObject>();
        leftCuts = new List<GameObject>();

        mesh = new Mesh();
        mesh.name = "Plane";

        mesh.vertices = GenerateVerts();
        mesh.triangles = GenerateTries();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter = gameObject.AddComponent<MeshFilter>();

        meshFilter.mesh = mesh;
    }

    private Vector3[] GenerateVerts()
    {
        return new Vector3[]
        {
            corner1.transform.localPosition,
            corner2.transform.localPosition,
            corner3.transform.localPosition,
        };
    }
    private int[] GenerateTries()
    {
        return new int[]
        {
            0,2,1,
        };
    }



    // Update is called once per frame
    void Update()
    {
        mesh.vertices = GenerateVerts();
        //detect if cube is inside or outside 
        //PreviewCuts();

    }

    public void PopCube()
    {
        //flush lists and separate
        /*Destroy(og);
        cut1.GetComponent<MeshRenderer>().material = balckWireframe;
        cut2.GetComponent<MeshRenderer>().material = balckWireframe;
        isPreview = false;*/
        //from objects to lists
    }

    public void PreviewCuts()
    {

        //restore cut to og
        List<Transform> children = new List<Transform>();
        //2 pass because children are modified during second loop
        foreach (Transform child in cubeParent.transform)
        {
            children.Add(child);
        }

        foreach (Transform child in children)
        {
            Debug.Log(child.name);
            PreviewCut(child.gameObject);
        }
    }

    

    public void PreviewCut(GameObject cutted)
    {
        //Create a triangle between the tip and base so that we can get the normal
        Vector3 side1 = corner1.transform.position - corner2.transform.position;
        Vector3 side2 = corner1.transform.position - corner3.transform.position;

        //Get the point perpendicular to the triangle above which is the normal
        //https://docs.unity3d.com/Manual/ComputingNormalPerpendicularVector.html
        Vector3 normal = Vector3.Cross(side1, side2).normalized;

        //Transform the normal so that it is aligned with the object we are slicing's transform.
        Vector3 transformedNormal = ((Vector3)(cutted.transform.localToWorldMatrix.transpose * normal)).normalized;

        //Get the enter position relative to the object we're cutting's local transform
        Vector3 transformedStartingPoint = cutted.transform.InverseTransformPoint(corner2.transform.position);
        Plane plane = new Plane();
        plane.SetNormalAndPosition(
                transformedNormal,
                transformedStartingPoint);

        var direction = Vector3.Dot(Vector3.up, transformedNormal);
        //Flip the plane so that we always know which side the positive mesh is on
        if (direction < 0)
        {
            plane = plane.flipped;
        }
        GameObject[] slices = Slicer.Slice(plane, cutted);
        //Destroy(cutted);

        //new logic:
        /*og = other.gameObject;
        og.SetActive(false);
        cut1 = slices[0];
        cut2 = slices[1];
        cut1.GetComponent<MeshRenderer>().material = redWireframe;
        cut2.GetComponent<MeshRenderer>().material = blueWireframe;
        cutNormal = transformedNormal;
        */
        //change og to ogs.add ; likewise with cut1 and 2; relocate ispreview = true
       
    }


}
