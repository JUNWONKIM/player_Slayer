using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_titleSceneLoad : MonoBehaviour
{
    public Text blinkText; 
    public float blinkInterval = 0.5f; 

    private bool isBlinking = true;

    void Start()
    {
        if (blinkText != null)
        {
            StartCoroutine(BlinkText());
        }
    }

    void Update()
    {
      
        if (Input.anyKeyDown)
        {
            LoadTitleScene();
        }
    }

    void LoadTitleScene()
    {
        SceneManager.LoadScene("TitleScene"); 
    }

    IEnumerator BlinkText()
    {
        while (isBlinking)
        {
            blinkText.enabled = !blinkText.enabled; 
            yield return new WaitForSeconds(blinkInterval); 
        }
    }
}
