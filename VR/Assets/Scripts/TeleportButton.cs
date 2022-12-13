using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TeleportButton : MonoBehaviour
{
    public GameObject button;
    public UnityEvent onPress;

    GameObject presser;
    AudioSource sound;
    bool isPressed;
    bool isDisabled;
    private int counter = 0;
    // Start is called before the first frame update
    void Start()
    {
        sound = GetComponent<AudioSource>();
        isDisabled = false;
        isPressed = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDisabled) return;
        if (!isPressed)
        {
            button.transform.localPosition = new Vector3(0, 0.005f, 0);
            presser = other.gameObject;
            onPress.Invoke();
            counter++;
            sound.Play();
            isPressed = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isDisabled) return;
        if (other.gameObject == presser)
        {
            button.transform.localPosition = new Vector3(0, 0.014f, 0);
            isPressed = false;
        }
    }


    public void DisableButton()
    {
        Debug.Log("Disabled");
        if (counter == 0) return;
        isDisabled = true;
        isPressed = false;
        button.transform.localPosition = new Vector3(0, 0.005f, 0);
        
    }

    public void EnableButton()
    {
        Debug.Log("enabled");
        if (counter == 0) return;
        isDisabled = false;
        button.transform.localPosition = new Vector3(0, 0.014f, 0);
    }


}
