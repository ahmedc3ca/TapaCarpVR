using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Lighsaber : MonoBehaviour
{
    
    //The number of vertices to create per frame
    private const int NUM_VERTICES = 12;

     
    [SerializeField]
    [Tooltip("The empty game object located at the tip of the blade")]
    private GameObject _tip = null;

    [SerializeField]
    [Tooltip("The empty game object located at the base of the blade")]
    private GameObject _base = null;

    [SerializeField]
    [Tooltip("The mesh object with the mesh filter and mesh renderer")]
    private GameObject _meshParent = null;

    [SerializeField]
    [ColorUsage(true, true)]
    [Tooltip("The colour of the blade and trail")]
    private Color _colour = Color.red;

    [SerializeField]
    [Tooltip("The amount of force applied to each side of a slice")]
    private float _forceAppliedToCut = 3f;

    [SerializeField]
    private Material redWireframe;
    [SerializeField]
    private Material blueWireframe;
    [SerializeField]
    private Material balckWireframe;

    private Mesh _mesh;
    private Vector3[] _vertices;
    private Vector3 _triggerEnterTipPosition;
    private Vector3 _triggerEnterBasePosition;
    private Vector3 _triggerExitTipPosition;

    //Input logic
    public bool isPressed = false;
    private bool isPreview = false;

    private GameObject og;
    private GameObject cut1;
    private GameObject cut2;
    private Vector3 cutNormal;

    void Start()
    {
        //Init mesh and triangles
        _meshParent.transform.position = Vector3.zero;
        _mesh = new Mesh();
        _meshParent.GetComponent<MeshFilter>().mesh = _mesh;

    }


    public void TriggerPressed()
    {
        isPressed = true;
    }

    public void TriggerReleased()
    {
        isPressed = false;
    }
    public void ApplyCut()
    {
        if (isPreview)
        {
            Pop();
        }
    }

    public void RestoreCut()
    {
        if (isPreview)
        {
            Restore();
        }
    }

    private void Pop()
    {
        Destroy(og);
        cut1.GetComponent<MeshRenderer>().material = balckWireframe;
        cut2.GetComponent<MeshRenderer>().material = balckWireframe;
        Rigidbody rigidbody = cut2.GetComponent<Rigidbody>();
        Vector3 newNormal = cutNormal + Vector3.up * _forceAppliedToCut;
        rigidbody.AddForce(newNormal, ForceMode.Impulse);
        isPreview = false;
    }

    private void Restore()
    {
        Destroy(cut1);
        Destroy(cut2);
        og.SetActive(true);
        isPreview = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!isPressed) return;
        if (isPreview) Restore();
        _triggerEnterTipPosition = _tip.transform.position;
        _triggerEnterBasePosition = _base.transform.position;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isPressed) return;
        _triggerExitTipPosition = _tip.transform.position;

        //Create a triangle between the tip and base so that we can get the normal
        Vector3 side1 = _triggerExitTipPosition - _triggerEnterTipPosition;
        Vector3 side2 = _triggerExitTipPosition - _triggerEnterBasePosition;

        //Get the point perpendicular to the triangle above which is the normal
        //https://docs.unity3d.com/Manual/ComputingNormalPerpendicularVector.html
        Vector3 normal = Vector3.Cross(side1, side2).normalized;

        //Transform the normal so that it is aligned with the object we are slicing's transform.
        Vector3 transformedNormal = ((Vector3)(other.gameObject.transform.localToWorldMatrix.transpose * normal)).normalized;

        //Get the enter position relative to the object we're cutting's local transform
        Vector3 transformedStartingPoint = other.gameObject.transform.InverseTransformPoint(_triggerEnterTipPosition);

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

        GameObject[] slices = Slicer.Slice(plane, other.gameObject);
/*        Debug.Log(other.gameObject.name);
        Destroy(other.gameObject);

        Rigidbody rigidbody = slices[1].GetComponent<Rigidbody>();
        Vector3 newNormal = transformedNormal + Vector3.up * _forceAppliedToCut;
        rigidbody.AddForce(newNormal, ForceMode.Impulse);*/


        //new logic:
        og = other.gameObject;
        og.SetActive(false);
        cut1 = slices[0];
        cut2 = slices[1];
        cut1.GetComponent<MeshRenderer>().material = redWireframe;
        cut2.GetComponent<MeshRenderer>().material = blueWireframe;
        cutNormal = transformedNormal;
        isPreview = true;
    }
}
