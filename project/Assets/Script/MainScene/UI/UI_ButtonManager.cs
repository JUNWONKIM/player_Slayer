using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ButtonManager : MonoBehaviour
{
    public Button[] creatureButtons; // 버튼 배열 
    private PlayerHP playerHP;

    public Color disabledColor = Color.gray; // 비활성화 시의 색상
    public Color enabledColor = Color.white; // 활성화 시의 색상
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerHP = player.GetComponent<PlayerHP>();
        }

        // 처음 버튼 상태 업데이트
        UpdateButtonStates();
    }

    void Update()
    {
        if (playerHP != null)
        {      
            UpdateButtonStates();  // 매 프레임마다 버튼 상태 업데이트 
        }
    }


    void UpdateButtonStates()
    {
        // 1번과 2번 버튼은 항상 활성화
        UpdateButtonState(creatureButtons[0], true);
        UpdateButtonState(creatureButtons[1], true);

        // 3번 버튼은 체력이 70% 이하일 때만 활성화
        bool isCreature3Enabled = playerHP.hp <= playerHP.max_hp * 0.7f;
        UpdateButtonState(creatureButtons[2], isCreature3Enabled);

        // 4번 버튼은 체력이 50% 이하일 때만 활성화
        bool isCreature4Enabled = playerHP.hp <= playerHP.max_hp * 0.5f;
        UpdateButtonState(creatureButtons[3], isCreature4Enabled);
    }

    void UpdateButtonState(Button button, bool isEnabled)
    {
        button.interactable = isEnabled;

        // 버튼 색상 변경
        ColorBlock colors = button.colors;
        colors.disabledColor = disabledColor; // 비활성화 상태에서 회색으로 설정
        colors.normalColor = isEnabled ? enabledColor : disabledColor;
        button.colors = colors;
    }
}