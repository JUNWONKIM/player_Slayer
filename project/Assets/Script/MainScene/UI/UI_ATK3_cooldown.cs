using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class UI_ATK3_cooldown : MonoBehaviour
{
    private Slider uiSlider;
    private bool isActive = false; //ui ��Ȱ��ȭ
    private GameObject bossPrefab;

    void Start()
    {
       
        GameObject sliderObject = GameObject.FindGameObjectWithTag("Slidebar_ATK3");

        if (sliderObject != null)
        {
            uiSlider = sliderObject.GetComponent<Slider>();
            uiSlider.gameObject.SetActive(false); //���� �� ��Ȱ��ȭ
        }
      
    }

   
    void Update()
    {
        bossPrefab = GameObject.FindGameObjectWithTag("Boss"); 

        if (bossPrefab != null && bossPrefab.activeInHierarchy) //���� �������� ������ ���
        {

            if (Input.GetKeyDown(KeyCode.C) && !isActive && uiSlider != null) //cŰ �Է� �� & ui ��Ȱ��ȭ ��
            {
                StartCoroutine(StartSliderCountdown());
            }
        }
    }

    private IEnumerator StartSliderCountdown() //�����̴� Ȱ��ȭ
    {
        isActive = true;

        uiSlider.gameObject.SetActive(true); // ui Ȱ��ȭ
        uiSlider.value = 1.0f; // �����̴��� �� �� ���·� ����

        float duration = 10.0f; //���ӽð�
        float startTime = Time.time; //���� �ð�

        while (Time.time < startTime + duration) // ���ӽð� ���� �����̴� ���� ����
        {
            float elapsed = Time.time - startTime;
            uiSlider.value = Mathf.Lerp(1.0f, 0.0f, elapsed / duration);
            yield return null;
        }

        uiSlider.value = 0.0f; //�����̴��� �� ���·� ����

        uiSlider.gameObject.SetActive(false); //�����̴� ��Ȱ��ȭ

        isActive = false;
    }
}
