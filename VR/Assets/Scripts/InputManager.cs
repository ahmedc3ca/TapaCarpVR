using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR;


public class InputManager : MonoBehaviour
{
    [SerializeField]
    private Text text; 

    [SerializeField]
    private XRNode xrNode = XRNode.LeftHand;

    private List<InputDevice> devices = new List<InputDevice>();
    private InputDevice device;

    private bool wasPressed = false;

    public UnityEvent onPress;
    public UnityEvent onRelease;

    void GetDevice()
    {
        InputDevices.GetDevicesAtXRNode(xrNode, devices);
        device = devices[0];

    }

    void OnEnable()
    {
        if (!device.isValid)
        {
            GetDevice();
        }
    }


    void Update()
    {
        if (!device.isValid)
        {
            GetDevice();
        }

        bool triggerButtonAction = false;
        if (device.TryGetFeatureValue(CommonUsages.triggerButton, out triggerButtonAction) && triggerButtonAction)
        {
            if (!wasPressed)
            {
                onPress.Invoke();
                wasPressed = true;
            }
        }
        else
        {
            if (wasPressed)
            {
                onRelease.Invoke();
                wasPressed = false;
            }
        }
    }
}
