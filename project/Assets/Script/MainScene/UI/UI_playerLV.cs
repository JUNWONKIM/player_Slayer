using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class UI_playerLV : MonoBehaviour
{
    public Text levelText; // �ؽ�Ʈ UI ���
    public PlayerLV playerLV; // PlayerLV ��ũ��Ʈ

    void Start()
    {
        UpdateLevelText(); 
    }

    void Update()
    {
        UpdateLevelText();
    }

    void UpdateLevelText() //���� �ؽ�Ʈ ������Ʈ
    {
        levelText.text = "Lv :  " + playerLV.LV;
    }
}
