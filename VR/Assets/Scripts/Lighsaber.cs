using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class Lighsaber : MonoBehaviour
{
    
    //The number of vertices to create per frame
    private const int NUM_VERTICES = 12;

    [SerializeField]
    [Tooltip("The blade gameobject")]
    private GameObject _blade = null;

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
    private bool isPressed = false;
    private bool isPreview = false;

    private GameObject og;
    private GameObject cut1;
    private GameObject cut2;
    private Vector3 cutNormal;

    private AudioSource sound;

    private static int CUBE_LAYER = 6;

    void Start()
    {
        //Init mesh and triangles
        _meshParent.transform.position = Vector3.zero;
        _mesh = new Mesh();
        _meshParent.GetComponent<MeshFilter>().mesh = _mesh;
        _blade.GetComponent<Renderer>().enabled = false;
        sound = GetComponent<AudioSource>();
    }


    public void TriggerPressed()
    {
        isPressed = true;
        sound.Play();
        _blade.GetComponent<Renderer>().enabled = true;
    }

    public void TriggerReleased()
    {
        isPressed = false;
        _blade.GetComponent<Renderer>().enabled = false;
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
        
        cut1.GetComponent<MeshRenderer>().material = balckWireframe;
        cut2.GetComponent<MeshRenderer>().material = balckWireframe;

        XRGrabInteractable grab1 = cut1.AddComponent<XRGrabInteractable>();
        XRGrabInteractable grab2 = cut2.AddComponent<XRGrabInteractable>();

        Sliceable originalSliceable = og.GetComponent<Sliceable>();
        
        Sliceable sliceable1 = cut1.AddComponent<Sliceable>();
        sliceable1.IsSolid = originalSliceable.IsSolid;
        sliceable1.ReverseWireTriangles = originalSliceable.ReverseWireTriangles;
        sliceable1.UseGravity = originalSliceable.UseGravity;

        Sliceable sliceable2 = cut2.AddComponent<Sliceable>();
        sliceable2.IsSolid = originalSliceable.IsSolid;
        sliceable2.ReverseWireTriangles = originalSliceable.ReverseWireTriangles;
        sliceable2.UseGravity = originalSliceable.UseGravity;


        Rigidbody rigidbody = cut2.GetComponent<Rigidbody>();
        Vector3 newNormal = cutNormal + Vector3.up * _forceAppliedToCut;
        rigidbody.AddForce(newNormal, ForceMode.Impulse);

        StartCoroutine(EnableKinematic(rigidbody));
        Destroy(og);
        isPreview = false;
    }

    IEnumerator EnableKinematic(Rigidbody rb)
    {
        yield return new WaitForSeconds(0.7f);

        rb.isKinematic = true;
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
        if (other.tag != "Cube") return;
        //if (other.GetComponent<Sliceable>() == null) return;
        if (!isPressed) return;
        if (isPreview) Restore();
        _triggerEnterTipPosition = _tip.transform.position;
        _triggerEnterBasePosition = _base.transform.position;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Cube") return;
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
