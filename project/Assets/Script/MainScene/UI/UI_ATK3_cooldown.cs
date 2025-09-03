using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class UI_ATK3_cooldown : MonoBehaviour
{
    private Slider uiSlider;
    private bool isActive = false; //ui 비활성화
    private GameObject bossPrefab;

    void Start()
    {
       
        GameObject sliderObject = GameObject.FindGameObjectWithTag("Slidebar_ATK3");

        if (sliderObject != null)
        {
            uiSlider = sliderObject.GetComponent<Slider>();
            uiSlider.gameObject.SetActive(false); //시작 시 비활성화
        }
      
    }

   
    void Update()
    {
        bossPrefab = GameObject.FindGameObjectWithTag("Boss"); 

        if (bossPrefab != null && bossPrefab.activeInHierarchy) //보스 프리팹이 존재할 경우
        {

            if (Input.GetKeyDown(KeyCode.C) && !isActive && uiSlider != null) //c키 입력 시 & ui 비활성화 시
            {
                StartCoroutine(StartSliderCountdown());
            }
        }
    }

    private IEnumerator StartSliderCountdown() //슬라이더 활성화
    {
        isActive = true;

        uiSlider.gameObject.SetActive(true); // ui 활성화
        uiSlider.value = 1.0f; // 슬라이더를 꽉 찬 상태로 설정

        float duration = 10.0f; //지속시간
        float startTime = Time.time; //시작 시간

        while (Time.time < startTime + duration) // 지속시간 동안 슬라이더 값을 줄임
        {
            float elapsed = Time.time - startTime;
            uiSlider.value = Mathf.Lerp(1.0f, 0.0f, elapsed / duration);
            yield return null;
        }

        uiSlider.value = 0.0f; //슬라이더를 빈 상태로 설정

        uiSlider.gameObject.SetActive(false); //슬라이더 비활성화

        isActive = false;
    }
}
