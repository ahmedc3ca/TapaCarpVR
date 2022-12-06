using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextPopup : MonoBehaviour
{   
    [SerializeField]
    GameObject canva;
    private AudioSource sound;
    void OnEnable()
    {
        transform.localScale = new Vector3(0, 0, 0);
        sound = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("something entered");
        LeanTween.scale(gameObject, new Vector3(1, 1, 1), 0.3f).setDelay(0.5f);
        sound.Play();
    }

    private void OnTriggerExit(Collider other)
    {
        LeanTween.scale(gameObject, new Vector3(0, 0, 0), 0.5f);
    }
}
