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

    private bool wasPressedTrigger = false;

    public UnityEvent onPressTrigger;
    public UnityEvent onReleaseTrigger;

    public UnityEvent onPressPrimary;
    public UnityEvent onPressSecondary;

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
            if (!wasPressedTrigger)
            {
                onPressTrigger.Invoke();
                wasPressedTrigger = true;
            }
        }
        else
        {
            if (wasPressedTrigger)
            {
                onReleaseTrigger.Invoke();
                wasPressedTrigger = false;
            }
        }

        bool AButtonAction = false;
        if (device.TryGetFeatureValue(CommonUsages.primaryButton, out AButtonAction) && AButtonAction)
        {
            onPressPrimary.Invoke();
        }

        bool BButtonAction = false;
        if (device.TryGetFeatureValue(CommonUsages.secondaryButton, out BButtonAction) && BButtonAction)
        {
            onPressSecondary.Invoke();
        }
    }
}
