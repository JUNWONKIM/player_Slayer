using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class UI_ATK3_cooldown : MonoBehaviour
{
    private Slider uiSlider;
    private bool isActive = false;
    private GameObject bossObject;

    void Start()
    {
        // Slidebar_ATK3 태그를 가진 슬라이더를 찾음
        GameObject sliderObject = GameObject.FindGameObjectWithTag("Slidebar_ATK3");

        if (sliderObject != null)
        {
            uiSlider = sliderObject.GetComponent<Slider>();
            uiSlider.gameObject.SetActive(false); // 시작할 때 비활성화
        }
        else
        {
            Debug.LogWarning("Slidebar_ATK3 태그를 가진 슬라이더를 찾을 수 없습니다.");
        }
    }

    void Update()
    {
        // Boss 오브젝트를 찾음 (매 프레임마다 확인)
        bossObject = GameObject.FindGameObjectWithTag("Boss");

        // Boss 오브젝트가 활성화되어 있는 경우에만 슬라이더를 작동
        if (bossObject != null && bossObject.activeInHierarchy)
        {
            // C 키를 눌렀을 때 슬라이더를 활성화하고 카운트다운 시작
            if (Input.GetKeyDown(KeyCode.C) && !isActive && uiSlider != null)
            {
                StartCoroutine(StartSliderCountdown());
            }
        }
    }

    private IEnumerator StartSliderCountdown()
    {
        isActive = true;

        uiSlider.gameObject.SetActive(true); // 슬라이더 활성화
        uiSlider.value = 1.0f; // 슬라이더를 꽉 찬 상태로 설정

        float startTime = Time.time;

        while (Time.time < startTime + 10.0f) // 10초 동안 슬라이더 값을 줄임
        {
            uiSlider.value = Mathf.Lerp(1.0f, 0.0f, (Time.time - startTime) / 10.0f);
            yield return null;
        }

        // 슬라이더가 완전히 빈 상태로 설정
        uiSlider.value = 0.0f;

        // 슬라이더를 비활성화
        uiSlider.gameObject.SetActive(false);

        isActive = false;
    }
}