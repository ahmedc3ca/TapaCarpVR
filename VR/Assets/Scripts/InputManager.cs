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

    private List<InputDevice> devices = new List<InputDevice>();
    private InputDevice ldevice;
    private InputDevice rdevice;

    private bool wasPressedTrigger = false;

    public UnityEvent onPressTrigger;
    public UnityEvent onReleaseTrigger;

    public UnityEvent onPressPrimary;
    public UnityEvent onPressSecondary;

    void GetDevice()
    {
        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, devices);
        ldevice = devices[0];
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, devices);
        rdevice = devices[0];

    }

    void OnEnable()
    {
        if (!(ldevice.isValid && rdevice.isValid))
        {
            GetDevice();
        }
    }


    void Update()
    {
        if (!(ldevice.isValid && rdevice.isValid))
        {
            GetDevice();
        }

        bool ltriggerButtonAction = false;
        bool rtriggerButtonAction = false;
        if ((ldevice.TryGetFeatureValue(CommonUsages.triggerButton, out ltriggerButtonAction) && ltriggerButtonAction) || (rdevice.TryGetFeatureValue(CommonUsages.triggerButton, out rtriggerButtonAction) && rtriggerButtonAction))
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

        bool lAButtonAction = false;
        bool rAButtonAction = false;
        if ((ldevice.TryGetFeatureValue(CommonUsages.primaryButton, out lAButtonAction) && lAButtonAction)|| (rdevice.TryGetFeatureValue(CommonUsages.primaryButton, out rAButtonAction) && rAButtonAction))
        {
            onPressPrimary.Invoke();
        }

        bool lBButtonAction = false;
        bool rBButtonAction = false;
        if ((ldevice.TryGetFeatureValue(CommonUsages.secondaryButton, out lBButtonAction) && lBButtonAction) || (rdevice.TryGetFeatureValue(CommonUsages.secondaryButton, out rBButtonAction) && rBButtonAction))
        {
            onPressSecondary.Invoke();
        }
    }
}
