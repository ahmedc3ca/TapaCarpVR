using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CutterManager : MonoBehaviour
{
    [SerializeField]
    private GameObject corner1;
    [SerializeField]
    private GameObject corner2;
    [SerializeField]
    private GameObject corner3;

    [SerializeField]
    private Material redWireframe;
    [SerializeField]
    private Material blueWireframe;
    [SerializeField]
    private Material balckWireframe;
    [SerializeField]
    [Tooltip("The amount of force applied to each side of a slice")]
    private float _forceAppliedToCut = 3f;

    public GameObject lineRenderer;
    public GameObject red_lineRenderer;
    public GameObject blue_lineRenderer;

    protected MeshFilter meshFilter;
    protected Mesh mesh;


    List<GameObject> ogs;
    List<GameObject> rightCuts;
    List<GameObject> leftCuts;

    private Vector3 cutNormal;
    void Start()
    {
        ogs = new List<GameObject>();
        rightCuts = new List<GameObject>();
        leftCuts = new List<GameObject>();
    }


    private void Update()
    {
        PreviewCuts();
    }



    public void PopCubes()
    {
        Debug.Log("popping");
        for (int i = 0; i < ogs.Count; i++)
        {
            
            var cut1 = rightCuts[i];
            var cut2 = leftCuts[i];


            cut1.GetComponent<MeshRenderer>().material = balckWireframe;
            cut2.GetComponent<MeshRenderer>().material = balckWireframe;
            XRGrabInteractable grab1 = cut1.AddComponent<XRGrabInteractable>();
            XRGrabInteractable grab2 = cut2.AddComponent<XRGrabInteractable>();


            Sliceable originalSliceable = ogs[i].GetComponent<Sliceable>();

            Sliceable sliceable1 = cut1.AddComponent<Sliceable>();
            sliceable1.IsSolid = originalSliceable.IsSolid;
            sliceable1.ReverseWireTriangles = originalSliceable.ReverseWireTriangles;
            sliceable1.UseGravity = originalSliceable.UseGravity;

            Sliceable sliceable2 = cut2.AddComponent<Sliceable>();
            sliceable2.IsSolid = originalSliceable.IsSolid;
            sliceable2.ReverseWireTriangles = originalSliceable.ReverseWireTriangles;
            sliceable2.UseGravity = originalSliceable.UseGravity;

            Rigidbody rigidbody1 = cut1.GetComponent<Rigidbody>();
            Vector3 newNormal1 = -(cutNormal + Vector3.up * _forceAppliedToCut);
            rigidbody1.AddForce(newNormal1, ForceMode.Impulse);

            Rigidbody rigidbody2 = cut2.GetComponent<Rigidbody>();
            Vector3 newNormal2 = cutNormal + Vector3.up * _forceAppliedToCut;
            rigidbody2.AddForce(newNormal2, ForceMode.Impulse);


            StartCoroutine(EnableKinematic(rigidbody1, rigidbody2));
            Destroy(ogs[i]);
        }
        Debug.Log("dones");

        ogs = new List<GameObject>();
        rightCuts = new List<GameObject>();
        leftCuts = new List<GameObject>();

    }

    IEnumerator EnableKinematic(Rigidbody rb1, Rigidbody rb2)
    {
        yield return new WaitForSeconds(0.7f);

        rb1.isKinematic = true;
        rb2.isKinematic = true;
    }
    public void PreviewCuts()
    {
        for(int i = 0; i < ogs.Count; i++)
        {
            PreviewCut(ogs[i], i);
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Sliceable>() == null) return;
        Debug.Log("trigg enter");
        ogs.Add(other.gameObject);
        rightCuts.Add(null);
        leftCuts.Add(null);
    }

    private void OnTriggerExit(Collider other)
    {
        int index = ogs.FindIndex(a => a == other.gameObject);
        if (index == -1) return;
        ogs.RemoveAt(index);
        Destroy(leftCuts[index]);
        Destroy(rightCuts[index]);
        leftCuts.RemoveAt(index);
        rightCuts.RemoveAt(index);
        other.gameObject.GetComponent<Renderer>().enabled = true;
    }
    public void PreviewCut(GameObject cutted, int index)
    {
        //Create a triangle between the tip and base so that we can get the normal
        Vector3 side1 = corner1.transform.position - corner2.transform.position;
        Vector3 side2 = corner1.transform.position - corner3.transform.position;

        //Get the point perpendicular to the triangle above which is the normal
        //https://docs.unity3d.com/Manual/ComputingNormalPerpendicularVector.html
        Vector3 normal = Vector3.Cross(side1, side2).normalized;

        //Transform the normal so that it is aligned with the object we are slicing's transform.
        Vector3 transformedNormal = ((Vector3)(cutted.transform.localToWorldMatrix.transpose * normal)).normalized;
        cutNormal = transformedNormal;
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
        if (rightCuts[index] != null)
        {
            Destroy(rightCuts[index]);
        }
        if(leftCuts[index] != null)
        {
            Destroy(leftCuts[index]);
        }
        GameObject[] slices = Slicer.Slice(plane, cutted);
        //Destroy(cutted);
        cutted.GetComponent<Renderer>().enabled = false;
        cutted.GetComponent<Edge_Renderer>().DisableEdges();
        //cutted.GetComponent<XRGrabInteractable>().enabled = false;

        var cut1 = slices[0];
        var cut2 = slices[1];
        
        rightCuts[index] = cut1;
        leftCuts[index] = cut2;

        /*        cut1.GetComponent<MeshRenderer>().material = redWireframe;
                cut2.GetComponent<MeshRenderer>().material = blueWireframe;*/
        var rend1 = cut1.AddComponent<Edge_Renderer>();
        rend1.lineRenderer = red_lineRenderer;
        rend1.UpdateEdges();

        var rend2 = cut1.AddComponent<Edge_Renderer>();
        rend2.lineRenderer = blue_lineRenderer;
        rend2.UpdateEdges();
    }


}
