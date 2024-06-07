using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_playerLV : MonoBehaviour
{
    public Text levelText; // 텍스트 UI 요소
    public PlayerLV playerLV; // PlayerLV 스크립트

    void Start()
    {
        // 초기 텍스트 설정
        UpdateLevelText();
    }

    void Update()
    {
        // 매 프레임마다 텍스트 업데이트
        UpdateLevelText();
    }

    void UpdateLevelText()
    {
        levelText.text = "Lv :  " + playerLV.LV;
    }
}
