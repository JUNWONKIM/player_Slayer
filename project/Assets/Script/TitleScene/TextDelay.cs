using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextDelay : MonoBehaviour
{
    public GameObject loadText;  
    public float delay = 5f;       

    void Start()
    {
       
        if (loadText != null)
        {
            loadText.SetActive(false);
        }

        Invoke("ShowObject", delay);
    }

    void ShowObject()
    {
        if (loadText != null)
        {
            loadText.SetActive(true);
        }
    }
}
