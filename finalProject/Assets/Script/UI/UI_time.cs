using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_time : MonoBehaviour
{
    public Text timerText;
    private float elapsedTime = 0f;

    void Update()
    {
        // 경과 시간 업데이트
        elapsedTime += Time.deltaTime;

        // 분과 초 계산
        int minutes = Mathf.FloorToInt(elapsedTime / 60F);
        int seconds = Mathf.FloorToInt(elapsedTime % 60F);

        // "00:00" 형식으로 텍스트 설정
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

}
