using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class UI_playerLV : MonoBehaviour
{
    public Text levelText; // 텍스트 UI 요소
    public PlayerLV playerLV; // PlayerLV 스크립트

    void Start()
    {
        UpdateLevelText(); 
    }

    void Update()
    {
        UpdateLevelText();
    }

    void UpdateLevelText() //레벨 텍스트 업데이트
    {
        levelText.text = "Lv :  " + playerLV.LV;
    }
}
