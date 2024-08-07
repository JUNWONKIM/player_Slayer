using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class UI_ATK3_cooldown : MonoBehaviour
{
    public Slider slider; // 슬라이더 UI 컴포넌트
    public float duration = 10.0f; // 슬라이더가 줄어들 전체 시간

    void Start()
    {
        // 슬라이더를 처음에 비활성화 상태로 설정
        slider.gameObject.SetActive(false);
    }

    public void StartSlider()
    {
        StartCoroutine(StartSliderCountdown());
    }

    private IEnumerator StartSliderCountdown()
    {
        slider.gameObject.SetActive(true); // 슬라이더를 활성화
        slider.value = 1.0f; // 슬라이더를 꽉 찬 상태로 설정

        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            // 슬라이더의 값을 줄어들게 설정
            slider.value = Mathf.Lerp(1.0f, 0.0f, (Time.time - startTime) / duration);
            yield return null;
        }

        // 슬라이더가 완전히 빈 상태로 설정
        slider.value = 0.0f;

        // 슬라이더의 게임 오브젝트를 비활성화
        slider.gameObject.SetActive(false);
    }
}